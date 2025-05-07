using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmBuild : DefaultBuild
{

    protected override string Building_class => "farming";

    protected override List<ResourceEffect> resourceEffects => new List<ResourceEffect>
    {
        // new ResourceEffect(ResourceType.Food, 0f, 0f, 0f, 0f),
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    



    

    
}
