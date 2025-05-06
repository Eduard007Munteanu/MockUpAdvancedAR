using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ResourcesDatabase
{
    private Dictionary<ResourceType, Resource> resources;

    public ResourcesDatabase()
    {
        // init resources
        FoodResource food = new FoodResource();
        MightResource might = new MightResource();

        // use their type defined in their class to add them.
        // add them to the dictionary
        resources = new Dictionary<ResourceType, Resource>
        {
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