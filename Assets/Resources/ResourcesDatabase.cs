using System;
using System.Collections.Generic;
using UnityEngine;

// Access resourceDatabase[ResourceType.Arts] to get the resource object
// Add amount: ResourceDatabase.Instance[ResourceType.Arts].AddAmount(10);
// Add modifier: ResourceDatabase.Instance[ResourceType.Arts].AddModifier(10, (optional mod1, mod2));
public class ResourceDatabase : MonoBehaviour
{
    // Singleton instance
    public static ResourceDatabase Instance { get; private set; }

    private Dictionary<ResourceType, Resource> resources;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            // If not, set this instance as the singleton
            Instance = this;

            // Optionally, make this object persist across scene loads
            // DontDestroyOnLoad(gameObject); // Uncomment this line if you need the ResourceDatabase to persist

            // Initialize the resources here
            InitializeResources();
        }
        else if (Instance != this)
        {
            // If an instance already exists and it's not this one, destroy this one.
            // This prevents duplicate instances.
            Debug.LogWarning("Another instance of ResourceDatabase already exists. Destroying this new one.");
            Destroy(gameObject);
        }
    }

    // Moved resource initialization to its own method for clarity
    private void InitializeResources()
    {
        // init resources
        ArtsResource arts = new ArtsResource();
        FoodResource food = new FoodResource();
        GoldResource gold = new GoldResource();
        WoodResource wood = new WoodResource();
        MightResource might = new MightResource();
        PopulationResource population = new PopulationResource();
        CivilResource civil = new CivilResource();
        SocietalResource societal = new SocietalResource();
        EconomyResource economy = new EconomyResource();
        CivilDesireResource civilDesire = new CivilDesireResource();
        // SocietalDesireResource societalDesire = new SocietalDesireResource();
        EconomyDesireResource economyDesire = new EconomyDesireResource();
        HappinessResource happiness = new HappinessResource();
        WorkPowerResource workPower = new WorkPowerResource();
        AgreementResource agreement = new AgreementResource();


        // use their type defined in their class to add them.
        // add them to the dictionary
        resources = new Dictionary<ResourceType, Resource>
        {
            { arts.Type, arts },
            { food.Type, food },
            { gold.Type, gold },
            { wood.Type, wood },
            { might.Type, might },
            { population.Type, population },
            { civil.Type, civil },
            { societal.Type, societal },
            { economy.Type, economy },
            { civilDesire.Type, civilDesire },
        //    { societalDesire.Type, societalDesire },
            { economyDesire.Type, economyDesire },
            { happiness.Type, happiness },
            { workPower.Type, workPower },
            { agreement.Type, agreement }
        };

        Debug.Log("ResourceDatabase initialized with resources.");
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

    // It's good practice to clean up the static instance if the GameObject is destroyed
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}