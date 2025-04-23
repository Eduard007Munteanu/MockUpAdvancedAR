using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasContentManager : MonoBehaviour
{
    // Start is called before the first frame update

    private int stoneScore = 0;
    private int treeScore = 0;
    private int goldScore = 0;

    private TextMeshProUGUI goldText;

    private TextMeshProUGUI stoneText;

    private TextMeshProUGUI woodText;
    void Start()
    {
        Transform layoutStats = transform.Find("Layout_Stats");




        if (layoutStats != null)
        {
            goldText = layoutStats.Find("Text (Gold)").GetComponent<TextMeshProUGUI>();
            if(goldText == null) Debug.LogError("Gold Text is null");
            stoneText = layoutStats.Find("Text (Stone)").GetComponent<TextMeshProUGUI>();
            if(stoneText == null) Debug.LogError("Stone Text is null");
            woodText = layoutStats.Find("Text (Wood)").GetComponent<TextMeshProUGUI>();
            if(woodText == null) Debug.LogError("Wood Text is null");
        }
    }

    void Initialization(string buildingClass, int ID){
        
    }

    // Update is called once per frame
    void Update()
    {
        CanvasScoreVisually();
    }



    public void UpdateScore(int score, string type){
        if(type.Equals("stone")){
            stoneScore += score; 
        }
        else if(type.Equals("wood")){
            treeScore += score; 
        }
        else if(type.Equals("gold")){
            goldScore += score; 
        }
        
    }

    public void CanvasScoreVisually(){
        if (stoneText != null) stoneText.text = $"Stone: {stoneScore}";
        if (woodText != null) woodText.text = $"Wood: {treeScore}";
        if (goldText != null) goldText.text = $"Gold: {goldScore}";
    }
}
