using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    [SerializeField] private GameObject  mainBuildingPrefab;

    private MainBuild mainBuildInstance;

    private Dictionary<string, List<Build>> buildingDictionary;


    // Start is called before the first frame update
    void Start()
    {
        SpawnMainBuilding();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void AddBuildingDictionary(Build building)
    {
        string key = building.GetBuildingClass();

        if (!buildingDictionary.ContainsKey(key))
        {
            buildingDictionary[key] = new List<Build>();
        }
        
        buildingDictionary[key].Add(building);
    }

    void RemoveBuildingDictionary(Build building)
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

        
    }
}
