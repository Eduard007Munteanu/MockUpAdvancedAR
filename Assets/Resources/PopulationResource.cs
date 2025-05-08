using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class PopulationResource : Resource
{
    public event Action<float> OnBirth;
    
    private float birthRateMod = 0.001f;
    private float ration = 1f;

    private int startingPopulation = 5; // Initial population amount
    private bool startingPopulationDone = false;
    private bool firstCycle = true;
    private int skipCycles = 5;

    public PopulationResource(
        float initialAmount = 0f, // Population might start small
        float minAmount = 0f,
        float maxAmount = 1000f, // Population can grow large
        int cycleTicks = 1,
        List<float> initialThresholds = null, // Optional thresholds for this resource
        float initialFlat = 1f, 
        float initialMod1 = 1f, 
        float initialMod2 = 1f, 
        float initialConst = 0f
        ) : base(ResourceType.Population, initialAmount, minAmount, maxAmount, cycleTicks, initialThresholds, initialFlat, initialMod1, initialMod2, initialConst) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {
        if (delta > 0f) { // pop born/added
            resources[ResourceType.Happiness].AddAmount(delta * 0.5f); // adjust happiness
        } else { // pop died/removed
            resources[ResourceType.Happiness].AddAmount(delta * 2f); // adjust happiness
        }

        AddProductionModifier(calculateBirthRateDelta(delta)); // adjust production based on birth rate
        resources[ResourceType.Food].AddProductionConstant(-delta * ration);

        if (CurrentAmount > (int) CurrentAmount)
            OnBirth?.Invoke(delta); // a miracle

        if (startingPopulation <= 0 && !startingPopulationDone) {
            startingPopulationDone = true;
            Debug.Log("kill yourself");
            AddProductionModifier(-1f);
            AddAmount(-1f);
            MinimumAmount = 0f;
        } else {
            startingPopulation--;
        }
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

    private float calculateBirthRateDelta(float delta) {
        return birthRateMod * delta;
    }

    public void InitialPops() {

        for (int i = 0; i < startingPopulation; i++) {
            AddAmount(1f); // add 1 pop per cycle
        }
        
    }
}