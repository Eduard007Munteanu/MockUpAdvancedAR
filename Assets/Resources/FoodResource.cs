// --- FoodResource.cs ---
using UnityEngine;

// Concrete implementation for the 'Food' resource.
public class FoodResource : Resource
{
    public FoodResource(
        float initialAmount = 100f,
        float minAmount = 9999f, 
        float maxAmount = 1000f, 
        int cycleTicks = 1
        ) : base(ResourceType.Arts, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }

    protected override void onAmountChange()
    {
        // TODO: check art thresholds for level up
    }

    protected override void onProductionChange()
    {

    }

    protected override void onThresholdCrossed()
    {
        // handle starving
    }
}