using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class BuildManager : MonoBehaviour  //One instance only
{

    public static BuildManager Instance {get; private set;}

    [SerializeField] private GameObject  mainBuildingPrefab;

    [SerializeField] private List<DefaultBuild> defaultBuildOptions;

    

    private Dictionary<string, List<DefaultBuild>> buildingDictionary = new Dictionary<string, List<DefaultBuild>>();

    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }


    private IEnumerator WaitAndSpawn()
    {
        // Wait until BetterGridOverlay.Instance is not null
        yield return new WaitUntil(() => BetterGridOverlay.Instance != null);

        // Now it's safe to proceed
        SpawnMainBuilding();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void AddBuildingDictionary(DefaultBuild building)
    {
        string key = building.GetBuildingClass();

        if (!buildingDictionary.ContainsKey(key))
        {
            buildingDictionary[key] = new List<DefaultBuild>();
        }
        
        buildingDictionary[key].Add(building);  ///Invoker to be used. 
    }

    void RemoveBuildingDictionary(DefaultBuild building)
    {
        string key = building.GetBuildingClass();

        if (buildingDictionary.ContainsKey(key))
        {
            buildingDictionary[key].Remove(building);

            if (buildingDictionary[key].Count == 0)
            {
                buildingDictionary.Remove(key); ///Invoker to be used. 
            }
        }
    }

    
    int GetBuildingCount(DefaultBuild building){
        string key = building.GetBuildingClass();
        int count = 0;

        if(buildingDictionary.ContainsKey(key)){
            count = buildingDictionary[key].Count;
        }

        return count;
    }






    void SpawnMainBuilding()
    {
        GameObject buildingObj = Instantiate(mainBuildingPrefab);





        MainBuild mainBuildInstance = buildingObj.GetComponent<MainBuild>();

        if (mainBuildInstance == null)
        {
            Debug.LogError("MainBuild script not found on prefab!");
            return;
        }



        int id = 1;
        mainBuildInstance.Init(id); //Here I will add the gridOverlay

        AddBuildingDictionary(mainBuildInstance);




        DefaultTile tile = mainBuildInstance.GetTile();




        Debug.Log("BIGBUGFIXING, BuildManager, mainbuilding tile is " + tile);

        Debug.Log("BIGBUGFIXING mainbuilding tile position is " + tile.transform.position);

        Renderer theBuildingRenderer = buildingObj.GetComponent<Renderer>();



        float scaleFactor = tile.ScalingTheObjects(theBuildingRenderer, 2);

        buildingObj.transform.localScale *= scaleFactor;



        Vector3 spawnPos = mainBuildInstance.SpawnBuilding();
        buildingObj.transform.position = spawnPos;

        buildingObj.transform.SetParent(tile.transform.parent, true); // Or under a shared AR parent


        Debug.Log("BIGBUGFIXING mainbuilding spawn position is " + spawnPos);


        mainBuildInstance.InitStartingPops();
        tile.ArrangeMobs(null);
    }

    private Vector3 SpawnPosition(DefaultTile tile, GameObject objectToBeSpawned){

        Renderer tileRenderer = tile.GetComponent<Renderer>();
        Renderer objectRenderer = objectToBeSpawned.GetComponent<Renderer>();

        float tileTopY = tileRenderer.bounds.max.y;
        float objectBottomOffset = objectRenderer.bounds.min.y - objectToBeSpawned.transform.position.y;
        float spawnY = tileTopY - objectBottomOffset;


        float tileBackZ = tileRenderer.bounds.min.z;
        float tileFrontZ = tileRenderer.bounds.max.z;

        float objectDepthZ = objectRenderer.bounds.size.z;

        
        
        float spawnZ = tileFrontZ - (objectDepthZ / 2f); 
        
        float spawnX = tileRenderer.bounds.center.x;


        //Vector3 tileCenter = tileRenderer.bounds.center;



        return new Vector3(spawnX, spawnY, spawnZ);
    }

    private void SpawnBuildingOnTile(DefaultTile tile, BuildCard card){
        string cardName = card.GetCardClass();
        Debug.Log("cardName from SpawnBuildingOnTile is " + cardName);
        DefaultBuild actualBuild = null;
        foreach(DefaultBuild defaultBuildOption in defaultBuildOptions){
            string defaultBuildOptionName = defaultBuildOption.GetBuildingClass();
            Debug.Log("defaultBuildOptionname from SpawnBuildingOnTile is " + defaultBuildOptionName);
            if(defaultBuildOptionName == cardName){ 
                Debug.Log("Actualbuild was found. It's type: " + defaultBuildOption.name);
                actualBuild = defaultBuildOption;
            }
        }
        Debug.Log("ActualBuild is " + actualBuild.name);
        //Vector3 spawnPosition = SpawnPosition(tile, actualBuild.gameObject );
        GameObject building = Instantiate(actualBuild.gameObject, Vector3.zero /* spawnPosition */, actualBuild.transform.rotation);//Quaternion.identity);
        

        
    
        Renderer theBuildingRenderer = building.GetComponent<Renderer>();
        float scaleFactor = tile.ScalingTheObjects(theBuildingRenderer, 2);

        building.transform.localScale *= scaleFactor;


        Vector3 spawnPosition = SpawnPosition(tile, building);
        building.transform.position = spawnPosition;

        
        int buildingCount = GetBuildingCount(building.GetComponent<DefaultBuild>());

        Debug.Log("BuildingCount is  " + buildingCount + " for building " + building.name);


        building.GetComponent<DefaultBuild>().Init(buildingCount, tile); //Maybe more, who knows
        AddBuildingDictionary(building.GetComponent<DefaultBuild>());


        tile.ArrangeMobs(null);
    }

    public void TrySpawnBuilding(DefaultTile tile, DefaultCard card) {
        if (card is BuildCard buildCard) {
            SpawnBuildingOnTile(tile, buildCard);
        } else {
            Debug.Log("Card is not a build card, ignoring.");
        }
    }
}
