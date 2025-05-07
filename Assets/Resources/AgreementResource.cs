// --- Hi ---
// Remember to add the ResourceType to Resources.cs

using System.Collections.Generic;
using UnityEngine;

public class AgreementResource : Resource
{
    private float lastAgreementAmount = 0f;

    public AgreementResource(
        float initialAmount = 0f,
        float minAmount = 0f,
        float maxAmount = 100f,
        int cycleTicks = 1
        ) : base(ResourceType.Agreement, initialAmount, minAmount, maxAmount, cycleTicks) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
        // calculate initial agreement
    }

    protected override void onAmountChange(float delta)
    {


        lastAgreementAmount = CurrentAmount;
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

    // To handle changes in the political axes 
    // Listen to their actions
}