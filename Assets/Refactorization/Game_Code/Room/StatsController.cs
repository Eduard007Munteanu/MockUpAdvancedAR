using UnityEngine;
using TMPro;
using Unity.VisualScripting; // Make sure to include the TextMeshPro namespace

public class StatsController : MonoBehaviour // Renamed from ResourceUIDisplayManager
{
    [Header("TextMeshPro References (3D Text)")]
    [Tooltip("Drag the 'ArtsText' GameObject here from the hierarchy.")]
    public TextMeshPro artsText; 

    [Tooltip("Drag the 'PopsText' GameObject here from the hierarchy.")]
    public TextMeshPro popsText; 

    [Tooltip("Drag the 'MightText' GameObject here from the hierarchy.")]
    public TextMeshPro mightText; 

    [Tooltip("Drag the 'FoodText' GameObject here from the hierarchy.")]
    public TextMeshPro foodText; 

    [Tooltip("Drag the 'HappinessText' GameObject here from the hierarchy.")]
    public TextMeshPro HappinessText; 

    [Tooltip("Drag the 'GoldText' GameObject here from the hierarchy.")]
    public TextMeshPro goldText; 

    [Tooltip("Drag the 'ScoreField' GameObject here from the hierarchy.")]
    public TextMeshPro scoreText; 

    private ResourceDatabase resources; // Removed the initialization in Awake
    void Awake()
    {
        // Initialize the ResourceDatabase instance
        while (resources == null)
        {
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }

        // Subscribe to resource changes
        resources[ResourceType.Happiness].OnAmountChanged += UpdateHappinessValue;
        resources[ResourceType.Arts].OnAmountChanged += UpdateArtsValue;
        resources[ResourceType.Wood].OnAmountChanged += UpdatePopsValue;
        resources[ResourceType.Might].OnAmountChanged += UpdateMightValue;
        resources[ResourceType.Food].OnAmountChanged += UpdateFoodValue;
        resources[ResourceType.Gold].OnAmountChanged += UpdateGoldValue;
        resources[ResourceType.Score].OnAmountChanged += UpdateScoreValue; // Added for score updates
    }


    void Start()
    {
        // Optional: Initialize text fields or check if references are set
        if (artsText == null || popsText == null || mightText == null || foodText == null || HappinessText == null || goldText == null)
        {
            Debug.LogError("StatsController: One or more TextMeshPro references are not set in the Inspector! Please ensure all 3D TextMeshPro objects are assigned.", this);
        }
        else
        {
            UpdateArtsValue(ResourceType.Arts, 0); // Initial value
            UpdatePopsValue(ResourceType.Wood, 0); // Initial value
            UpdateMightValue(ResourceType.Might, 0); // Initial value
            UpdateFoodValue(ResourceType.Food, 0); // Initial value
            UpdateHappinessValue(ResourceType.Happiness, 0); // Initial value
            UpdateGoldValue(ResourceType.Gold, 0); // Initial value
            UpdateScoreValue(ResourceType.Score, 0); // Initial value
        }
    }

    // --- Methods to Update Each Text Field ---
    // The logic inside these methods remains the same, as both TextMeshPro
    // and TextMeshProUGUI use '.text' to set their content.

    public void UpdateArtsText(string newText)
    {
        if (artsText != null)
        {
            artsText.text = newText;
        }
    }

    public void UpdateArtsValue(ResourceType type, float value)
    {
        if (artsText != null)
        {
            Debug.Log($"Arts Value Updated: {value}");
            artsText.text = "Arts: " + (int) resources[type].CurrentAmount + "/" + (int) resources[type].MaximumAmount;
        }
    }

    public void UpdatePopsText(string newText)
    {
        if (popsText != null)
        {
            popsText.text = newText;
        }
    }

    public void UpdatePopsValue(ResourceType type, float value)
    {
        if (popsText != null)
        {
            popsText.text = "Wood: " + (int) resources[type].CurrentAmount;
        }
    }

    public void UpdateMightText(string newText)
    {
        if (mightText != null)
        {
            mightText.text = newText;
        }
    }

    public void UpdateMightValue(ResourceType type, float value)
    {
        if (mightText != null)
        {
            mightText.text = "Might: " + (int) resources[type].CurrentAmount;
        }
    }

    public void UpdateFoodText(string newText)
    {
        if (foodText != null)
        {
            foodText.text = newText;
        }
    }

    public void UpdateFoodValue(ResourceType type, float value)
    {
        if (foodText != null)
        {
            foodText.text = "Food: " + (int) resources[type].CurrentAmount + "/" + resources[type].MaximumAmount;
        }
    }

    public void UpdateHappinessText(string newText)
    {
        if (HappinessText != null)
        {
            HappinessText.text = newText;
        }
    }

    public void UpdateHappinessValue(ResourceType type, float value)
    {
        if (HappinessText != null)
        {
            HappinessText.text = "Happiness: " + (int) resources[type].CurrentAmount + "/" + resources[type].MaximumAmount;
        }
    }

    public void UpdateGoldText(string newText)
    {
        if (goldText != null)
        {
            goldText.text = newText;
        }
    }

    public void UpdateGoldValue(ResourceType type, float value)
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + (int) resources[type].CurrentAmount + "/" + resources[type].MaximumAmount;
        }
    }

    public void UpdateScoreValue(ResourceType type, float value)
    {
        if (scoreText != null)
        {
            scoreText.text = ((int) resources[type].CurrentAmount).ToString();
        }
    }
}