using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BuildManager : MonoBehaviour  //One instance only
{

    public static BuildManager Instance {get; private set;}

    [SerializeField] private GameObject  mainBuildingPrefab;

    [SerializeField] private List<DefaultBuild> defaultBuildOptions;

    private MainBuild mainBuildInstance;

    private Dictionary<string, List<DefaultBuild>> buildingDictionary;

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
        
        buildingDictionary[key].Add(building);
    }

    void RemoveBuildingDictionary(DefaultBuild building)
    {
        string key = building.GetBuildingClass();

        if (buildingDictionary.ContainsKey(key))
        {
            buildingDictionary[key].Remove(building);

            if (buildingDictionary[key].Count == 0)
            {
                buildingDictionary.Remove(key); // Clean up empty lists
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






    void SpawnMainBuilding(){
        GameObject buildingObj = Instantiate(mainBuildingPrefab);
        mainBuildInstance = buildingObj.GetComponent<MainBuild>();

        if (mainBuildInstance == null)
        {
            Debug.LogError("MainBuild script not found on prefab!");
            return;
        }

        
        Vector3 spawnPos = mainBuildInstance.SpawnBuilding();

        
        buildingObj.transform.position = spawnPos;

        mainBuildInstance.Init(1, "MainBuild");
        AddBuildingDictionary(mainBuildInstance);

        
    }

    private Vector3 SpawnPosition(DefaultTile tile, GameObject objectToBeSpawned){
        Vector3 tilePosition = tile.gameObject.transform.position;   //Attach to GameObject, then allright. 
        float tileHeight = tile.GetTileHeight();
        float objectToBeSpawnedHeight = objectToBeSpawned.GetComponent<Renderer>().bounds.size.y;

        Vector3 spawnPosition = tilePosition + Vector3.up * ((tileHeight + (objectToBeSpawnedHeight / 2f))  / 1f);

        return spawnPosition;
    }

    private void SpawnBuildingOnTile(DefaultTile tile, BuildCard card){
        string cardName = card.GetCardClass();
        DefaultBuild actualBuild = null;
        foreach(DefaultBuild defaultBuildOption in defaultBuildOptions){
            string defaultBuildOptionName = defaultBuildOption.GetBuildingClass();
            if(defaultBuildOptionName == cardName){ 
                actualBuild = defaultBuildOption;
            }
        }
        Vector3 spawnPosition = SpawnPosition(tile, actualBuild.gameObject );
        GameObject building = Instantiate(actualBuild.gameObject, spawnPosition, Quaternion.identity);
        

        string buildingClassName = card.GetCardClass(); 



        AddBuildingDictionary(building.GetComponent<DefaultBuild>());
        int buildingCount = GetBuildingCount(building.GetComponent<DefaultBuild>());
        building.GetComponent<DefaultBuild>().Init(buildingCount, buildingClassName, tile); //Maybe more, who knows
    }

    public void TrySpawnBuilding(DefaultTile tile, DefaultCard card) {
        if (card is BuildCard buildCard) {
            SpawnBuildingOnTile(tile, buildCard);
        } else {
            Debug.Log("Card is not a build card, ignoring.");
        }
    }
}
