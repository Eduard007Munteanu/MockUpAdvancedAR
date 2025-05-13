using System.Collections.Generic;
using UnityEngine;

public class GoldResource : Resource
{

    private enum GoldThresholds
    {
        Low,
        Achievement,
    }

    public GoldResource(
        float initialAmount = 500f,
        float minAmount = 0f,
        float maxAmount = 1000f, // Gold might have a higher max
        int cycleTicks = 1
        ) : base(ResourceType.Gold, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Score].AddAmount(delta * 0.05f);
        if (CurrentAmount > 1000f)
        {
            CubePaintings.Instance.AddPainting(1);
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