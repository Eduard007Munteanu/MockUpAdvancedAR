using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json.Schema;

public abstract class DefaultBuild : MonoBehaviour, Build  // Was not abstract to begin with  => In case error appears
{

    [SerializeField] private GameObject PanelPrefab;

    private List<DefaultMob> assignedMobs = new List<DefaultMob>();  //Given mob added in assignedMobs, effect should happen. 

    private int id;

    protected virtual string Building_class => "Default";

    private DefaultTile tile;

    //protected abstract DefaultBuildingEffect BuildingEffect { get; }
    protected virtual List<ResourceEffect> resourceEffects => new List<ResourceEffect>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Init(int Id,  DefaultTile tile =null)
    {   
        this.id = Id;
        this.tile = tile;
    }




    public void AddAssignedMob(DefaultMob mob)
    {
        assignedMobs.Add(mob);
        //BuildingEffect.Effect();
        foreach (var effect in resourceEffects) effect.Apply();
    }


    public void RemoveAssignedMob(DefaultMob mob)
    {
        assignedMobs.Remove(mob);
        //BuildingEffect.NegativeEffect();
        foreach (var effect in resourceEffects) effect.Cancel();
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

    public DefaultMob GetSpecificActualMob(DefaultMob mob){
        int index = assignedMobs.IndexOf(mob);
        if (index >= 0)
        {
            return assignedMobs[index];
        }
        else
        {
            Debug.LogError("Mob not found in the list");
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


    public DefaultTile GetTile()
    {
        return tile;
    }

    

    public DefaultPanel GetPanel()
    {
        return PanelPrefab.GetComponent<DefaultPanel>();
    }

    public GameObject GetPanelPrefab()
    {
        return PanelPrefab;
    }

    public string GetBuildingClass()
    {
        return Building_class;
    }


    public virtual Vector3 SpawnBuilding(){
        return tile.transform.position;
    }

    public virtual void CreateMob(){

    }

    



}