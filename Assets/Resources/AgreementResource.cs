// --- Hi ---
// Remember to add the ResourceType to Resources.cs

using System.Collections.Generic;
using UnityEditor.Media;
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
        resources[ResourceType.Happiness].AddAmount(delta*0.5f);
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
        // calculate differences between the last and current agreement amount
        float deltaAmount = calculateAgreement() - lastAgreementAmount;
        lastAgreementAmount = CurrentAmount;
    }

    private float calculateAgreement()
    {
        float civil = resources[ResourceType.Civil].CurrentAmount;
        float economy = resources[ResourceType.Economy].CurrentAmount;
        float civilDesire = resources[ResourceType.Civil_Desire].CurrentAmount;
        float economyDesire = resources[ResourceType.Economy_Desire].CurrentAmount;
        float societal = resources[ResourceType.Societal].CurrentAmount;
        // float societalDesire = resources[ResourceType.Societal_Desire].CurrentAmount;

        float civilDiff = civil - civilDesire;
        float economyDiff = economy - economyDesire;

        // value between 0 and 100f
        float agreement = 0.5f * civilDiff + 0.5f * economyDiff; // TODO: Update weights

        return agreement;
    }
}