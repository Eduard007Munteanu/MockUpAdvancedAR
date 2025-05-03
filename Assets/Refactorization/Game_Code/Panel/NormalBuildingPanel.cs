using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NormalBuildingPanel : DefaultPanel
{
    // Start is called before the first frame update

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

        

        buildingClassText.text = AdditionalText(buildingClassText.text, building.GetBuildingClass() );
        buildingIdText.text = AdditionalText(buildingIdText.text, building.GetID().ToString());




    }


    private string AdditionalText(string currentText, string additionaLText){
        return currentText += " " + additionaLText;
    }
    
    
}
