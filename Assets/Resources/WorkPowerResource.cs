using System.Collections.Generic;
using UnityEngine;

public class WorkPowerResource : Resource
{

    public WorkPowerResource(
        float initialAmount = 1f,
        float minAmount = 0f,
        float maxAmount = 1000f,
        int cycleTicks = 1
        ) : base(ResourceType.WorkPower, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
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
}