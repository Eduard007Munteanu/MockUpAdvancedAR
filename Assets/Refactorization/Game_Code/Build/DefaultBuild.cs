using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json.Schema;

public class DefaultBuild : MonoBehaviour
{

    [SerializeField] private GameObject PanelPrefab;

    private List<Mobs> assignedMobs = new List<Mobs>();

    private int id;

    private string building_class;

    private Tile tile;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Init(int Id, string building_class, Tile tile)
    {   
        this.id = Id;
        this.building_class = building_class;
        this.tile = tile;
    }




    public void AddAssignedMob(Mobs mob)
    {
        assignedMobs.Add(mob);
    }


    public void RemoveAssignedMob(Mobs mob)
    {
        assignedMobs.Remove(mob);
    }

    

    public int GetID()
    {
        return id;
    }

    public int GetMobCount()
    {
        return assignedMobs.Count;
    }

    public List<Mobs> GetMobs()
    {
        return assignedMobs;
    }

    public Mobs GetSpecificMob(int index)
    {
        if (assignedMobs.Count > 0)
        {
            return assignedMobs[index];
        }
        else
        {
            Debug.LogError("No such element given index");
            return null;
        }
    }

    public Mobs GetLastAssignedMob(){
        if(assignedMobs.Count > 0){
            return assignedMobs[assignedMobs.Count - 1];
        }else{
            return null;
        }
    }


    public Tile GetTile()
    {
        return tile;
    }

    

    public Panel GetPanel()
    {
        return PanelPrefab.GetComponent<Panel>();
    }

    public GameObject GetPanelPrefab()
    {
        return PanelPrefab;
    }

    public string GetBuildingClass()
    {
        return building_class;
    }


    public virtual Vector3 SpawnBuilding(){
        return ((MonoBehaviour)tile).transform.position;
    }



}