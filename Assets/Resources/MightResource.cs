
using System.Collections.Generic;
using UnityEngine;

// individual battle power

public class MightResource : Resource
{
    public MightResource(
        float initialAmount = 1f,
        float minAmount = 1f,
        float maxAmount = 10000f,
        int cycleTicks = 1
        ) : base(ResourceType.Might, initialAmount, minAmount, maxAmount, cycleTicks)
    {

    }

    protected override void onAmountChange(float delta)
    {
        if (CurrentAmount > 50f)
        {
            CubePaintings.Instance.AddPainting(2);
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

    }

    protected override void onSpecialAction()
    {
        
    }
}