// --- Resource.cs ---
using UnityEngine;
using System;
using System.Collections.Generic;
// using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine.InputSystem.Controls;
using Unity.VisualScripting; // Required for Action delegate

// Hi Eduard
// This is a base class for all resources in the game.

// produces production per cycle and will manage thresholds too have to figure that one out

// You can interact with the resources the following ways:
// - AddAmount(float delta, applymods): Adds a specified amount to the resource.
// Like when materials are produced or consumed. For collecting materials like golditem 
// use applymods = true this way added amount will be boosted by production factors
// - AddProductionModifier(float flatDelta, float mod1Delta, float mod2Delta): Modifies the production factors of the resource.
// - AddProductionConstant(float constDelta): added directly to amount every cycle
// - TriggerSpecialAction(): Call this to trigger a special action for the resource. This is a placeholder for any special logic you want to implement.
// - Tick(): Call this every tick to update the resource. It will check if it's time to produce more resources.


public enum ResourceType
{
    Population,
    Arts, Food, Gold, Wood,
    Civil, Societal, Economy,
    Civil_Desire, Societal_Desire, Economy_Desire, 
    Agreement,
    Happiness,
    Might, EnemyMight,// Threat, TollRatio, 
    // DrawAmount, DrawsLeft, // ??
    WorkPower,

    // Add other resources/stats as needed
}

// Abstract base class for all game resources/stats.
// Defines common structure and behavior for tracking amounts and calculating production.
public abstract class Resource
{

    // State: Basic properties of the resource
    public ResourceType Type { get; private set; }
    public float CurrentAmount { get; protected set; }
    public float MinimumAmount { get; protected set; } // Configurable, often 0
    public float MaximumAmount { get; protected set; } // Configurable storage limit/cap
    public float Production { get; protected set; }

    // Production calculation: overall_production_per_cycle = (flat * mod1 * mod2) + const
    // Protected allows derived classes potential access if complex logic requires it, otherwise could be private.
    protected float flat;
    protected float mod1;
    protected float mod2;
    protected float constant;
    protected bool randomProduction = false;

    // Thresholds probably other class
    protected Thresholds thresholds; // TODO: List of thresholds for this resource

    // Production Timing
    private int productionCycleTicks; // ticks between production attempts
    private int ticksUntilNextCycle;  // counter for the current cycle

    protected ResourceDatabase resources; // Reference to the database

    // Events for notification
    public event Action<ResourceType, float> OnAmountChanged; // Parameter is the new CurrentAmount
    public event Action<ResourceType, float> OnProductionChanged; // Parameter is the new productionPerCycle
    public event Action<ResourceType, int> OnThresholdCrossed; // Parameter is the index of the threshold crossed
    public event Action<ResourceType, float> OnReachedMax; // Parameter is the excess amount
    public event Action<ResourceType, float> OnReachedMin; // Parameter is the deficit amount

    private float initialAmount, initialFlat, initialMod1, initialMod2, initialConst; // Initial values for the resource

    // Constructor: Initializes the resource. Called by concrete implementations.
    protected Resource(
        ResourceType type, 
        float initialAmount, 
        float minAmount, 
        float maxAmount, 
        int cycleTicks,
        List<float> initialThresholds = null, // Optional thresholds for this resource
        float initialFlat = 0f, 
        float initialMod1 = 1f, 
        float initialMod2 = 1f, 
        float initialConst = 0f
        )
    {
        Type = type;
        MinimumAmount = minAmount;
        MaximumAmount = maxAmount;
        productionCycleTicks = cycleTicks;
        ticksUntilNextCycle = productionCycleTicks; // Start ready for the first cycle or wait full cycle? Let's wait.
        thresholds = (initialThresholds == null) ? null : new Thresholds(initialThresholds, CurrentAmount); // Initialize thresholds

        this.initialAmount = initialAmount; // Store initial values for later use
        this.initialFlat = initialFlat;
        this.initialMod1 = initialMod1;
        this.initialMod2 = initialMod2;
        this.initialConst = initialConst;
    }

    public void PostInitialize() {
        resources = ResourceDatabase.Instance; // Get the singleton instance of the resource database
        if (resources == null)
        {
            Debug.LogError($"ResourcesDatabase is null for {Type}");
            return;
        }
        AddAmount(Mathf.Clamp(initialAmount, MinimumAmount, MaximumAmount)); // Ensure initial amount is within bounds
        AddProductionModifier(initialFlat, initialMod1, initialMod2); // Initialize production factors
        AddProductionConstant(initialConst); // Initialize production constant

        RecalculateProduction(); // Calculate initial production rate
    }

    // Modifies CurrentAmount by delta, clamps between MinimumAmount and MaximumAmount.
    // Triggers onAmountChange() for derived class logic and OnAmountChanged event.
    public virtual void AddAmount(float delta, bool applyMods = false)
    {
        Debug.Log($"ResourceBase AddAmount {delta} to {Type}. Current amount: {CurrentAmount}");

        float previousAmount = CurrentAmount;
        delta = applyMods ? delta * Production : delta; // Apply production rate if requested
        float noClampAmount = CurrentAmount + delta; // Calculate new amount without clamping
        CurrentAmount = Mathf.Clamp(noClampAmount, MinimumAmount, MaximumAmount);

        // Only trigger if the amount actually changed
        if (CurrentAmount != previousAmount)
        {
            if (thresholds != null)
            {
                var threshDict = thresholds.CheckThresholdsCrossed(CurrentAmount);
                if (threshDict != null) // if any thresholds crossed
                {
                    foreach (var thresh in threshDict)
                    {
                        onThresholdCrossed(thresh.Key, thresh.Value);
                        // OnThresholdCrossed?.Invoke(Type, thresh.Value); // Notify external listeners (UI etc.)
                    }
                }
            }

            onAmountChange(delta); // Call abstract method for derived class logic (thresholds etc.)
            OnAmountChanged?.Invoke(Type, CurrentAmount); // Invoke event for external listeners (UI etc.)
            // TODO: Not sure to notify them here or in the subclasses
        }

        if (noClampAmount >= MaximumAmount) // Check if we exceeded the max amount
        {
            float excess = noClampAmount - MaximumAmount; // Calculate excess amount
            onReachedMax(excess); // Call abstract method for derived class logic (e.g., level up)
            OnReachedMax?.Invoke(Type, excess); // Invoke event for external listeners (UI etc.)
        }
        else if (noClampAmount <= MinimumAmount) // Check if we fell below the min amount
        {
            float deficit = MinimumAmount - noClampAmount; // Calculate deficit amount
            onReachedMin(deficit); // Call abstract method for derived class logic (e.g., level down)
            OnReachedMin?.Invoke(Type, deficit); // Invoke event for external listeners (UI etc.)
        }
    }

    // Modifies the multiplicative production factors (flat, mod1, mod2).
    // Recalculates the cached production rate and triggers change notifications.
    public virtual void AddProductionModifier(float flatDelta = 0f, float mod1Delta = 0f, float mod2Delta = 0f)
    {
        Debug.Log($"ResourceBase AddProductionModifier {flatDelta}, {mod1Delta}, {mod2Delta} to {Type}. Current production: {Production}");
        
        flat += flatDelta;
        mod1 += mod1Delta;
        mod2 += mod2Delta;
        // Ensure modifiers don't go below zero if that's intended design? Assume they can for now.

        RecalculateProduction();
    }

    // Modifies the additive constant production factor.
    // Recalculates the cached production rate and triggers change notifications.
    public virtual void AddProductionConstant(float constDelta)
    {
        Debug.Log($"ResourceBase AddProductionConstant {constDelta} to {Type}. Current production: {Production}");

        constant += constDelta;
        
        RecalculateProduction();
    }

    public virtual void TriggerSpecialAction()
    {
        Debug.Log($"ResourceBase TriggerSpecialAction for {Type}. Current production: {Production}");
        
        onSpecialAction(); // Call abstract method for derived class logic (e.g., special events)
    }

    // Recalculates the cached production value based on current factors.
    // Called internally when factors change.
    private void RecalculateProduction()
    {
        // Debug.Log($"ResourceBase RecalculateProduction for {Type}. Current production: {Production}");
        
        // Core production formula
        float prevProduction = Production; // Store previous production for comparison
        Production = (flat * mod1 * mod2) + constant;
        float delta = Production - prevProduction; // Calculate change in production rate
        // Potentially clamp productionPerCycle if needed (e.g., minimum production rate)
        onProductionChange(delta); // Call abstract method for derived class logic
        OnProductionChanged?.Invoke(Type, Production); // Invoke event
    }

    // When the timer reaches zero, it triggers the Produce() method and resets.
    public virtual void Tick()
    {
        Debug.Log($"Tick for {Type}. amount: {CurrentAmount}. production: {Production}.");
        
        if (productionCycleTicks == 0) return;
        ticksUntilNextCycle--;
        if (ticksUntilNextCycle <= 0)
        {
            Produce();
            ticksUntilNextCycle = productionCycleTicks; // Reset timer
        }
    }

    // Applies the cached production amount to the CurrentAmount.
    // This is the point where the resource quantity actually changes due to regular production.
    // Can be overridden if production logic is more complex (e.g., requires Kingdom state).
    protected virtual void Produce()
    {
        // THIS IS DIRTY BUT I DO IT FOR ART
        var prod = randomProduction ? (float) (int) UnityEngine.Random.Range(0, Production) : Production; // Randomize production if needed
        // Debug.Log($"ResourceBase Produce for {Type}. Current production: {prod}");
        if (Production == 0) return;
        AddAmount(prod);
    }

    // Abstract method: Called by AddAmount after CurrentAmount changes.
    // Concrete classes MUST implement this to handle resource-specific logic,
    // such as checking thresholds, triggering game events, or modifying other resources via Kingdom.
    protected abstract void onAmountChange(float delta);

    // Abstract method: Called after production factors (flat, mod1, mod2, constant) change.
    // Concrete classes MUST implement this if actions are needed when the *rate* changes
    // (e.g., updating UI elements displaying the production rate).
    protected abstract void onProductionChange(float delta);
    // called on the derived class when the threshold is crossed
    protected abstract void onThresholdCrossed(int i, ThresholdCross dir);
    protected abstract void onReachedMax(float excess);
    protected abstract void onReachedMin(float deficit);
    protected abstract void onSpecialAction();

    public int GetTicksPerCycle() => productionCycleTicks;
    public int GetTicksUntilNextCycle() => ticksUntilNextCycle;

    // Provide getters for factors if needed for display or complex logic
    public float GetFlatFactor() => flat;
    public float GetMod1Factor() => mod1;
    public float GetMod2Factor() => mod2;
    public float GetConstantFactor() => constant;
}