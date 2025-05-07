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

    protected override void onSpecialAction()
    {
        // calculate differences between the last and current agreement amount
    }

    private float calculateAgreement()
    {
        float civil = resources[ResourceType.Civil].CurrentAmount;
        float economy = resources[ResourceType.Economy].CurrentAmount;
        float societal = resources[ResourceType.Societal].CurrentAmount;
        float civilDesire = resources[ResourceType.Civil_Desire].CurrentAmount;
        float economyDesire = resources[ResourceType.Economy_Desire].CurrentAmount;
        // float societalDesire = resources[ResourceType.Societal_Desire].CurrentAmount;

        float civilDiff = civil - civilDesire;
        float economyDiff = economy - economyDesire;

        float population = resources[ResourceType.Population].CurrentAmount;

        float foodamount = resources[ResourceType.Food].CurrentAmount;
        float foodprod = resources[ResourceType.Food].Production;
        float foodStability = foodamount / foodprod;

        float goldamount = resources[ResourceType.Gold].CurrentAmount;
        float goldprod = resources[ResourceType.Gold].Production;
        float goldStability = goldamount / goldprod;

        float woodAmount = resources[ResourceType.Wood].CurrentAmount;
        float woodProd = resources[ResourceType.Wood].Production;
        float woodStability = woodAmount / woodProd;

        // economy is the most important until the economy is stable
        // then civil

        // check if the economy is stable
        // and then check for absolute values to know how long is left
        

        // societal changes how much the people are willing to change their minds
        // thinking: happiness modifier modifier

        return -1;
    }

    // To handle changes in the political axes 
    // Listen to their actions
}