using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Build
{
   
    void AddAssignedMob();

    void RemoveAssignedMob();


    void GetBuildingClass();


    void GetID();

     void GetTile();

     int GetMobCount();

     List<Mobs> GetMobs(); 

    Mobs GetSpecificMob(int index); 
}
