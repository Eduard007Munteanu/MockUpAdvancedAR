using UnityEngine;


// public class SchoolBuiliding : DefaultBuild
// {
//     //protected override DefaultBuildingEffect BuildingEffect => new SchoolBuildingEffect();
// }
using System.Collections.Generic;

public class SchoolBuiliding : DefaultBuild
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
            new ResourceEffect(ResourceType.Arts, 0f, 0f, 0.2f),
            new ResourceEffect(ResourceType.Civil, 1f),
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
