using System.Collections.Generic;
using UnityEngine;

public class WoodResource : Resource
{

    private enum WoodThresholds
    {
        Low,
        Achievement
    }

    public WoodResource(
        float initialAmount = 100f,
        float minAmount = 0f,
        float maxAmount = 1000f, // Wood might have a higher max
        int cycleTicks = 1
        ) : base(ResourceType.Wood, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
        thresholds = new Thresholds(new List<float> {
            0.00001f,
            50f
        }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Score].AddAmount(delta * 0.05f);
        if (CurrentAmount > 1000f)
        {
            CubePaintings.Instance.AddPainting(5);
            resources[ResourceType.Score].AddAmount(1000f);
            achievementUnlocked = true;
        }
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        // starving handled in reachedmin
        switch ((WoodThresholds) i ) {

            case WoodThresholds.Low:
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
            case WoodThresholds.Achievement:
                if (dir == ThresholdCross.FromDown && !achievementUnlocked)
                {
                    CubePaintings.Instance.AddPainting(3);
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