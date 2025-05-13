// --- FoodResource.cs ---
using System.Collections.Generic;
using UnityEngine;

// Concrete implementation for the 'Food' resource.
public class FoodResource : Resource
{
    private enum FoodThresholds
    {
        Starving,
        Achievement,
        // Add more thresholds as needed
    }

    public FoodResource(
        float initialAmount = 500f,
        float minAmount = 0f,
        float maxAmount = 1000f,
        int cycleTicks = 5
        ) : base(ResourceType.Food, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Score].AddAmount(delta * 0.05f);

        if (CurrentAmount > 1000f && !achievementUnlocked)
        {
            CubePaintings.Instance.AddPainting(0);
            resources[ResourceType.Score].AddAmount(1000f);
            achievementUnlocked = true;
        }
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onReachedMax(float excess) {
        resources[ResourceType.Economy].AddAmount(excess * 0.1f);
    }

    protected override void onReachedMin(float deficit) {
        resources[ResourceType.Happiness].AddAmount(deficit * - 0.1f);
        resources[ResourceType.Economy].AddAmount(deficit * - 0.1f);
    }

    protected override void onSpecialAction()
    {
    
    }
}