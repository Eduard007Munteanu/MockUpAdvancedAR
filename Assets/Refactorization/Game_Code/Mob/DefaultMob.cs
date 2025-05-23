using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;

public class DefaultMob : MonoBehaviour, Mobs  //Not abstract now, given no other mob class for now
{

    public bool isMoving = false;  // I don't want now to add get + set methods

    public DefaultBuild buidlingAssignedTo; // I don't want now to add get + set methods

    [SerializeField] public float speedFactor = 0.0005f; // I don't want now to add get + set methods 

    public  Vector3 toDestination; // I don't want now to add get + set methods

    public  GameObject toColliderObj; // I don't want now to add get + set methods

    private IMobBehavior currentBehavior;


    public DefaultTile currentTile = null;  // I don't want now to add get + set methods

    private ResourceDatabase resources; // Singleton instance of ResourceDatabase

    // Start is called before the first frame update


    private float mightPower = 10f;

    private float heightY = 0f;



    void Start()
    {
        while (resources == null)
        {
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }

        // initialY = this.gameObject.transform.position.y;
    }

    public float GetMightPower(){
        return mightPower;
    }

    public void SetMightPower(float changedMight){
        mightPower = changedMight;
    }

    // Update is called once per frame
    void Update()
    {
        currentBehavior?.ActionLoop();
        // this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, heightY, this.gameObject.transform.position.z);
        // this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, initialY, this.gameObject.transform.position.z);
    }

    public void SetMobBehavior(IMobBehavior newBehavior){
        currentBehavior = newBehavior;
        currentBehavior.Init(this);
    }


    public void InitMove(Vector3 destination, GameObject colliderObj)
    {

        if(currentTile != null){
            currentTile.RemoveMob(this);
            currentTile = null;
        }

        currentBehavior?.InitMove(destination, colliderObj);
    }


    public void ReactOnClick(){
        //currentBehavior.OnClick();
        isMoving = false;
    }



    public DefaultBuild GetBuildingAssignedTo()
    {
        return buidlingAssignedTo;
    }


    private void SetY(float toHeight)
    {
        heightY = toHeight;
    }

    private float getY()
    {
        return heightY;
    }


    public bool DidSetY(float toHeight)
    {
        if (getY() != 0f)
        {
            return true;
        }
        else
        {
            SetY(toHeight);
            return false;
        }
    }
    
    
    public void AssignToBuilding(DefaultBuild building)
    {
        buidlingAssignedTo = building;
        buidlingAssignedTo.AddAssignedMob(this);
    }

    public void RemoveFromBuilding()
    {
        if (buidlingAssignedTo != null)
        {
            if (buidlingAssignedTo.GetSpecificActualMob(this) != null)
            {
                buidlingAssignedTo.RemoveAssignedMob(this);
            }

            buidlingAssignedTo = null;
        }
    }

    public float GetMobHeight()
    {
        return gameObject.GetComponent<Renderer>().bounds.size.y;
    }

    public void SetBehaviorBasedOnBuilding(DefaultBuild building)   //Need a factory code for this, otherwise a little annoying.
    {                                                                   //Will factory be implemented? Likely not!
        string type = null;
        if(building != null){
            type = building.GetBuildingClass();
        } 
        
        switch (type)
        {
            case "farming":
                SetMobBehavior(new FarmerMobBehavior());
                break;

            case "military":
                SetMobBehavior(new JustGoBehavior());
                break;

            case "sleep":
                SetMobBehavior(new JustGoBehavior());
                break;

            case "Main":
                SetMobBehavior(new JustGoBehavior());
                break;

            case null:
                SetMobBehavior(new JustGoBehavior());
                break;

            default:
                Debug.LogWarning("Unknown building type: " + type);
                break;
        }
    }

    void OnDestroy() {
        resources[ResourceType.Population].AddAmount(-1f); // Decrease population when mob is destroyed
    }
}
