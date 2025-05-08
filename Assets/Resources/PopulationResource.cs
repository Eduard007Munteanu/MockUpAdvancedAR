using System;
using System.Collections.Generic;
using UnityEngine;

public class PopulationResource : Resource
{
    public event Action<float> OnPopulationChanged;
    
    private float birthRateMod = 0.01f;
    private float ration = 1f;

    public PopulationResource(
        float initialAmount = 5f, // Population might start small
        float minAmount = 0f,
        float maxAmount = 1000f, // Population can grow large
        int cycleTicks = 10,
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
            
        } else { // pop died/removed
            
        }

        AddProductionModifier(calculateBirthRateDelta(delta)); // adjust production based on birth rate
        resources[ResourceType.Happiness].AddAmount(delta * 1f); // adjust happiness
        resources[ResourceType.Food].AddProductionConstant(-delta * ration);

        OnPopulationChanged?.Invoke(delta);
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
}