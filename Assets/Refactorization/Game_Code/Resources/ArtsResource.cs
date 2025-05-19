// --- ArtsResource.cs ---
using System;
using System.Collections.Generic;
using UnityEngine;

// Concrete implementation for the 'Arts' resource.
public class ArtsResource : Resource
{
    private int artsLevel = 0; // level of arts, used for checking thresholds
    private float artsLevelScaling = 1.2f;

    public event Action<int> OnLevelUp; // Parameter is the current level
    
    // Constructor: Sets up the Arts resource with specific initial values.
    public ArtsResource(
        float initialAmount = 0f, 
        float minAmount = 0f, 
        float maxAmount = 100f, 
        int cycleTicks = 2
        ) : base(ResourceType.Arts, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }
    protected override void onAmountChange(float delta)
    {
        Debug.Log($"ArtsResource: Amount changed by {delta}. Current amount: {CurrentAmount}");
        if (resources != null) // Ensure resources is initialized
        {
            resources[ResourceType.Happiness].AddAmount(delta * 0.001f); // Example: Arts production increases happiness
            resources[ResourceType.Score].AddAmount(delta * 0.05f);
        }
        
        if (CurrentAmount > 600f && !achievementUnlocked){
            // Assuming CubePaintings.Instance is valid and accessible
            if (CubePaintings.Instance != null)
            {
                CubePaintings.Instance.AddPainting(5);
            }
            if (resources != null)
            {
                resources[ResourceType.Score].AddAmount(1000f);
            }
            achievementUnlocked = true;
        }
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onReachedMax(float excess) {
        // set amount to 0, then add excess to start the new level count
        CurrentAmount = 0f;
        AddAmount(excess); // This will also trigger onAmountChange if needed

        MaximumAmount *= artsLevelScaling;
        artsLevel++;
        
        // base.onReachedMax(excess); // Call the base class's OnReachedMax event invocation
        OnLevelUp?.Invoke(artsLevel); // Invoke the specific level up event for Arts

        // Display Notification
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification("Arts Advancement", $"Level {artsLevel} Reached!\n<size=70%>(Draw a card)</size>");
        }
        else
        {
            Debug.LogWarning("NotificationManager instance not found. Cannot show 'Arts Advancement' notification.");
        }

        if (CardsDeck.Instance != null)
        {
            CardsDeck.Instance.AddDraw();
        }
        
        if (resources != null) // Ensure resources is initialized
        {
            resources[ResourceType.Civil_Desire].AddAmount(0.5f); // Example: Arts production increases with civil resource amount
        }
        
        Debug.Log($"ArtsResource: Reached max. Current level: {artsLevel}, New max: {MaximumAmount}");
    }
    protected override void onReachedMin(float deficit) {
        
    }

    protected override void onSpecialAction()
    {
        
    }
}