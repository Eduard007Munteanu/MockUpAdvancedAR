using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBuildingPanel : DefaultPanel
{
    // Start is called before the first frame update
    void Start()
    {
        
        if (ItemDatabase.Instance != null)   //POTENTIAL ERRORS? WHO KNOWS. EASTER EGG
        {
            ItemDatabase.Instance.OnCollectedItemsUpdated += UpdatePanel;
        }
        else
        {
            Debug.LogWarning("ItemDatabase instance was not ready during panel Init.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init(DefaultBuild building){
        Canvas canvas = GetComponent<Canvas>();
        if(canvas != null){
            canvas.worldCamera = Camera.main;
        }

        var statsTransform = transform.Find("Layout_Stats");
        
        var classTransform = statsTransform.Find("Text (Building Class Placeholder)");
        var idTransform    = statsTransform.Find("Text (Building ID Placeholder)");

        TextMeshProUGUI buildingClassText = classTransform?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI buildingIdText    = idTransform?.GetComponent<TextMeshProUGUI>();


        string initClassText = "Building Type:";
        string initIdText = "Building ID:";

        

        buildingClassText.text = AdditionalText(initClassText, building.GetBuildingClass() );
        buildingIdText.text = AdditionalText(initIdText, building.GetID().ToString());

        // ---- Link the spawn mob button ----
        var modsTransform = transform.Find("Modifications");
        var spawnButtonTransform = modsTransform?.Find("Button (Spawn Mobs)");

        if (spawnButtonTransform != null && building is MainBuild mainBuild)
        {
            var mobSpawnButton = spawnButtonTransform.GetComponent<MobSpawnButton>();
            if (mobSpawnButton != null)
            {
                mobSpawnButton.LinkBuilding(mainBuild);
            }
        }

    }




    



    public override void UpdatePanel(DataPacket dataPacketFromBuildingManager){     
        //int stoneScore = dataPacketFromBuildingManager.Get<int>("StoneItemScore");
        //int woodScore = dataPacketFromBuildingManager.Get<int>("TreeItemScore");
        //int goldScore = dataPacketFromBuildingManager.Get<int>("GoldItemScore");

        int stoneScore = TryGetAlikeMethod(dataPacketFromBuildingManager, "StoneItemScore");
        int woodScore = TryGetAlikeMethod(dataPacketFromBuildingManager, "TreeItemScore");
        int goldScore = TryGetAlikeMethod(dataPacketFromBuildingManager, "GoldItemScore");


        var statsTransform = transform.Find("Layout_Stats");

        var stoneText = statsTransform.Find("Text (Stone)")?.GetComponent<TextMeshProUGUI>();
        var woodText = statsTransform.Find("Text (Wood)" )?.GetComponent<TextMeshProUGUI>();
        var goldText = statsTransform.Find("Text (Gold)" )?.GetComponent<TextMeshProUGUI>();


        string initGoldScoreText = "Gold: ";
        string initWoodScoreText = "Wood: ";
        string initStoneScoreText = "Stone: ";

        if (stoneText != null && stoneScore != -1) stoneText.text = initStoneScoreText +  stoneScore.ToString();
        if (woodText != null && woodScore != -1) woodText.text = initWoodScoreText + woodScore.ToString();
        if (goldText != null && goldScore != -1) goldText.text = initGoldScoreText + goldScore.ToString();
    }



    private string AdditionalText(string currentText, string additionaLText){
        return currentText += " " + additionaLText;
    }


    private int TryGetAlikeMethod(DataPacket packet, string key){
        if(packet.TryGet<int>(key, out int stone)){
            return stone;
        } else{
            return -1;
        }
    }
}
