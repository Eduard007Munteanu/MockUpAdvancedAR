using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultTile : MonoBehaviour//, Tile   //This guy shuold know about the power level of the mobs and the enemy
{


    private List<DefaultMob> mobs;

    private List<EnemyMob> enemyMobs;

    private DefaultBuild buildingOnTile;


    // Start is called before the first frame update
    void Start()
    {
        mobs = new List<DefaultMob>();
        enemyMobs = new List<EnemyMob>();
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


    public void ArrangeEnemyMobs(EnemyMob mob){  //Very similar to ArrangeMobs, maybe refactor that part
        Vector3[] tileCorners = GridOverlay.Instance.GetTileCorners(this);
        Vector3 bottomLeft = tileCorners[3];
        Vector3 bottomRight = tileCorners[2];
        Vector3 topRight = tileCorners[1];
        Vector3 topLeft = tileCorners[0];

        int maxCols = 5;

        Vector3 xDir = (bottomRight - bottomLeft).normalized;  // Move right
        Vector3 zDir = (topLeft - bottomLeft).normalized;      // Move forward (upward visually)

        float tileWidth = Vector3.Distance(bottomLeft, bottomRight);
        float tileHeight = Vector3.Distance(bottomLeft, topLeft);
        float colSpacing = tileWidth / maxCols;

        int mobIndex = enemyMobs.Count;
        int col = mobIndex % maxCols;
        int row = mobIndex / maxCols;

        float rowHeight = tileHeight / 3f;

        if (row * colSpacing > rowHeight)
        {
            Debug.LogWarning("No more vertical space to place mobs!");
        }

        Vector3 spawnPos = bottomLeft + (xDir * colSpacing * col) + (zDir * colSpacing * row);
        spawnPos.y = mob.transform.position.y;

        mob.transform.position = spawnPos;
        mob.currentTile = this;
        enemyMobs.Add(mob);
    }




    public void RemoveMob(DefaultMob mob){
        mobs.Remove(mob);
    }





    


    public List<DefaultMob> GetListOfMobs(){
        return mobs;
    }

    public int GetAmountOfMobs(){
        return mobs.Count;
    }

    private DefaultMob GetLastMob(){
        return mobs.Count >= 1 ? mobs[mobs.Count - 1] : null;
    }
}
