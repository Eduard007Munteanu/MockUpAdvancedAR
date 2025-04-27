using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Mobs 
{
    void AssignToBuilding(Build building);

    void RemoveFromBuilding();

    //void MoveTo(Vector3 destination, GameObject colliderObj);

    Item FindClosestItem(string itemName); //Can be further optimized by keeping track of a stack from closest to furthest item

    void CollectItem(PanelDatabase panelDatabase);

    void ReactOnClick();

    float GetMobHeight();

    void InitMove(Vector3 destination, GameObject colliderObj);



}
