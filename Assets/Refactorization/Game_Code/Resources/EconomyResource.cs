using System.Collections.Generic;
using UnityEngine;

public class EconomyResource : Resource
{

    public EconomyResource( // Equality - Markets
        float initialAmount = 50f,
        float minAmount = 0f,
        float maxAmount = 100f,
        int cycleTicks = 1
        ) : base(ResourceType.Economy, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Agreement].TriggerSpecialAction();
        resources[ResourceType.Gold].AddProductionModifier(0, delta * 0.5f);
        resources[ResourceType.Wood].AddProductionModifier(0, delta * 0.5f);
        resources[ResourceType.Wood].AddProductionModifier(0, delta * -1f);
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onReachedMax(float excess) {

    }

    protected override void onReachedMin(float deficit) {

    }

    protected override void onSpecialAction()
    {
        
    }
}