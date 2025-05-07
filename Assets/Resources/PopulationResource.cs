using System.Collections.Generic;
using UnityEngine;

public class PopulationResource : Resource
{

    public PopulationResource(
        float initialAmount = 10f, // Population might start small
        float minAmount = 0f,
        float maxAmount = 1000f, // Population can grow large
        int cycleTicks = 1
        ) : base(ResourceType.Population, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        
    }

    protected override void onProductionChange(float delta)
    {

    }

    protected override void onThresholdCrossed(int i, ThresholdCross dir)
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