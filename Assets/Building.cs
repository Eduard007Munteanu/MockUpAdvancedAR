using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{


    

    // Start is called before the first frame update
    
    private int id;

    private float size;

    private string building_class;

    private List<GameObject> assignedMobs = new List<GameObject>();

    private GameObject tile;

    
    

    public void Initialization(int ID, string building_class, GameObject tile){    //ID should be modified bassed on the building type  + Tile values also to be added for more concrete spawn
        this.id = ID;
        this.building_class = building_class;
        this.tile = tile;
    }
    
    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReactToPinch(){
        Debug.Log("Building " + id + " pinched!"); //Test
        
    }

    public void SetMaterial(Material material){
        Renderer rend = GetComponent<Renderer>();
        rend.material = material;
    }


    public string GetBuildingClass(){
        return building_class;
    }

    public int GetID(){
        return id; 
    }

    public void addAssignedMob(GameObject mob){
        assignedMobs.Add(mob);
    }

    public void removeAssignedMob(GameObject mob){
        assignedMobs.Remove(mob);
    }

    public int mobCountValue(){
        int mobCount = assignedMobs.Count;
        return mobCount;
    }

    public GameObject GetLastAssignedMob(){
        if(assignedMobs.Count > 0){
            return assignedMobs[assignedMobs.Count - 1];
        }else{
            return null;
        }
    }


    public GameObject getTile(){
        return tile;
    }


    public Vector3[] GetTileCorners(GameObject tile)
    {
        Transform tileTransform = tile.transform;

        Vector3 center = tileTransform.position;
        Vector3 right = tileTransform.right * 0.5f * tileTransform.localScale.x * 10f;
        Vector3 forward = tileTransform.forward * 0.5f * tileTransform.localScale.z * 10f;

        Vector3 topLeft     = center - right + forward;
        Vector3 topRight    = center + right + forward;
        Vector3 bottomLeft  = center - right - forward;
        Vector3 bottomRight = center + right - forward;

        return new Vector3[] { topLeft /*bottomRight from board perspective  */, topRight /*bottomLeft from board perspective */, /*topLeft from board perspective */ bottomRight, /*topRight from board perspective */ bottomLeft };
    }
}

