using System.Collections.Generic;
using UnityEngine;

public class CivilDesireResource : Resource
{

    public CivilDesireResource(
        float initialAmount = 50,
        float minAmount = 0f,
        float maxAmount = 100f,
        int cycleTicks = 1
        ) : base(ResourceType.Civil_Desire, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
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