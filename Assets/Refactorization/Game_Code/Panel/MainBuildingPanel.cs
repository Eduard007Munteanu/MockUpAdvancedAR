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
        int stoneScore = dataPacketFromBuildingManager.Get<int>("StoneScore");
        int woodScore = dataPacketFromBuildingManager.Get<int>("WoodScore");
        int goldScore = dataPacketFromBuildingManager.Get<int>("GoldScore");

        var statsTransform = transform.Find("Layout Stats");

        var stoneText = statsTransform.Find("Text (Stone)")?.GetComponent<TextMeshProUGUI>();
        var woodText = statsTransform.Find("Text (Wood)" )?.GetComponent<TextMeshProUGUI>();
        var goldText = statsTransform.Find("Text (Gold)" )?.GetComponent<TextMeshProUGUI>();

        if (stoneText != null) stoneText.text = stoneScore.ToString();
        if (woodText != null) woodText.text = woodScore.ToString();
        if (goldText != null) goldText.text = goldScore.ToString();
    }



    private string AdditionalText(string currentText, string additionaLText){
        return currentText += " " + additionaLText;
    }
}
