using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json.Schema;

public class DefaultBuild : MonoBehaviour, Build
{

    [SerializeField] private GameObject PanelPrefab;

    private List<DefaultMob> assignedMobs = new List<DefaultMob>();

    private int id;

    private string building_class;

    private DefaultTile tile;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Init(int Id, string building_class, DefaultTile tile)
    {   
        this.id = Id;
        this.building_class = building_class;
        this.tile = tile;
    }




    public void AddAssignedMob(DefaultMob mob)
    {
        assignedMobs.Add(mob);
    }


    public void RemoveAssignedMob(DefaultMob mob)
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

    public List<DefaultMob> GetMobs()
    {
        return assignedMobs;
    }

    public DefaultMob GetSpecificMob(int index)
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

    public DefaultMob GetLastAssignedMob(){
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
        return PanelPrefab.GetComponent<DefaultPanel>();
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
        return tile.transform.position;
    }

    public virtual void CreateMob(){

    }

    



}