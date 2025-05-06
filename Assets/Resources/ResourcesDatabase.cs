using System;
using System.Collections.Generic;

// Access resourceDatabase[ResourceType.Arts] to get the resource object
// Add amount: resourceDatabase[ResourceType.Arts].AddAmount(10);
// Add modifier: resourceDatabase[ResourceType.Arts].AddModifier(10, (optional mod1, mod2));
public class ResourcesDatabase
{
    private Dictionary<ResourceType, Resource> resources;

    public ResourcesDatabase()
    {
        // init resources
        ArtsResource arts = new ArtsResource();
        FoodResource food = new FoodResource();
        MightResource might = new MightResource();

        // use their type defined in their class to add them.
        // add them to the dictionary
        resources = new Dictionary<ResourceType, Resource>
        {
            { arts.Type, arts },
            { food.Type, food },
            { might.Type, might },
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