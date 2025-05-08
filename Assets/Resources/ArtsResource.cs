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
        int cycleTicks = 1
        ) : base(ResourceType.Arts, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // randomProduction = true;
    }
    protected override void onAmountChange(float delta)
    {
        Debug.Log($"ArtsResource: Amount changed by {delta}. Current amount: {CurrentAmount}");
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        // no functionality. art progression is done through checking against min max
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

        // add to desire of freedom
        resources[ResourceType.Civil_Desire].AddAmount(0.5f); // Example: Arts production increases with civil resource amount
    }
    protected override void onReachedMin(float deficit) {

    }

    protected override void onSpecialAction()
    {
        
    }
}