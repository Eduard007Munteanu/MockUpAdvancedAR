using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryBuild : DefaultBuild
{

    protected override string Building_class => "military";

    //protected override DefaultBuildingEffect BuildingEffect => throw new System.NotImplementedException();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Init(int Id, DefaultTile tile) {
        base.Init(Id, tile);
        resourceEffects = new List<ResourceEffect>
        {
            new ResourceEffect(ResourceType.Might, 5f),
            new ResourceEffect(ResourceType.Civil, -3f)
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
