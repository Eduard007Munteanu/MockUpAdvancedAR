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
    protected override void onAmountChange(float delta)
    {
        // TODO: check art thresholds for level up
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
    }
    protected override void onReachedMin(float deficit) {

    }
}