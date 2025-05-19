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

    [SerializeField] public float speedFactor = 0.001f; // I don't want now to add get + set methods 

    public  Vector3 toDestination; // I don't want now to add get + set methods

    public  GameObject toColliderObj; // I don't want now to add get + set methods

    private IMobBehavior currentBehavior;


    public DefaultTile currentTile = null;  // I don't want now to add get + set methods

    private ResourceDatabase resources; // Singleton instance of ResourceDatabase

    // Start is called before the first frame update


    private float mightPower = 10f;



    void Start()
    {
        while (resources == null){
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }
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
                SetMobBehavior(new FarmerMobBehavior());
                break;

            case "sleep":
                SetMobBehavior(new FarmerMobBehavior());
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
