using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMob : MonoBehaviour, Mobs
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AssignToBuilding(Build building)
    {
        throw new System.NotImplementedException();
    }

    public void CollectItem(PanelDatabase panelDatabase)
    {
        throw new System.NotImplementedException();
    }

    public Item FindClosestItem(string itemName)
    {
        throw new System.NotImplementedException();
    }

    public void MoveTo(Vector3 destination, GameObject colliderObj)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveFromBuilding()
    {
        throw new System.NotImplementedException();
    }

    
}
