using System.Collections.Generic;
using UnityEngine;

public class SocietalDesireResource : Resource
{

    public SocietalDesireResource(
        float initialAmount = 50f,
        float minAmount = 0f,
        float maxAmount = 100f,
        int cycleTicks = 1
        ) : base(ResourceType.Societal_Desire, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        resources[ResourceType.Agreement].TriggerSpecialAction();
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