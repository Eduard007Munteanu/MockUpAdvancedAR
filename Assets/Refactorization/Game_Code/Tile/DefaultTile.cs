using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultTile : MonoBehaviour//, Tile
{


    private List<DefaultMob> mobs;

    private DefaultBuild buildingOnTile;


    // Start is called before the first frame update
    void Start()
    {
        mobs = new List<DefaultMob>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddBuilding(DefaultBuild building){
        buildingOnTile = building;
    }

    DefaultBuild getBuilding(){
        return buildingOnTile;
    }

    
    public (float , float ) GetTileCoordinates()
    {
        float x_coordinate = gameObject.GetComponent<Renderer>().bounds.size.x;
        float y_coordinate = gameObject.GetComponent<Renderer>().bounds.size.y;
        return (x_coordinate, y_coordinate);
    }

    public float GetTileHeight()
    {
        return gameObject.GetComponent<Renderer>().bounds.size.y;
    }


    public void ArrangeMobs(DefaultMob mob){
        Vector3[] tileCorners = GridOverlay.Instance.GetTileCorners(this);
        Vector3 topLeft = tileCorners[0];
        Vector3 topRight = tileCorners[1];
        Vector3 bottomRight = tileCorners[2];
        Vector3 bottomLeft = tileCorners[3];

        //float mobHeight = mob.GetComponent<Renderer>().bounds.size.y;

        if(GetLastMob() == null){
            mob.transform.position = new Vector3(topLeft.x, mob.transform.position.y, topLeft.z);
            mobs.Add(mob);
        }
    }


    public List<DefaultMob> getListOfMobs(){
        return mobs;
    }

    private DefaultMob GetLastMob(){
        return mobs.Count >= 1 ? mobs[mobs.Count - 1] : null;
    }
}
