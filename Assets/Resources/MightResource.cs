
using System.Collections.Generic;
using UnityEngine;

// individual battle power

public class MightResource : Resource
{
    private enum MightThresholds
    {
        Achievement,
    }
    public MightResource(
        float initialAmount = 1f,
        float minAmount = 1f,
        float maxAmount = 10000f,
        int cycleTicks = 1
        ) : base(ResourceType.Might, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        thresholds = new Thresholds(new List<float> {
            1000f
        }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {

    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        switch ((MightThresholds)i)
        {
            case MightThresholds.Achievement:
                if (dir == ThresholdCross.FromDown)
                {
                    // TODO: Handle achievement condition
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