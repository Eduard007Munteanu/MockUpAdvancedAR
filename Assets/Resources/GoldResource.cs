using System.Collections.Generic;
using UnityEngine;

public class GoldResource : Resource
{

    private enum GoldThresholds
    {
        Low,
    }

    public GoldResource(
        float initialAmount = 500f,
        float minAmount = 0f,
        float maxAmount = 1000f, // Gold might have a higher max
        int cycleTicks = 1
        ) : base(ResourceType.Gold, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
        
        thresholds = new Thresholds(new List<float> {
            0.00001f
        }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Score].AddAmount(delta * 0.05f);
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        // starving handled in reachedmin
        switch ((GoldThresholds) i ) {

            case GoldThresholds.Low:
                if (dir == ThresholdCross.FromUp)
                {
                    // Handle starving condition, take away from happiness
                    resources[ResourceType.Economy].AddAmount(8f);
                }
                else
                {
                    // Handle recovery from starving condition, so add back some happiness
                    resources[ResourceType.Economy].AddAmount(-10f);
                }
                break;
            default:
                break;
        }
    }

    protected override void onReachedMax(float excess) {

    }

    protected override void onReachedMin(float deficit) {

    }

    protected override void onSpecialAction()
    {
        
    }
}