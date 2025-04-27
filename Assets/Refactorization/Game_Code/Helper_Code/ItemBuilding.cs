using System.Collections.Generic;
using UnityEngine;

public class ItemBuilding: MonoBehaviour{

    private Dictionary<string, string> itemBuildingDictionary = new Dictionary<string, string>();

    void Start(){
        InitializeItemBuilding();
    }


    void InitializeItemBuilding(){
        itemBuildingDictionary.Add("Farming_house", "Gold");
        itemBuildingDictionary.Add("Military_house", "Tree");
        itemBuildingDictionary.Add("Sleep_house", "Stone");
        
    }

    public string GetItemName(string buildingName){
        if(itemBuildingDictionary.ContainsKey(buildingName)){
            return itemBuildingDictionary[buildingName];
        }else{
            Debug.LogWarning("Building name not found in dictionary: " + buildingName);
            return null; 
        }
    }




}