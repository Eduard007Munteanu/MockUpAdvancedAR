// --- ArtsResource.cs ---
using System;
using System.Collections.Generic;
using UnityEngine;

// Concrete implementation for the 'Arts' resource.
public class ArtsResource : Resource
{
    // private enum ThresholdTypes {
    //     Level1,
    //     Level2,
    //     Level3,
    // }
    
    // Constructor: Sets up the Arts resource with specific initial values.
    public ArtsResource(
        float initialAmount = 0f, 
        float minAmount = 0f, 
        float maxAmount = 10000f, 
        int cycleTicks = 1
        ) : base(ResourceType.Arts, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }

    protected override void onAmountChange()
    {
        // TODO: check art thresholds for level up
    }

    protected override void onProductionChange()
    {

    }

    protected override void onThresholdCrossed()
    {

    }
}