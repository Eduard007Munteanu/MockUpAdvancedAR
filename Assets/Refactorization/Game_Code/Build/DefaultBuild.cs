using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json.Schema;

// TODO:
// - Give birth

public abstract class DefaultBuild : MonoBehaviour, Build  // Was not abstract to begin with  => In case error appears
{

    [SerializeField] private GameObject PanelPrefab;

    private List<DefaultMob> assignedMobs = new List<DefaultMob>();  //Given mob added in assignedMobs, effect should happen. 

    private int id;

    protected virtual string Building_class => "Default";

    protected virtual float HP { get; set; } = 40;

    private DefaultTile tile;

    protected List<ResourceEffect> resourceEffects;

    protected ResourceDatabase resources; // Singleton instance of ResourceDatabase
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public float GetHP(){
        return HP;
    }

    public void SetHP(float newHP){
        HP = newHP;
    }


    public virtual void Init(int Id,  DefaultTile tile =null)
    {   
        while (resources == null){
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }
        
        this.id = Id;
        this.tile = tile;
        if(tile != null){
            Debug.Log("For building " + GetBuildingClass() + "tile is " + tile.name );
            this.tile.AddBuilding(this);
        }
    }

    public void AddAssignedMob(DefaultMob mob)
    {
        Debug.Log("AddAssignedMob");

        assignedMobs.Add(mob);

        foreach (var effect in resourceEffects) effect.Apply();

        // debug resource effect length
        Debug.Log($"ResourceEffect count: {resourceEffects.Count}");
        foreach (var effect in resourceEffects)
        {
            Debug.Log($"ResourceEffect: {effect.Type} - {effect.Amount} - {effect.Flat} - {effect.Mod1} - {effect.Mod2} - {effect.Constant}");
        }
    }


    public void RemoveAssignedMob(DefaultMob mob)
    {
        assignedMobs.Remove(mob);

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