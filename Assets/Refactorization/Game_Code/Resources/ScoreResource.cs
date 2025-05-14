using System.Collections.Generic;
using UnityEngine;

public class ScoreResource : Resource
{

    public ScoreResource(
        float initialAmount = 0f,
        float minAmount = 0f,
        float maxAmount = 100000f, // Gold might have a higher max
        int cycleTicks = 0
        ) : base(ResourceType.Score, initialAmount, minAmount, maxAmount, cycleTicks)
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        
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