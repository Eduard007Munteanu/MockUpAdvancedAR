using TMPro;
using UnityEngine;

public class MainBuildingCanvas : MonoBehaviour
{
    private int stoneScore = 0;
    private int woodScore  = 0;
    private int goldScore  = 0;

    private TextMeshProUGUI stoneText;
    private TextMeshProUGUI woodText;
    private TextMeshProUGUI goldText;

    void Start()
    {
        var layout = transform.Find("Layout_Stats");
        if (layout == null)
        {
            Debug.LogError("Layout_Stats missing!");
            return;
        }

        stoneText = layout.Find("Text (Stone)")?.GetComponent<TextMeshProUGUI>();
        woodText  = layout.Find("Text (Wood)" )?.GetComponent<TextMeshProUGUI>();
        goldText  = layout.Find("Text (Gold)" )?.GetComponent<TextMeshProUGUI>();

        if (stoneText == null) Debug.LogError("Stone text null");
        if (woodText  == null) Debug.LogError("Tree text null");
        if (goldText  == null) Debug.LogError("Gold text null");
    }

    void Update()
    {
        // Poll every mob for new deliveries
        foreach (var mob in FindObjectsOfType<Mob>())
        {
            Debug.Log(mob.name + " is in the scene");
            if (mob.TryConsumeDelivery(out string matType)){
                Debug.Log("Increment by 1");
                UpdateScore(1, matType);
            }
            
                
        }

        // redraw
        CanvasScoreVisually();
    }

    private void UpdateScore(int amount, string type)
    {
        switch (type)
        {
            case "Stone": stoneScore += amount; break;
            case "Tree":  woodScore  += amount; break;
            case "Gold":  goldScore  += amount; break;
        }
    }

    private void CanvasScoreVisually()
    {
        if (stoneText != null) stoneText.text = $"Stone: {stoneScore}";
        if (woodText  != null) woodText .text = $"Tree:  {woodScore}";
        if (goldText  != null) goldText .text = $"Gold:  {goldScore}";
    }
}
