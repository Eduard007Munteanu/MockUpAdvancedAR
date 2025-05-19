
using System.Collections.Generic;
using UnityEngine;

// individual battle power

public class EnemyMightResource : Resource
{
    public EnemyMightResource(
        float initialAmount = 1f,
        float minAmount = 10f,
        float maxAmount = 1000f,
        int cycleTicks = 1
        ) : base(ResourceType.EnemyMight, initialAmount, minAmount, maxAmount, cycleTicks)
    {

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