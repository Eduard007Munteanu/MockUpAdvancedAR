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
        ) : base(ResourceType.Wood, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Score].AddAmount(delta * 0.05f);
        if (CurrentAmount > 1000f && !achievementUnlocked)
        {
            CubePaintings.Instance.AddPainting(5);
            resources[ResourceType.Score].AddAmount(1000f);
            achievementUnlocked = true;
        }
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onReachedMax(float excess) {

    }

    protected override void onReachedMin(float deficit) {
        resources[ResourceType.Happiness].AddAmount(deficit * - 0.1f);
        resources[ResourceType.Economy].AddAmount(deficit * - 0.1f);
    }

    protected override void onSpecialAction()
    {
        
    }
}