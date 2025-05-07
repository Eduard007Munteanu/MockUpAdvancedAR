using System.Collections.Generic;
using UnityEngine;

public class WoodResource : Resource
{

    public WoodResource(
        float initialAmount = 100f,
        float minAmount = 0f,
        float maxAmount = 1000f, // Wood might have a higher max
        int cycleTicks = 1
        ) : base(ResourceType.Wood, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {

    }

    protected override void onReachedMax(float excess) {

    }

    protected override void onReachedMin(float deficit) {

    }

    protected override void onSpecialAction()
    {
        
    }
}