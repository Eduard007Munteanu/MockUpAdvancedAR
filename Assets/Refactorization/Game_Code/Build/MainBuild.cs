using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuild : MonoBehaviour, Build
{



    [SerializeField] private GameObject PanelPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void AddAssignedMob()
    {
        throw new System.NotImplementedException();
    }

    public void GetBuildingClass()
    {
        throw new System.NotImplementedException();
    }

    public void GetID()
    {
        throw new System.NotImplementedException();
    }

    public int GetMobCount()
    {
        throw new System.NotImplementedException();
    }

    public List<Mobs> GetMobs()
    {
        throw new System.NotImplementedException();
    }

    public Mobs GetSpecificMob(int index)
    {
        throw new System.NotImplementedException();
    }

    public void GetTile()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAssignedMob()
    {
        throw new System.NotImplementedException();
    }

    public void Init(string BuildingClassName, Tile tile)
    {
        throw new System.NotImplementedException();
    }

    Tile Build.GetTile()
    {
        throw new System.NotImplementedException();
    }

    public void SetTile(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public Panel GetPanel()
    {
        return PanelPrefab.GetComponent<Panel>();
    }

    public GameObject GetPanelPrefab()
    {
        return PanelPrefab;
    }

    string Build.GetBuildingClass()
    {
        throw new System.NotImplementedException();
    }
}
