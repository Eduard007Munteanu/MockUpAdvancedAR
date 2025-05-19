using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmBuild : DefaultBuild
{

    protected override string Building_class => "farming";

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
            new ResourceEffect(ResourceType.Economy, 1f),
        };
    }
    



    

    
}
