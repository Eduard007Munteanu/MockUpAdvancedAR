using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;

public abstract class DefaultMob : MonoBehaviour, Mobs
{

    private bool isMoving = false;

    private DefaultBuild buidlingAssignedTo;

    [SerializeField] private float speedFactor = 0.008f;

    private ItemBuilding itemBuilding;

    private DefaultItem closestItem; 


    private Vector3 toDestination;

    private GameObject toColliderObj;

    private bool buildingOrTile;  //Not flexible!

    private bool didICollectFromItem;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       SwitchAction();
    }


    public void ReactOnClick()
    {
        isMoving = false;

    }

    public DefaultBuild GetBuildingAssignedTo()
    {
        return buidlingAssignedTo;
    }
    
    
    public void AssignToBuilding(DefaultBuild building)
    {
        buidlingAssignedTo = building;
    }

    public void RemoveFromBuilding()
    {
        buidlingAssignedTo = null;
    }

    // public void CollectItem(PanelDatabase panelDatabase)
    // {
    //     throw new System.NotImplementedException();
    // }

    public DefaultItem FindClosestItem(string itemName)
    {
        DefaultItem targetItem = FindObjectsOfType<MonoBehaviour>().OfType<DefaultItem>()
                .Where(m => m.GetItemClass() == itemName)
                .OrderBy(m => Vector3.Distance(transform.position, ((MonoBehaviour)m).gameObject.transform.position))
                .FirstOrDefault();

        return targetItem;
    }

    public void InitMove(Vector3 destination, GameObject colliderObj)
    {
        toDestination = destination;
        toColliderObj = colliderObj;
        isMoving = true;

        Build building = toColliderObj.GetComponent<Build>();
        Tile tile = toColliderObj.GetComponent<Tile>();


        if(building != null){
            buildingOrTile = true;
        }
        else if(tile != null){
            buildingOrTile = false;
        }
        
    }

    public void Move(){
        Vector3 dir = (toDestination - transform.position).normalized;
        transform.position += dir * speedFactor;

        if(Vector3.Distance(transform.position, toDestination) < 0.1f){
            isMoving = false;
            transform.position = toDestination;
        }
    }

    

    public void LoopMove(){
        DefaultBuild building = toColliderObj.GetComponent<DefaultBuild>();
        Item item = toColliderObj.GetComponent<Item>();

        if(building != null){
            Vector3 dir = (toDestination - transform.position).normalized;
            transform.position += dir * speedFactor;

            if(Vector3.Distance(transform.position, toDestination) < 0.1f){
                string closestItemName = itemBuilding.GetItemName(building.GetBuildingClass());
                closestItem = FindClosestItem(closestItemName);
                if (closestItem == null)
                {
                    isMoving = false;
                    return;
                }
                toColliderObj = closestItem.gameObject;
                toDestination = closestItem.transform.position;
                if(didICollectFromItem){
                    ItemDatabase.Instance.UpdateCollectedItemsCount(closestItem, 1); //Hardcoded the value you get by collecting a material
                    didICollectFromItem = false;
                }
            }
        }

        else if(item != null){
            Vector3 dir = (toDestination - transform.position).normalized;
            transform.position += dir * speedFactor;

            if(Vector3.Distance(transform.position, toDestination) < 0.1f){
                toColliderObj = buidlingAssignedTo.gameObject;
                toDestination = buidlingAssignedTo.transform.position;
                didICollectFromItem = true;
            }
        }
    }

    public void SwitchAction(){
        if(isMoving){
            if(buildingOrTile){
                LoopMove();
            }
            else if(!buildingOrTile){
                Move();
            }
        }
        
    }

    public float GetMobHeight()
    {
        throw new System.NotImplementedException();
    }
}
