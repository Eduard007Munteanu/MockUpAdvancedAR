// --- FoodResource.cs ---
using System.Collections.Generic;
using UnityEngine;

// Concrete implementation for the 'Food' resource.
public class FoodResource : Resource
{

    public FoodResource(
        float initialAmount = 100f,
        float minAmount = 0f,
        float maxAmount = 1000f,
        int cycleTicks = 1
        ) : base(ResourceType.Arts, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // add thresholds here, follow FoodThresholds order, keep list in ascending order
        // thresholds = new Thresholds(new List<float> {
        //     0f
        // }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {

    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        // starving handled in reachedmin
    }

    protected override void onReachedMax(float excess) {

    }

    protected override void onReachedMin(float deficit) {

    }

    protected override void onSpecialAction()
    {
        
    }
}