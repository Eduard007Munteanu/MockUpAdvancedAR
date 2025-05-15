using System.Collections.Generic;
using UnityEngine;

public class ItemBuilding: MonoBehaviour{


    public static ItemBuilding Instance {get; private set;}

    private Dictionary<string, string> itemBuildingDictionary = new Dictionary<string, string>();

    void Start(){
        InitializeItemBuilding();
    }


    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }


    void InitializeItemBuilding(){
        // itemBuildingDictionary.Add("farming", "GoldItem");
        // itemBuildingDictionary.Add("military", "TreeItem");
        // itemBuildingDictionary.Add("sleep", "StoneItem");
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