using System.Collections.Generic;
using UnityEngine;

public class HappinessResource : Resource
{

    public HappinessResource(
        float initialAmount = 50f, // Happiness might start at a mid-point
        float minAmount = 0f,
        float maxAmount = 100f, // Happiness often capped at 100
        int cycleTicks = 1
        ) : base(ResourceType.Happiness, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        // if happiness modifies resource production then it will create a loop
        // BUT happiness only modifies production mod so it should be fine
        resources[ResourceType.Civil_Desire].AddAmount(delta * 0.2f); // Example: Happiness production increases with civil resource amount
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