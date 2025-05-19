using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;


public class FarmerMobBehavior : IMobBehavior    //Listen to invoker if max capacity reached, so nothing happens afterwards default => break  
{

    private bool buildingOrTile;  //Not flexible!

    private bool didICollectFromItem = false;

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
                // .Where(m => m.GetItemClass() == itemName)
                .OrderBy(m => {
                    Vector2 pos1 = new Vector2(mob.transform.position.x, mob.transform.position.z);
                    Vector2 pos2 = new Vector2(m.gameObject.transform.position.x, m.gameObject.transform.position.z);
                    return Vector2.Distance(pos1, pos2);
                })
                .FirstOrDefault();

        return targetItem;
    }


    private void Move(){
        Vector3 currentPosition = mob.transform.position;
        Vector3 targetPosition = mob.toDestination;

        Vector3 direction = targetPosition - currentPosition;
        direction.y = 0; // Ignore Y axis for direction calculation

        // Prevent division by zero if already at target on XZ plane
        if (direction.sqrMagnitude < 0.0001f) {
            mob.isMoving = false;
            // Ensure exact position on XZ plane, keep original Y
            mob.transform.position = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);
            mob.toColliderObj.GetComponent<DefaultTile>().ArrangeMobs(mob);
            return;
        }
        
        Vector3 normalizedDirection = direction.normalized;

        Vector3 movement = normalizedDirection * mob.speedFactor;
        mob.transform.position = new Vector3(currentPosition.x + movement.x, currentPosition.y, currentPosition.z + movement.z); // Keep current Y

        Vector2 currentPos2D = new Vector2(mob.transform.position.x, mob.transform.position.z);
        Vector2 destinationPos2D = new Vector2(mob.toDestination.x, mob.toDestination.z);

        if(Vector2.Distance(currentPos2D, destinationPos2D) < 0.01f){
            Debug.Log("Reached the closest point on tile to my DefaultMob object");
            mob.isMoving = false;
            // Snap to destination XZ, keep current Y
            mob.transform.position = new Vector3(mob.toDestination.x, currentPosition.y, mob.toDestination.z);
            mob.toColliderObj.GetComponent<DefaultTile>().ArrangeMobs(mob);
            
            // I want to set the mob in a  specific order regarding the tile, involving taking into account possible other mobs from the same tile. 
        }
    }


    private void LoopMove(){
        Debug.Log("VERSION CONTROL We are at the loop move " );
        DefaultBuild building = mob.toColliderObj.GetComponent<DefaultBuild>();
        Item item = mob.toColliderObj.GetComponent<Item>();

        if(building != null){
            Debug.Log("VERSION CONTROL Building not null " + building.GetBuildingClass());

            
            Vector3 currentPosition = mob.transform.position;
            Vector3 targetPosition = mob.toDestination;
            Vector3 direction = targetPosition - currentPosition;
            direction.y = 0; // Ignore Y axis for direction calculation

            // Prevent division by zero or tiny movements if already at target on XZ plane
            if (direction.sqrMagnitude < 0.0001f) 
            {
                 // Process arrival logic directly
                Debug.Log("VERSION CONTROL we are very close to the building (LoopMove), distance reached ");
                string closestItemName = ItemBuilding.Instance.GetItemName(building.GetBuildingClass());
                closestItem = FindClosestItem(closestItemName);
                if (closestItem == null)
                {
                    mob.isMoving = false;
                    return;
                }
                mob.toColliderObj = closestItem.gameObject;
                mob.toDestination = closestItem.transform.position; // This destination will be used in the next frame
                Debug.Log("VERSION CONTROL didICollectFromItem is  " + didICollectFromItem);
                if (didICollectFromItem)
                {
                    if (ItemDatabase.Instance == null)
                    {
                        Debug.LogError("ItemDatabase.Instance is null! Make sure it's initialized in the scene.");
                    }
                    else
                    {
                        Debug.Log("ItemDatabase not null, different problem!");
                        ItemDatabase.Instance.UpdateCollectedItemsCount(closestItem, 1); 
                    }
                    didICollectFromItem = false;
                }
                // Snap to destination XZ, keep current Y
                mob.transform.position = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);
                return; // Movement handled or destination changed
            }

            Vector3 normalizedDirection = direction.normalized;
            Vector3 movement = normalizedDirection * mob.speedFactor;
            mob.transform.position = new Vector3(currentPosition.x + movement.x, currentPosition.y, currentPosition.z + movement.z); // Keep current Y


            Vector2 currentPos2D = new Vector2(mob.transform.position.x, mob.transform.position.z);
            Vector2 destinationPos2D = new Vector2(mob.toDestination.x, mob.toDestination.z);

            if(Vector2.Distance(currentPos2D, destinationPos2D) < 0.01f){
                Debug.Log("VERSION CONTROL we are very close to the building, distance reached " );
                // Snap to destination XZ, keep current Y
                mob.transform.position = new Vector3(mob.toDestination.x, currentPosition.y, mob.toDestination.z);

                string closestItemName = ItemBuilding.Instance.GetItemName(building.GetBuildingClass());
                closestItem = FindClosestItem(closestItemName);
                if (closestItem == null)
                {
                    mob.isMoving = false;
                    return;
                }
                mob.toColliderObj = closestItem.gameObject;
                mob.toDestination = closestItem.transform.position;
                Debug.Log("VERSION CONTROL didICollectFromItem is  " + didICollectFromItem);
                if (didICollectFromItem)
                {

                    //Null checking debug code:

                    if (ItemDatabase.Instance == null)
                    {
                        Debug.LogError("ItemDatabase.Instance is null! Make sure it's initialized in the scene.");   //Yeah, it's null.....
                    }
                    else
                    {
                        Debug.Log("ItemDatabase not null, different problem!");
                    }


                    //Null checking debug code:

                    Debug.Log("VERSION CONTROL Closest item: " + closestItem.GetItemClass());
                    ItemDatabase.Instance.UpdateCollectedItemsCount(closestItem, 1); //Hardcoded the value you get by collecting a material
                    didICollectFromItem = false;
                }
            }
        }

        else if(item != null){
            if (didICollectFromItem && counter > 0)
            {
                counter -= Time.deltaTime;
                return;
            }
            else if (counter <= 0)
            {
                counter = 5; // Reset counter
                // Ensure mob.buidlingAssignedTo is not null before accessing its properties
                if (mob.buidlingAssignedTo != null && mob.buidlingAssignedTo.gameObject != null) {
                    mob.toColliderObj = mob.buidlingAssignedTo.gameObject;
                    mob.toDestination = mob.buidlingAssignedTo.transform.position;
                } else {
                    Debug.LogError("mob.buidlingAssignedTo or its gameObject is null!");
                    mob.isMoving = false; // Stop movement if assignment is invalid
                    return;
                }
            }
            
            Debug.Log("VERSION CONTROL Item not null " );
            Vector3 currentPosition = mob.transform.position;
            Vector3 targetPosition = mob.toDestination;
            Vector3 direction = targetPosition - currentPosition;
            direction.y = 0; // Ignore Y axis for direction calculation

            // Prevent division by zero or tiny movements if already at target on XZ plane
            if (direction.sqrMagnitude < 0.0001f)
            {
                // Process arrival logic directly
                Debug.Log("VERSION CONTROL we are very close to the item (LoopMove), distance reached ");
                didICollectFromItem = true;
                // Snap to destination XZ, keep current Y
                mob.transform.position = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);
                // Potentially stop moving or set next state if item interaction is complete here
                // For now, it will re-evaluate didICollectFromItem in the next frame
                return; 
            }

            Vector3 normalizedDirection = direction.normalized;
            Vector3 movement = normalizedDirection * mob.speedFactor;
            mob.transform.position = new Vector3(currentPosition.x + movement.x, currentPosition.y, currentPosition.z + movement.z); // Keep current Y

            Vector2 currentPos2D = new Vector2(mob.transform.position.x, mob.transform.position.z);
            Vector2 destinationPos2D = new Vector2(mob.toDestination.x, mob.toDestination.z);

            if (Vector2.Distance(currentPos2D, destinationPos2D) < 0.01f)
            {
                Debug.Log("VERSION CONTROL we are very close to the item, distance reached ");
                 // Snap to destination XZ, keep current Y
                mob.transform.position = new Vector3(mob.toDestination.x, currentPosition.y, mob.toDestination.z);
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