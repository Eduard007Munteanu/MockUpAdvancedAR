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

    PopulationResource populationResource;

    //protected override DefaultBuildingEffect BuildingEffect => throw new NotImplementedException();

    private DefaultTile tiles;

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
        gridOverlay = GridOverlay.Instance;
        DefaultTile tileToUse = TileFindCalculation();                                  //NOT THAT BEAUTIFULL, BUT SHOULD WORK. 
        base.Init(Id, tileToUse);
        
        // subscribe to population changes
        populationResource = (PopulationResource)resources[ResourceType.Population];
        populationResource.OnBirth += spawnNewBorn;

        resourceEffects = new List<ResourceEffect>
        {
            new ResourceEffect(ResourceType.Arts, 0f, 1f),
        };

    }

    public override Vector3 SpawnBuilding(){
        tiles = TileFindCalculation();
        float tileHeight = tiles.GetTileHeight();
        float buildingHeight = GetComponent<Renderer>().bounds.size.y;
        Vector3 tilePosition = tiles.transform.position;  //Given tile is a object

        Vector3 spawnPosition = tilePosition + Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);
        return spawnPosition;
    }

    public void InitStartingPops(){
        populationResource.InitialPops();
    }

    private DefaultTile TileFindCalculation(){
        (int x, int z) = gridOverlay.GetRowAndColumnsOfPlatform();
        (int, int) buildingSpawnPosition = (z-1, Mathf.FloorToInt(x/2));
        DefaultTile tile = gridOverlay.FindTileWithCoordinates(buildingSpawnPosition.Item2, buildingSpawnPosition.Item1);
        return tile;


    }

    public override void CreateMob(){

        Vector3[] tileCorners = gridOverlay.GetTileCorners(tiles);
        float tileHeight = tiles.GetTileHeight();

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

        DefaultMob lastAssignedMobComponent = GetLastAssignedMob(); //Get the last mob spawned in the building.

        if(lastAssignedMobComponent != null){   

            GameObject lastAssignedMob = lastAssignedMobComponent.gameObject;

            spawnPosition = lastAssignedMob.transform.position - Vector3.right * spaceBetweenMobs;
            if(Vector3.Distance(bottomRight, bottomLeft) < Vector3.Distance(bottomRight, spawnPosition)){
                spawnPosition = lastAssignedMob.transform.position - Vector3.forward * spaceBetweenMobs;
                spawnPosition.x = bottomRight.x;
                if(Vector3.Distance(buildingPosition, spawnPosition) < Vector3.Distance(buildingPosition, threshold)){
                    // return; // wtf
                } 
            } 
        } else {
            spawnPosition = bottomRight; //+ Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);  //Potentially correct
            spawnPosition.y = bottomRight.y + (tileHeight + (mobHeight / 2));
        }

        
        
        Vector3 finalSpawnPos = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);    


        GameObject mob = Instantiate(mobPrefab.gameObject, finalSpawnPos, mobPrefab.transform.rotation);//Quaternion.identity);

        Debug.Log($"a miracle Mob is: {mob}");

        if(mob != null){
            Debug.Log("a miracle happened Mob created successfully!");
            
        } else {
            Debug.Log("a miracle didnt happen because mob is null");
        }

        mob.GetComponent<DefaultMob>().AssignToBuilding(this);
    }

    // Listen to population changes and spawn mobs
    private void spawnNewBorn(float delta) {
        Debug.Log("a miracle could happen");
        this.CreateMob();
    }
}
