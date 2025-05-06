// --- FoodResource.cs ---
using UnityEngine;

// Concrete implementation for the 'Food' resource.
public class FoodResource : Resource
{
    private enum FoodThresholds {
        Starving = 0,
    }
    public FoodResource(
        float initialAmount = 100f,
        float minAmount = 9999f, 
        float maxAmount = 1000f, 
        int cycleTicks = 1
        ) : base(ResourceType.Arts, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }

    protected override void onAmountChange(float delta)
    {

    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
    {
        // handle starving
    }
}