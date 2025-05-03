using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using Unity.VisualScripting;

public class ItemDatabase: MonoBehaviour{
    public static ItemDatabase Instance;

    // private List<DefaultItem> items; At some point be concerned regarding items on ground

    private Dictionary<string, int> collectedItemsCount = new Dictionary<string, int>();



    //Event trigger => we warn others that the OnCollectedItemsCountDataPacker() method should be called given update made. 
    public event System.Action<DataPacket> OnCollectedItemsUpdated;


    void Awake()
    {
        if (Instance == null)  
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject); 
        }
    }

    public void UpdateCollectedItemsCount(DefaultItem item, int increaseBy){
        string itemClass = item.GetItemClass();
        if(!collectedItemsCount.ContainsKey(itemClass)){
            collectedItemsCount[itemClass] = increaseBy;
        } else if(collectedItemsCount.ContainsKey(itemClass)){
            collectedItemsCount[itemClass] += increaseBy;
        }


        OnCollectedItemsUpdated?.Invoke(GetCollectedItemsCountDataPacket());
    }

    public DataPacket GetCollectedItemsCountDataPacket(){
        DataPacket packet = new DataPacket();

        foreach (var kvp in collectedItemsCount)
        {
            packet.Add(kvp.Key + "Score", kvp.Value);
        }

        return packet;
    }

}