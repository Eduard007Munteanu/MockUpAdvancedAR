using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface Build
{


    void Init(string BuildingClassName, Tile tile);
   
    void AddAssignedMob();

    void RemoveAssignedMob();


    string GetBuildingClass();


    void GetID();

    Tile GetTile();

    void SetTile(Tile tile);

    int GetMobCount();

    List<Mobs> GetMobs(); 

    Mobs GetSpecificMob(int index); 

    Panel GetPanel();

    GameObject GetPanelPrefab();
}
