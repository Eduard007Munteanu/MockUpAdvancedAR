using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Meta.XR.MRUtilityKit.SceneDecorator;
using Oculus.Interaction.Samples;
using Unity.VisualScripting;
using UnityEngine;

public class MainBuild : DefaultBuild
{
    private BetterGridOverlay gridOverlay;

    [SerializeField] private DefaultMob mobPrefab;

    protected override string Building_class => "Main";

    PopulationResource populationResource;

    //protected override DefaultBuildingEffect BuildingEffect => throw new NotImplementedException();

    private DefaultTile tiles;

    private List<DefaultMob> buffer = new List<DefaultMob>();

    private int initialPops = 5;

    private bool isIniting ;

    private float initialY;

    // Start is called before the first frame update
    void Start()
    {
        
        //Debug.Log("GridOverlay is ", gridOverlay);
    }

    // Update is called once per frame
    void Update()
    {
        if (buffer.Count > 0 && tiles.CanMobBeArrangedChecker())
        {
            DefaultMob theMob = buffer[0];
            buffer.RemoveAt(0);
            tiles.ArrangeMobs(theMob);
            theMob.AssignToBuilding(this);
        }
    }

    public override void Init(int Id, DefaultTile tile = null){
        // this.Id = Id;
        // this.building_class_main = main_class;
        gridOverlay = BetterGridOverlay.Instance;
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


    public override Vector3 SpawnBuilding()
    {
        tiles = TileFindCalculation();

        Debug.Log("BIGBUGFIXING, From SpawnBuilding MainBuild, tiles is " + tiles);

        Renderer tileRenderer = tiles.GetComponent<Renderer>();
        Renderer buildingRenderer = GetComponent<Renderer>();

        float tileTopY = tileRenderer.bounds.max.y;

        
        float objectBottomOffset = buildingRenderer.bounds.min.y - transform.position.y;

        
        float spawnY = tileTopY - objectBottomOffset;

        
        // Vector3[] corners = BetterGridOverlay.Instance.GetTileCorners(tiles);
        // Vector3 topLeft = corners[0];
        // Vector3 topRight = corners[1];

        
        // Vector3 halfBellow =  topLeft - ((topLeft - topRight) / 2);//(topRight - topLeft) / 2;

        // float buildingbackZ = buildingRenderer.bounds.min.z;
        // float buildingfrontZ = buildingRenderer.bounds.max.z;
        // float desiredBackZ = halfBellow.z;

        // float deltaZ = desiredBackZ + ((buildingbackZ - buildingfrontZ) / 2);

         

        // float actualZ =  deltaZ;

        
        float tileBackZ = tileRenderer.bounds.min.z;
        float tileFrontZ = tileRenderer.bounds.max.z;

        float objectDepthZ = buildingRenderer.bounds.size.z;

        
        float halfTileZ = (tileFrontZ - tileBackZ) / 2f;
        float spawnZ = tileFrontZ - (objectDepthZ / 2f); 
        
        float spawnX = tileRenderer.bounds.center.x;


        //Vector3 tileCenter = tileRenderer.bounds.center;



        return new Vector3(spawnX, spawnY, spawnZ);
    }


    public void InitStartingPops()
    {
        isIniting = true;
        populationResource.InitialPops(initialPops);
        isIniting = false;
    }

    private DefaultTile TileFindCalculation(){
        (int x, int z) = gridOverlay.GetRowAndColumnsOfPlatform();
        (int, int) buildingSpawnPosition = (x-1, Mathf.FloorToInt(z/2));
        DefaultTile tile = gridOverlay.FindTileWithCoordinates(buildingSpawnPosition.Item1, buildingSpawnPosition.Item2);
        return tile;


    }

    public override void CreateMob(bool initial = false){

        if(tiles.CanMobBeArrangedChecker()){
            Vector3[] tileCorners = gridOverlay.GetTileCorners(tiles);
            Vector3 bottomRight = tileCorners[2];
            
            Vector3 spawnPosition = bottomRight;
            Renderer bottomRightTileRenderer = tiles.GetComponent<Renderer>();
            Renderer mobRenderer = mobPrefab.GetComponent<Renderer>();

            float tileTopY = bottomRightTileRenderer.bounds.max.y;

            float objectBottomOffset = mobRenderer.bounds.min.y - mobPrefab.transform.position.y;

            float spawnY = tileTopY - objectBottomOffset;
            
            spawnPosition.y = spawnY;

            if (isIniting) spawnPosition += new Vector3(0f, 0.011f, 0f);

            Vector3 finalSpawnPos = new Vector3(0f, spawnPosition.y, 0f);

            // GameObject mob;
            // if (buffer.Count > 0){
            //     mob = buffer[0].gameObject;
            //     buffer.Remove(mob.GetComponent<DefaultMob>());

            // } else{
            //     mob = Instantiate(mobPrefab.gameObject, finalSpawnPos, Quaternion.identity);
            // }
            GameObject mob = Instantiate(mobPrefab.gameObject, finalSpawnPos, mobPrefab.transform.rotation);//Quaternion.identity);

            mob.GetComponent<DefaultMob>().DidSetY(mob.GetComponent<DefaultMob>().transform.position.y);


            mob.transform.position = finalSpawnPos;
            mob.transform.SetParent(tiles.transform.parent, true); // This was missing before

            
            Renderer theMobRenderer = mob.GetComponent<Renderer>();
            float scaleFactor = tiles.ScalingTheObjects(theMobRenderer, 5);

            mob.transform.localScale *= scaleFactor;

            Debug.Log("Tiles name is " + tiles);


            tiles.ArrangeMobs(mob.GetComponent<DefaultMob>());

            Debug.Log($"a miracle Mob is: {mob}");

            if(mob != null){
                Debug.Log("a miracle happened Mob created successfully!");
                
            } else {
                Debug.Log("a miracle didnt happen because mob is null");
            }

            mob.GetComponent<DefaultMob>().AssignToBuilding(this);
        } else {
            //Eduard logic.


            GameObject mob = Instantiate(mobPrefab.gameObject, Vector3.zero, mobPrefab.transform.rotation);//Quaternion.identity);

            Renderer mobRenderer = mob.GetComponent<Renderer>();

            float scaleFactor = tiles.ScalingTheObjects(mobRenderer, 5);

            mob.transform.localScale *= scaleFactor;

            Debug.Log("Tiles name is " + tiles);



            


            Renderer bottomRightTileRenderer = tiles.GetComponent<Renderer>();
            float tileTopY = bottomRightTileRenderer.bounds.max.y;

            float objectBottomOffset = mobRenderer.bounds.min.y - mob.transform.position.y;
            float spawnY = tileTopY - objectBottomOffset;
            

            
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = spawnY;
            // spawnPosition += new Vector3(0f, 0.01f, 0f);        // Hardcoded! 

            mob.transform.position = spawnPosition;

            mob.transform.SetParent(tiles.transform.parent, true); // Keep world position, but parent it under same AR anchor




            buffer.Add(mob.GetComponent<DefaultMob>());

            

            

            




            //GameObject mob = Instantiate(mobPrefab.gameObject, finalSpawnPos, Quaternion.identity);

            //transform.position

        }

        

        






        
    }

    // Listen to population changes and spawn mobs
    private void spawnNewBorn(float delta) {
        Debug.Log("a miracle could happen");
        this.CreateMob();
    }
}
