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

    [SerializeField] public float speedFactor = 0.008f; // I don't want now to add get + set methods 

    

    public  Vector3 toDestination; // I don't want now to add get + set methods

    public  GameObject toColliderObj; // I don't want now to add get + set methods

    private IMobBehavior currentBehavior;


    public DefaultTile currentTile = null;  // I don't want now to add get + set methods



    // Start is called before the first frame update
    void Start()
    {
        
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

}
