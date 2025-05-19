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

    private DefaultTile generalTile = null;



    public void Init(DefaultMob mob)
    {
        this.mob = mob;
        generalTile = BetterGridOverlay.Instance.GetTiles()[0].GetComponent<DefaultTile>(); 
        Debug.Log("General tile is " + generalTile.name);
    }


    public void ActionLoop()
    {
        if (mob.isMoving)
        {
            if (buildingOrTile)
            {
                LoopMove();
            }
            else if (!buildingOrTile)
            {
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
                .OrderBy(m => Vector3.Distance(mob.transform.position, m.gameObject.transform.position))
                .FirstOrDefault();

        return targetItem;
    }


    private void Move()
    {
        Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
        mob.transform.position += dir * mob.speedFactor;




        if (Vector3.Distance(mob.transform.position, mob.toDestination) < 0.01f)
        {
            Debug.Log("Reached the closest point on tile to my DefaultMob object");
            mob.isMoving = false;
            // transform.position = toDestination;
            mob.toColliderObj.GetComponent<DefaultTile>().ArrangeMobs(mob);

            // I want to set the mob in a  specific order regarding the tile, involving taking into account possible other mobs from the same tile. 
        }
    }


    private void LoopMove()
    {
        Debug.Log("VERSION CONTROL We are at the loop move ");
        DefaultBuild building = mob.toColliderObj.GetComponent<DefaultBuild>();
        DefaultItem item = mob.toColliderObj.GetComponent<DefaultItem>();

        if (building != null)
        {
            Debug.Log("VERSION CONTROL Building not null " + building.GetBuildingClass());


            //mob.toDestination = new Vector3(mob.toDestination.x, vectorYHeightGivenTile(mob.currentTile, building.gameObject), mob.toDestination.z);


            Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
            mob.transform.position += dir * mob.speedFactor;

            if (Vector3.Distance(mob.transform.position, mob.toDestination) < 0.01f)
            {
                Debug.Log("VERSION CONTROL we are very close to the building, distance reached ");


                string closestItemName = ItemBuilding.Instance.GetItemName(building.GetBuildingClass());
                closestItem = FindClosestItem(closestItemName);
                if (closestItem == null)
                {
                    mob.isMoving = false;
                    return;
                }
                mob.toColliderObj = closestItem.gameObject;


                mob.toDestination = closestItem.transform.position;

                mob.toDestination = new Vector3(mob.toDestination.x, mob.transform.position.y, mob.toDestination.z);




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

        else if (item != null)
        {
            if (didICollectFromItem && counter > 0)
            {
                counter -= Time.deltaTime;
                return;
            }
            else if (counter <= 0)
            {

                // didICollectFromItem = true;
                counter = 5;
                mob.toColliderObj = mob.buidlingAssignedTo.gameObject;
                mob.toDestination = mob.buidlingAssignedTo.transform.position;


                mob.toDestination = new Vector3(mob.toDestination.x, mob.transform.position.y, mob.toDestination.z);


            }

            Debug.Log("VERSION CONTROL Item not null ");


            //mob.toDestination = new Vector3(mob.toDestination.x, vectorYHeightGivenTile(mob.currentTile, building.gameObject), mob.toDestination.z);

            Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
            mob.transform.position += dir * mob.speedFactor;


            if (Vector3.Distance(mob.transform.position, mob.toDestination) < 0.01f)
            {
                Debug.Log("VERSION CONTROL we are very close to the item, distance reached ");

                // start timer
                // wait for timer
                //startTimer();
                didICollectFromItem = true;





            }
        }
    }


    float vectorYHeightGivenTile(DefaultTile tile, GameObject gameObject)
    {  //Incorrect


        DefaultItem item = gameObject.GetComponent<DefaultItem>();
        DefaultBuild build = gameObject.GetComponent<DefaultBuild>();


        if (item != null)
        {
            Renderer tileRenderer = tile.GetComponent<Renderer>();
            Renderer objectRenderer = item.GetComponent<Renderer>();

            float tileTopY = tileRenderer.bounds.max.y;
            float objectBottomOffset = objectRenderer.bounds.min.y - item.transform.position.y;
            float spawnY = tileTopY - objectBottomOffset;


            //float height = tilePosition.y + ((tileHeight + (mobHeight / 2f))  / 1f);

            Debug.Log("spawnY is " + spawnY);
            return spawnY;
        }
        else if (build != null)
        {
            Renderer tileRenderer = tile.GetComponent<Renderer>();
            Renderer objectRenderer = build.GetComponent<Renderer>();

            float tileTopY = tileRenderer.bounds.max.y;
            float objectBottomOffset = objectRenderer.bounds.min.y - build.transform.position.y;
            float spawnY = tileTopY - objectBottomOffset;

            Debug.Log("spawnY is " + spawnY);
            return spawnY;
        }

        Debug.Log("spawnY is " + 0);

        return 0;

    

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