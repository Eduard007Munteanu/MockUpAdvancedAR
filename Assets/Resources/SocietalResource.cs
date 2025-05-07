using System.Collections.Generic;
using UnityEngine;

public class SocietalResource : Resource
{

    public SocietalResource(
        float initialAmount = 50f,
        float minAmount = 0f,
        float maxAmount = 100f,
        int cycleTicks = 1
        ) : base(ResourceType.Societal, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Agreement].TriggerSpecialAction();
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