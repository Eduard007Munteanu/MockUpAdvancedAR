using System.Collections.Generic;
using UnityEngine;

public class SocietalResource : Resource
{

    public SocietalResource(
        float initialAmount = 50f,
        float minAmount = 0f,
        float maxAmount = 100f,
        int cycleTicks = 1
        ) : base(ResourceType.Societal, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Agreement].TriggerSpecialAction();
        resources[ResourceType.Arts].AddProductionModifier(delta * 0.01f); // Example: Arts production increases with civil resource amount
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