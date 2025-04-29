using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBuildingPanel : DefaultPanel, Panel
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(DefaultBuild building){
        Canvas canvas = GetComponent<Canvas>();
        if(canvas != null){
            canvas.worldCamera = Camera.main;
        }

        var statsTransform = transform.Find("Layout Stats");
        
        var classTransform = statsTransform.Find("Text (Building Class Placeholder)");
        var idTransform    = statsTransform.Find("Text (Building ID Placeholder)");

        TextMeshProUGUI buildingClassText = classTransform?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI buildingIdText    = idTransform?.GetComponent<TextMeshProUGUI>();


        buildingClassText.text = building.GetBuildingClass();

        buildingIdText.text = building.GetID().ToString();


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
}
