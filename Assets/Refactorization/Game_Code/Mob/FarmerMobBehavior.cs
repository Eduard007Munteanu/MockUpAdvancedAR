using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;


public class FarmerMobBehavior : IMobBehavior    //Listen to invoker if max capacity reached, so nothing happens afterwards default => break  
{

    private bool buildingOrTile;  //Not flexible!

    private bool didICollectFromItem;

    private DefaultItem closestItem; 

    private DefaultMob mob;

    private float counter = 1f;

    

    public void Init(DefaultMob mob)
    {
        this.mob = mob;
    }


    public void ActionLoop()
    {
        if(mob.isMoving){
            if(buildingOrTile){
                LoopMove();
            }
            else if(!buildingOrTile){
                Move();
            }
        }
    }

    public void InitMove(Vector3 destination, GameObject colliderObj)
    {
        //mob.RemoveFromBuilding();

        mob.toDestination = destination;
        mob.toColliderObj = colliderObj;
        mob.isMoving = true;

        if (colliderObj.TryGetComponent<Build>(out _))
        {
            buildingOrTile = true;
        }
        else if (colliderObj.TryGetComponent<DefaultTile>(out _))
        {
            Debug.Log("Reached here, ma dude!");
            buildingOrTile = false;
        }
        else
        {
            // Fallback: treat as generic movement
            buildingOrTile = false;
        }
    }

    public void OnClick()
    {
        mob.isMoving = false;
    }


    private DefaultItem FindClosestItem(string itemName)
    {
        DefaultItem targetItem = Object.FindObjectsOfType<MonoBehaviour>().OfType<DefaultItem>()
                .Where(m => m.GetItemClass() == itemName)
                .OrderBy(m => Vector3.Distance(mob.transform.position, m.gameObject.transform.position))
                .FirstOrDefault();

        return targetItem;
    }


    private void Move(){
        Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
        mob.transform.position += dir * mob.speedFactor;

        


        if(Vector3.Distance(mob.transform.position, mob.toDestination) < 0.1f){
            Debug.Log("Reached the closest point on tile to my DefaultMob object");
            mob.isMoving = false;
            // transform.position = toDestination;
            mob.toColliderObj.GetComponent<DefaultTile>().ArrangeMobs(mob);
            
            // I want to set the mob in a  specific order regarding the tile, involving taking into account possible other mobs from the same tile. 
        }
    }


    private void LoopMove(){
        DefaultBuild building = mob.toColliderObj.GetComponent<DefaultBuild>();
        Item item = mob.toColliderObj.GetComponent<Item>();

        if(building != null){

            if(didICollectFromItem && counter > 0){
                counter -= Time.deltaTime;
                return; 
            } else if(counter <= 0){
                didICollectFromItem  = false;
                counter = 5;
            }

            Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
            mob.transform.position += dir * mob.speedFactor;

            if(Vector3.Distance(mob.transform.position, mob.toDestination) < 0.1f){
                string closestItemName = ItemBuilding.Instance.GetItemName(building.GetBuildingClass());
                closestItem = FindClosestItem(closestItemName);
                if (closestItem == null)
                {
                    mob.isMoving = false;
                    return;
                }
                mob.toColliderObj = closestItem.gameObject;
                mob.toDestination = closestItem.transform.position;
                if(didICollectFromItem){

                    //Null checking debug code:

                    if (ItemDatabase.Instance == null)
                    {
                        Debug.LogError("ItemDatabase.Instance is null! Make sure it's initialized in the scene.");   //Yeah, it's null.....
                    }
                    else{
                        Debug.Log("ItemDatabase not null, different problem!");
                    }


                    //Null checking debug code:

                    ItemDatabase.Instance.UpdateCollectedItemsCount(closestItem, 1); //Hardcoded the value you get by collecting a material
                    didICollectFromItem = false;
                }
            }
        }

        else if(item != null){
            Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
            mob.transform.position += dir * mob.speedFactor;

            if(Vector3.Distance(mob.transform.position, mob.toDestination) < 0.1f){
                mob.toColliderObj = mob.buidlingAssignedTo.gameObject;
                mob.toDestination = mob.buidlingAssignedTo.transform.position;
                // start timer
                // wait for timer
                //startTimer();
                didICollectFromItem = true;

            }
        }
    }

    // private void startTimer() {
    //     // Start a timer for 1 seconds
    //     float timer = 0f;
    //     while (timer < 1f)
    //     {
    //         timer += Time.deltaTime;
    //     }
    // }


    
}