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


    public void ArrangeMobs(DefaultMob mob)
    {
        Vector3[] tileCorners = GridOverlay.Instance.GetTileCorners(this);
        Vector3 topLeft = tileCorners[0];
        Vector3 topRight = tileCorners[1];
        Vector3 bottomRight = tileCorners[2];
        Vector3 bottomLeft = tileCorners[3];

        int maxCols = 5;

        
        Vector3 xDir = (topRight - topLeft).normalized;
        Vector3 zDir = (bottomLeft - topLeft).normalized;

        float tileWidth = Vector3.Distance(topLeft, topRight);
        float tileHeight = Vector3.Distance(topLeft, bottomLeft);
        float colSpacing = tileWidth / maxCols;

        int mobIndex = mobs.Count;
        int col = mobIndex % maxCols;
        int row = mobIndex / maxCols;

        float rowHeight = tileHeight / 3f;   //Let's have some empty space as well. 

        
        if (row * colSpacing > rowHeight)
        {
            Debug.LogWarning("No more vertical space to place mobs!");
        }

        Vector3 spawnPos = topLeft + (xDir * colSpacing * col) + (zDir * colSpacing * row);
        spawnPos.y = mob.transform.position.y; 

        mob.transform.position = spawnPos;
        mob.currentTile = this;
        mobs.Add(mob);
    }


    public virtual bool CanMobBeArrangedChecker(DefaultMob mob){                               //Duplication.... To be fixed.
        Vector3[] tileCorners = GridOverlay.Instance.GetTileCorners(this);
        Vector3 topLeft = tileCorners[0];
        Vector3 topRight = tileCorners[1];
        Vector3 bottomRight = tileCorners[2];
        Vector3 bottomLeft = tileCorners[3];

        int maxCols = 5;

        
        Vector3 xDir = (topRight - topLeft).normalized;
        Vector3 zDir = (bottomLeft - topLeft).normalized;

        float tileWidth = Vector3.Distance(topLeft, topRight);
        float tileHeight = Vector3.Distance(topLeft, bottomLeft);
        float colSpacing = tileWidth / maxCols;

        int mobIndex = mobs.Count;
        int col = mobIndex % maxCols;
        int row = mobIndex / maxCols;

        float rowHeight = tileHeight / 3f;   //Let's have some empty space as well. 


        if (row * colSpacing > rowHeight)
        {
            Debug.LogWarning("No more vertical space to place mobs!");
            return false;
        }
        return true;
    }




    public void RemoveMob(DefaultMob mob){
        mobs.Remove(mob);
    }





    


    public List<DefaultMob> getListOfMobs(){
        return mobs;
    }

    private DefaultMob GetLastMob(){
        return mobs.Count >= 1 ? mobs[mobs.Count - 1] : null;
    }
}
