// using System.Collections;
// using System.Collections.Generic;
// using Meta.XR.Acoustics;
// using UnityEngine;

// public class BuildingManager : MonoBehaviour
// {

    

//     public int column_of_tiles = 5;
//     public int row_of_tiles = 5;

//     public GameObject buildingPrefab;

//     private Dictionary<string, List<Building>> buildingsData = new Dictionary<string, List<Building>>();   //Potentially the building than just the ID.
 

//     public Material mainBuildingMaterial;  //For now is the military, we will change later

//     // Start is called before the first frame update
//     void Start()
//     {
//         InitMainBuilding();
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

    
//     public void RemoveBuildingCountToSpecificClass(Building building)
//     {
//         string className = building.GetBuildingClass(); 
//         if (!buildingsData.ContainsKey(className))
//         {
//             Debug.LogWarning($"Class {className} not found in buildingsData.");
//             return;
//         }
//         buildingsData[className].Remove(building);
//         // if (buildingsData[className].Count == 0)            ! Optional, remove the class if it has no buildings left ! 
//         // {
//         //     buildingsData.Remove(className);
//         // }
//     }

//     public int SetBuildingID(string className){
//         return buildingsData[className].Count;
//     }

//     public void AddBuildingCountToSpecificClass(string className, Building building){
//         if (buildingsData.ContainsKey(className))
//         {
//             buildingsData[className].Add(building);
//         }
//         else
//         {
//             buildingsData[className] = new List<Building> { building };
//         }
//     }

//     public void InitMainBuilding(){
//         GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

//         Debug.Log($"Found {tiles.Length} tile(s):");

//         foreach (GameObject tile in tiles)
//         {
//             Debug.Log("Found this nice tile named: " + tile.name);
//             string[] parts = tile.name.Split('_');
//             if(int.TryParse(parts[1], out int result_1)  && int.TryParse(parts[2], out int result_2)){
//                 if (result_2 == row_of_tiles - 1 && result_1 == Mathf.FloorToInt(column_of_tiles / 2)){
//                     Vector3 tilePosition = tile.transform.position;


//                     float tileHeight = tile.GetComponent<Renderer>().bounds.size.y;
//                     float buildingHeight = buildingPrefab.GetComponent<Renderer>().bounds.size.y;

//                     Vector3 spawnPosition = tilePosition + Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);

//                     Debug.Log("Specific tile height from TileChecker: " + spawnPosition.y); //Test

                    
//                     GameObject newBuilding = Instantiate(buildingPrefab, spawnPosition, Quaternion.identity);
//                     Building building_local = newBuilding.GetComponent<Building>();
//                     if (building_local != null){
//                         building_local.SetMaterial(mainBuildingMaterial);
//                         string mainBuildingClass = "MainBuilding";
//                         int mainBuildingID = 1;
//                         building_local.Initialization(mainBuildingID, mainBuildingClass, tile); 
//                         AddBuildingCountToSpecificClass(mainBuildingClass, building_local);
//                     }

//                     break;
                        
//                 }
//             }
//         }
//     }
// }
