using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBuild : DefaultBuild
{
    
    protected override string Building_class => "sleep";

    //protected override DefaultBuildingEffect BuildingEffect => throw new System.NotImplementedException();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init(int Id, DefaultTile tile) {
        base.Init(Id, tile);
        resourceEffects = new List<ResourceEffect>
        {
            new ResourceEffect(ResourceType.Population, 0f, 0f, 0f, 0f, 0f, 0f, 5f),
            new ResourceEffect(ResourceType.Happiness, 1f),
        };
    }
    
}
