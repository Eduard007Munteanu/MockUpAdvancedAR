using System;
using System.Collections.Generic;
using UnityEngine;

// Access resourceDatabase[ResourceType.Arts] to get the resource object
// Add amount: ResourceDatabase.Instance[ResourceType.Arts].AddAmount(10);
// Add modifier: ResourceDatabase.Instance[ResourceType.Arts].AddModifier(10, (optional mod1, mod2));
public class ResourceDatabase : MonoBehaviour
{
    // Singleton instance
    // public static ResourceDatabase Instance { get; private set; } // Remove this auto-property
    // In ResourceDatabase.cs
private static ResourceDatabase _instance;
public static ResourceDatabase Instance
{
    get
    {
        if (_instance == null)
        {
            // This might happen if accessed from an Awake() of another script
            // that runs before ResourceDatabase's Awake().
            // Setting execution order is the best fix.
            Debug.LogError("ResourceDatabase instance is null. Ensure ResourceDatabase.cs execution order is set earlier.");
        }
        return _instance;
    }
    private set { _instance = value; }
}

    void Awake()
    {
        Debug.Log("ResourceDatabase Awake called.");
        if (_instance == null)
        {
            Debug.Log("ResourceDatabase instance being created.");
            _instance = this; // Set _instance directly
            DontDestroyOnLoad(gameObject); // Optional: if you need it to persist across scenes
            InitializeResources();
            Debug.Log("ResourceDatabase initialized with resources AFTER InitializeResources call.");
        }
        else if (_instance != this)
        {
            Debug.LogWarning("Another instance of ResourceDatabase already exists. Destroying this new one.");
            Destroy(gameObject);
        }
    }

    // It's good practice to clean up the static instance if the GameObject is destroyed
    void OnDestroy()
    {
        // Similarly, it's safer to check against the backing field here.
        // if (Instance == this) // Original line
        if (_instance == this) // Change to check the backing field
        {
            // Instance = null; // This uses the property's setter, which is fine. Or use _instance = null;
            _instance = null;
        }
    }

    private Dictionary<ResourceType, Resource> resources;

    // In ResourceDatabase.cs

    private void InitializeResources()
    {
        Debug.Log("InitializeResources in ResourceDatabase: Phase 1 - Construction.");
        // Phase 1: Construct all resource objects
        ArtsResource arts = new ArtsResource();
        FoodResource food = new FoodResource();
        GoldResource gold = new GoldResource();
        WoodResource wood = new WoodResource();
        MightResource might = new MightResource();
        EnemyMightResource enemyMight = new EnemyMightResource();
        PopulationResource population = new PopulationResource();
        CivilResource civil = new CivilResource();
        SocietalResource societal = new SocietalResource();
        EconomyResource economy = new EconomyResource();
        CivilDesireResource civilDesire = new CivilDesireResource();
        EconomyDesireResource economyDesire = new EconomyDesireResource();
        HappinessResource happiness = new HappinessResource();
        WorkPowerResource workPower = new WorkPowerResource();
        AgreementResource agreement = new AgreementResource();
        ScoreResource score = new ScoreResource();

        // Phase 2: Populate the dictionary
        Debug.Log("InitializeResources in ResourceDatabase: Phase 2 - Populating Dictionary.");
        resources = new Dictionary<ResourceType, Resource>
        {
            { agreement.Type, agreement },
            { arts.Type, arts },
            { food.Type, food },
            { gold.Type, gold },
            { wood.Type, wood },
            { might.Type, might },
            { enemyMight.Type, enemyMight },
            { population.Type, population },
            { civil.Type, civil },
            { societal.Type, societal },
            { economy.Type, economy },
            { civilDesire.Type, civilDesire },
            { economyDesire.Type, economyDesire },
            { happiness.Type, happiness },
            { workPower.Type, workPower },
            { score.Type, score }
        };

        Debug.Log("ResourceDatabase initialized with resources. Phase 3 - Post Initialization.");
        // Phase 3: Allow resources to perform post-initialization logic
        foreach (var resourcePair in resources)
        {
            resourcePair.Value.PostInitialize(); // Call a new method
        }

        Debug.Log("ResourceDatabase fully initialized after PostInitialize calls.");
    }

    public Resource this[ResourceType type]
    {
        get
        {
            if (resources == null)
            {
                Debug.LogError("ResourceDatabase has not been initialized yet or resources dictionary is null!");
                return null; // Or throw an exception
            }
            if (resources.TryGetValue(type, out Resource resource))
            {
                return resource;
            }
            else
            {
                Debug.LogError($"Resource type {type} not found in ResourceDatabase.");
                return null; // Or throw an exception
            }
        }
    }

    public void Tick()
    {
        // Call the Tick method on each resource
        foreach (var resource in resources.Values)
        {
            resource.Tick();
        }

        Debug.Log("All resources have been ticked.");
    }
}