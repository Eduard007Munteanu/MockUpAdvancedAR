using UnityEngine;

public class MobSpawnButton : MonoBehaviour
{
    private MainBuild linkedMainBuild;

    public void LinkBuilding(MainBuild mainBuild)
    {
        linkedMainBuild = mainBuild;
    }

    public void TriggerMobSpawn()
    {
        if (linkedMainBuild != null)
        {
            linkedMainBuild.CreateMob();
        }
    }
}
