// --- FoodResource.cs ---
using System.Collections.Generic;
using UnityEngine;

// Concrete implementation for the 'Food' resource.
public class FoodResource : Resource
{
    private enum FoodThresholds
    {
        Starving,
        // Add more thresholds as needed
    }

    public FoodResource(
        float initialAmount = 500f,
        float minAmount = 0f,
        float maxAmount = 1000f,
        int cycleTicks = 5
        ) : base(ResourceType.Food, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // add thresholds here, follow FoodThresholds order, keep list in ascending order
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
        switch ((FoodThresholds) i ) {

            case FoodThresholds.Starving:
                if (dir == ThresholdCross.FromUp)
                {
                    // Handle starving condition, take away from happiness
                    resources[ResourceType.Happiness].AddAmount(-10f);
                    resources[ResourceType.Economy].AddAmount(8f);
                }
                else
                {
                    // Handle recovery from starving condition, so add back some happiness
                    resources[ResourceType.Happiness].AddAmount(8f);
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