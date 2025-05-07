using System;
using System.Collections.Generic;
using UnityEngine;

// Access resourceDatabase[ResourceType.Arts] to get the resource object
// Add amount: resourceDatabase[ResourceType.Arts].AddAmount(10);
// Add modifier: resourceDatabase[ResourceType.Arts].AddModifier(10, (optional mod1, mod2));
public class ResourceDatabase : MonoBehaviour
{
    // Singleton instance
    public static ResourceDatabase Instance { get; private set; }

    private Dictionary<ResourceType, Resource> resources;

    public ResourceDatabase()
    {
        // init resources
        ArtsResource arts = new ArtsResource();
        FoodResource food = new FoodResource();
        MightResource might = new MightResource();
        PopulationResource population = new PopulationResource();
        GoldResource gold = new GoldResource();
        CivilResource civil = new CivilResource();
        SocietalResource societal = new SocietalResource();
        EconomyResource economy = new EconomyResource();
        CivilDesireResource civilDesire = new CivilDesireResource();
        SocietalDesireResource societalDesire = new SocietalDesireResource();
        EconomyDesireResource economyDesire = new EconomyDesireResource();
        HappinessResource happiness = new HappinessResource();
        WorkPowerResource workPower = new WorkPowerResource();

        // use their type defined in their class to add them.
        // add them to the dictionary
        resources = new Dictionary<ResourceType, Resource>
        {
            { arts.Type, arts },
            { food.Type, food },
            { might.Type, might },
            { population.Type, population },
            { gold.Type, gold },
            { civil.Type, civil },
            { societal.Type, societal },
            { economy.Type, economy },
            { civilDesire.Type, civilDesire },
            { societalDesire.Type, societalDesire },
            { economyDesire.Type, economyDesire },
            { happiness.Type, happiness },
            { workPower.Type, workPower },
        };
    }

    public Resource this[ResourceType type]
    {
        get
        {
            return resources[type];
        }
    }
}