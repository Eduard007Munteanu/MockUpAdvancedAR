
using System.Collections.Generic;
using UnityEngine;

// individual battle power

public class MightResource : Resource
{
    public MightResource(
        float initialAmount = 1f,
        float minAmount = 1f,
        float maxAmount = 10000f,
        int cycleTicks = 1
        ) : base(ResourceType.Might, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        thresholds = new Thresholds(new List<float> {
            50f
        }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        if (CurrentAmount > 50f)
        {
            CubePaintings.Instance.AddPainting(2);
            resources[ResourceType.Score].AddAmount(1000f);
            achievementUnlocked = true;
        }
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        switch ( i ) {
            case 0:
                if (dir == ThresholdCross.FromDown && !achievementUnlocked)
                {
                    CubePaintings.Instance.AddPainting(4);
                    resources[ResourceType.Score].AddAmount(1000f);
                    achievementUnlocked = true;
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