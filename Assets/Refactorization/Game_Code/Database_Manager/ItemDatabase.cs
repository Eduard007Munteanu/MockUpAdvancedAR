using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using Unity.VisualScripting;

public class ItemDatabase: MonoBehaviour{
    public static ItemDatabase Instance {get; private set;}

    // private List<DefaultItem> items; At some point be concerned regarding items on ground

    private Dictionary<string, int> collectedItemsCount = new Dictionary<string, int>();

    private ResourceDatabase resources; // I don't want now to add get + set methods

    //Event trigger => we warn others that the OnCollectedItemsCountDataPacker() method should be called given update made. 
    public event System.Action<DataPacket> OnCollectedItemsUpdated;


    void Awake()
    {
        Debug.Log("ItemDatabase Awake called!");

        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject); 
        }

        while (resources == null){
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }
    }

    public void UpdateCollectedItemsCount(DefaultItem item, int increaseBy){

        resources[item.Type].AddAmount(increaseBy);

        string itemClass = item.GetItemClass();
        if(!collectedItemsCount.ContainsKey(itemClass)){
            collectedItemsCount[itemClass] = increaseBy;
        } else if(collectedItemsCount.ContainsKey(itemClass)){
            collectedItemsCount[itemClass] += increaseBy;
        }

        if (item.GetItemClass() == "StoneItem"){
            resources[ResourceType.Food].AddAmount(increaseBy);
        } else if (item.GetItemClass() == "GoldItem"){
            resources[ResourceType.Gold].AddAmount(increaseBy);
        } else if (item.GetItemClass() == "TreeItem") {
            resources[ResourceType.Wood].AddAmount(increaseBy);
        }

        OnCollectedItemsUpdated?.Invoke(GetCollectedItemsCountDataPacket());
    }

    public DataPacket GetCollectedItemsCountDataPacket(){
        DataPacket packet = new DataPacket();

        foreach (var kvp in collectedItemsCount)
        {
            packet.Add(kvp.Key + "Score", kvp.Value);
        }

    // // Test, I want to check the packet:
    // string packetContents = "Packet Contents: { ";
    // foreach (var item in packet.GetType().GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(packet) as Dictionary<string, object>)
    // {
    //     packetContents += $"[{item.Key}: {item.Value}], ";
    // }
    // packetContents += "}";
    // Debug.Log(packetContents);
    // // Test, I want to check the packet:

        return packet;
    }

}