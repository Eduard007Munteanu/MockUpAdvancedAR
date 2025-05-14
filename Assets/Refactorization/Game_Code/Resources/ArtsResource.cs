// --- ArtsResource.cs ---
using System;
using System.Collections.Generic;
using UnityEngine;

// Concrete implementation for the 'Arts' resource.
public class ArtsResource : Resource
{
    private int artsLevel = 0; // level of arts, used for checking thresholds
    private float artsLevelScaling = 1.2f;

    public event Action<int> OnLevelUp; // Parameter is the deficit amount
    
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
        resources[ResourceType.Happiness].AddAmount(delta * 0.001f); // Example: Arts production increases happiness
        resources[ResourceType.Score].AddAmount(delta * 0.05f);
        
        if (CurrentAmount > 1000f && !achievementUnlocked){
            CubePaintings.Instance.AddPainting(5);
            resources[ResourceType.Score].AddAmount(1000f);
            achievementUnlocked = true;
        }
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onReachedMax(float excess) {
        // set amount to 0
        // up the max value
        CurrentAmount = excess;
        MaximumAmount *= artsLevelScaling;
        artsLevel++;
        // onReachedMax already invoked with excess
        // still invoke with current level through an other action
        OnLevelUp?.Invoke(artsLevel);
        CardsDeck.Instance.AddDraw();

        // add to desire of freedom
        resources[ResourceType.Civil_Desire].AddAmount(0.5f); // Example: Arts production increases with civil resource amount
        
        Debug.Log($"ArtsResource: Reached max. Current level: {artsLevel}, New max: {MaximumAmount}");
    }
    protected override void onReachedMin(float deficit) {
        
    }

    protected override void onSpecialAction()
    {
        
    }
}