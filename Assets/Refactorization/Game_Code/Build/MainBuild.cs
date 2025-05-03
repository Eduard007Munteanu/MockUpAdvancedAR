using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit.SceneDecorator;
using Unity.VisualScripting;
using UnityEngine;

public class MainBuild : DefaultBuild
{


     private GridOverlay gridOverlay;


    

    [SerializeField] private DefaultMob mobPrefab;

    protected override string Building_class => "Main";

    private Tile tile;

    



    // Start is called before the first frame update
    void Start()
    {
        
        //Debug.Log("GridOverlay is ", gridOverlay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init(int Id, DefaultTile tile = null){
        // this.Id = Id;
        // this.building_class_main = main_class;
        base.Init(Id, tile);
        gridOverlay = GridOverlay.Instance;

    }

    public override Vector3 SpawnBuilding(){
        tile = TileFindCalculation();
        float tileHeight = tile.GetTileHeight();
        float buildingHeight = GetComponent<Renderer>().bounds.size.y;
        Vector3 tilePosition = ((MonoBehaviour)tile).transform.position;  //Given tile is a object



        Vector3 spawnPosition = tilePosition + Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);
        return spawnPosition;
    }

    private Tile TileFindCalculation(){
        (int x, int z) = gridOverlay.GetRowAndColumnsOfPlatform();
        (int, int) buildingSpawnPosition = (z-1, Mathf.FloorToInt(x/2));
        Tile tile = gridOverlay.FindTileWithCoordinates(buildingSpawnPosition.Item2, buildingSpawnPosition.Item1);
        return tile;


    }

    public override void CreateMob(){
        Vector3[] tileCorners = gridOverlay.GetTileCorners(tile);
        float tileHeight = tile.GetTileHeight();

        Vector3 topLeft = tileCorners[0];
        Vector3 topRight = tileCorners[1];
        Vector3 bottomRight = tileCorners[2];
        Vector3 bottomLeft = tileCorners[3];


        Debug.Log("TopLeft value is: " + topLeft); //Test
        Debug.Log("TopRight value is: " + topRight); //Test
        Debug.Log("BottomRight value is: " + bottomRight); //Test
        Debug.Log("BottomLeft value is: " + bottomLeft); //Test

        Vector3 buildingPosition = transform.position;
        float spaceBetweenMobs = Vector3.Distance(topLeft, topRight) / 5; //This is the space between the mobs.


        Vector3 spawnPosition = Vector3.zero;
        float buildingHeight = GetComponent<Renderer>().bounds.size.y;
        float mobHeight = mobPrefab.gameObject.GetComponent<Renderer>().bounds.size.y;


        Vector3 threshold = new Vector3(buildingPosition.x, buildingPosition.y, buildingPosition.z + 0.2f); //This is the threshold for the mob to spawn.

        GameObject lastAssignedMob = GetLastAssignedMob().gameObject; //Get the last mob spawned in the building.
        if(lastAssignedMob != null){
            spawnPosition = lastAssignedMob.transform.position - Vector3.right * spaceBetweenMobs;
            if(Vector3.Distance(bottomRight, bottomLeft) < Vector3.Distance(bottomRight, spawnPosition)){
                spawnPosition = lastAssignedMob.transform.position - Vector3.forward * spaceBetweenMobs;
                spawnPosition.x = bottomRight.x;
                if(Vector3.Distance(buildingPosition, spawnPosition) < Vector3.Distance(buildingPosition, threshold)){
                    return;
                } 
            } 
        } else {
            spawnPosition = bottomRight; //+ Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);  //Potentially correct
            spawnPosition.y = bottomRight.y + (tileHeight + (mobHeight / 2));
        }

        
        
        Vector3 finalSpawnPos = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);    


        GameObject mob = Instantiate(mobPrefab.gameObject, finalSpawnPos, Quaternion.identity);

        if(mob != null){
            Debug.Log("Mob created successfully!");
        }

        mob.GetComponent<DefaultMob>().AssignToBuilding(this);

    }

    

    
}
