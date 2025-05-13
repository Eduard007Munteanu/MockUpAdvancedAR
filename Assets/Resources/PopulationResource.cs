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
        float maxAmount = 10f, // Population can grow large
        int cycleTicks = 1,
        List<float> initialThresholds = null, // Optional thresholds for this resource
        float initialFlat = 0.05f, 
        float initialMod1 = 1f, 
        float initialMod2 = 1f, 
        float initialConst = 0f
        ) : base(ResourceType.Population, initialAmount, minAmount, maxAmount, cycleTicks, initialThresholds, initialFlat, initialMod1, initialMod2, initialConst) // TODO: Update ResourceType
    {
        // thresholds = new Thresholds(new List<float> { /* ...threshold values... */ }, initialAmount);
        thresholds = new Thresholds(new List<float> {
            50f,
        }, initialAmount);
    }

    protected override void onAmountChange(float delta)
    {

        float oldAmount = CurrentAmount - delta; // Calculate amount before this change
        int oldFloor = Mathf.FloorToInt(oldAmount);
        int newFloor = Mathf.FloorToInt(CurrentAmount);

        // Population Increased across one or more integer thresholds
        if (newFloor > oldFloor)
        {
            int unitsIncreased = newFloor - oldFloor;
            for (int i = 0; i < unitsIncreased; i++)
            {
                Debug.Log($"PopulationResource: a miracle (crossed integer threshold to {oldFloor + i + 1})");
                OnBirth?.Invoke(1f); // Invoke for each whole unit of population increase
            }
            if (unitsIncreased > 0)
            {
                resources[ResourceType.Happiness].AddAmount(unitsIncreased * 0.5f); // adjust happiness
                AddProductionModifier(calculateBirthRateDelta(unitsIncreased)); // adjust production based on birth rate
                resources[ResourceType.Food].AddProductionConstant(-unitsIncreased * ration); // food consumption increases
            }
            resources[ResourceType.Score].AddAmount(unitsIncreased * 5f);
        }
        // Population Decreased across one or more integer thresholds
        else if (newFloor < oldFloor)
        {
            int unitsDecreased = oldFloor - newFloor; // Positive number of integers crossed downwards
            if (unitsDecreased > 0)
            {
                // OnBirth is typically not invoked for decreases
                resources[ResourceType.Happiness].AddAmount(-unitsDecreased * 2f); // adjust happiness (decrease)
                AddProductionModifier(calculateBirthRateDelta(-unitsDecreased)); // adjust production based on population loss
                resources[ResourceType.Food].AddProductionConstant(unitsDecreased * ration); // food consumption decreases (so, add to production constant)
            }
        }
        if (CurrentAmount > 1000f)
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

    private float calculateBirthRateDelta(float delta) {
        return birthRateMod * delta;
    }

    public void InitialPops() {

        for (int i = 0; i < startingPopulation; i++) {
            AddAmount(1f); // add 1 pop per cycle
        }

        Debug.Log($"PopulationResource: Initial population set to {startingPopulation}.");
        
    }
}