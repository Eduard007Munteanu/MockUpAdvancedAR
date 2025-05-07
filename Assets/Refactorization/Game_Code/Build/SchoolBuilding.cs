using UnityEngine;


public class SchoolBuiliding : DefaultBuild
{
    protected override DefaultBuildingEffect BuildingEffect => new SchoolBuildingEffect();
}

