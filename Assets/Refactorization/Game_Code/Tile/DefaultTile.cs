using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultTile : MonoBehaviour//, Tile   //This guy shuold know about the power level of the mobs and the enemy
{


    private List<DefaultMob> mobs = new List<DefaultMob>();

    private List<EnemyMob> enemyMobs = new List<EnemyMob>();

    private DefaultBuild buildingOnTile;

    private Fighting fighting;

    private float timer = 0f;

    private float activator = 10f; 


    


    // Start is called before the first frame update
    void Start()
    {
        SurfaceArea();
    }

    // Update is called once per frame
    void Update()
    {
        if((mobs.Count > 0 && enemyMobs.Count > 0) || (enemyMobs.Count > 0) && buildingOnTile != null ){
            timer += Time.deltaTime;
            if(timer >= activator){
                FightingActivation();
                timer = 0f;
            }
            
        }
    }



    public (float, float) SurfaceArea(){
        Renderer tileRenderer = GetComponent<Renderer>();
        Vector3 surfaceSize = tileRenderer.bounds.size;

        float surfaceWidth = surfaceSize.x;
        float surfaceDepth = surfaceSize.z;


        Debug.Log("Surfacearea of the tile has surfaceWidth being: " + surfaceWidth + " and surfaceDepth being " + surfaceDepth);

        return (surfaceWidth, surfaceDepth);
    }


    public float ScalingTheObjects(Renderer objectRenderer, float n_factor){
        Vector3 currentSize = objectRenderer.bounds.size;

        (float surfaceWidth, float surfaceDepth) = SurfaceArea();
        // float desiredWidth = surfaceWidth / n_factor;                                   // Hardcoded
        // float scaleFactor = desiredWidth / currentSize.x;

        float desiredSize = Mathf.Min(surfaceWidth, surfaceDepth) / n_factor;
        float scaleFactor = desiredSize / Mathf.Max(currentSize.x, currentSize.z);

        return scaleFactor;

    }








    void FightingActivation(){
        Debug.Log("FightingActivation here!");
        
        fighting = new Fighting(enemyMobs, mobs, GetBuilding());
        (List<DefaultMob> updateMobs, List<EnemyMob> updateEnemyMobs, DefaultBuild theBuilding) = fighting.SimulateFighting();


        buildingOnTile = theBuilding;



        mobs.Clear();
        mobs.AddRange(updateMobs);

        enemyMobs.Clear();
        enemyMobs.AddRange(updateEnemyMobs);




        Debug.Log("UpdateEnemyMobs count is " + updateEnemyMobs.Count);
        Debug.Log("enemyMobs count is " + enemyMobs.Count);


        fighting.TriggerMoveToAllEnemy(enemyMobs);

    }












    public void AddBuilding(DefaultBuild building){
        buildingOnTile = building;
    }

    public DefaultBuild GetBuilding(){
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
        if(mob != null){
            mobs.Add(mob); // Always add the mob first
        }
        

        Vector3[] tileCorners = BetterGridOverlay.Instance.GetTileCorners(this);
        Vector3 topLeft = tileCorners[0];
        Vector3 topRight = tileCorners[1];

        Vector3 xDir = (topRight - topLeft).normalized;
        float tileWidth = Vector3.Distance(topLeft, topRight);

        // Measure total required width
        float totalWidth = 0f;
        List<float> mobWidths = new();

        foreach (var m in mobs)
        {
            float width = m.GetComponent<Renderer>().bounds.size.x;
            mobWidths.Add(width);
            totalWidth += width;
        }

        if (totalWidth > tileWidth)
        {
            Debug.LogWarning($"Not enough space to place {mobs.Count} mobs. Required: {totalWidth}, Available: {tileWidth}");
            if(mob != null){
                mobs.Remove(mob); // Roll back
            }
            return;
        }



        Vector3 cursor = Vector3.zero; // Initialize cursor with a default value
        if(buildingOnTile != null){
            Renderer buildingOnTileRenderer = buildingOnTile.GetComponent<Renderer>();
            float maxZPositionOfBuilding = buildingOnTileRenderer.bounds.size.z;
            float mobZ;
            if(mob == null){
                if(mobs.Count > 0){
                    mobZ = mobs[0].GetComponent<Renderer>().bounds.size.z;
                } else {
                    mobZ = 0;
                }
            } else{
                mobZ = mob.GetComponent<Renderer>().bounds.size.z;
            }
            


            float offsetFromBuilding = maxZPositionOfBuilding / 2f + mobZ;  //HARDCODED
            Vector3 tileCenter = (topLeft + topRight) * 0.5f;
            Vector3 groupStart = tileCenter - new Vector3(0, 0, (maxZPositionOfBuilding / 2f + offsetFromBuilding));
            cursor = groupStart - xDir * (totalWidth / 2f);
            Debug.Log("There is a building on tile");
            //float differenceBetweenMinMaxZTile = GetComponent<Renderer>().bounds.max.z - GetComponent<Renderer>().bounds.min.z;
            

        } else if(buildingOnTile == null){
            Vector3 tileCenter = (topLeft + topRight) * 0.5f;
            Vector3 groupStart = tileCenter - xDir * (totalWidth / 2f);

            // Place all mobs side by side starting from topLeft
            cursor = groupStart;//topLeft;
            Debug.Log("No building on tile");
        }
        

        for (int i = 0; i < mobs.Count; i++)
        {
            float width = mobWidths[i];
            Vector3 offset = xDir * (width / 2f);
            Vector3 spawnPos = cursor + offset;
            spawnPos.y = mobs[i].transform.position.y;

            mobs[i].transform.position = spawnPos;
            mobs[i].currentTile = this;

            // Advance the cursor by full width
            cursor += xDir * width;
        }


    }

    public virtual bool CanMobBeArrangedChecker(){                               
        List<DefaultMob> theMobs = new List<DefaultMob>(mobs);

        if(theMobs.Count == 0){  //You can arrange if no mob on tile. 
            return true;
        }

        DefaultMob mob = theMobs[0];

        theMobs.Add(mob); 

        Vector3[] tileCorners = BetterGridOverlay.Instance.GetTileCorners(this);
        Vector3 topLeft = tileCorners[0];
        Vector3 topRight = tileCorners[1];

        Vector3 xDir = (topRight - topLeft).normalized;
        float tileWidth = Vector3.Distance(topLeft, topRight);

       
        float totalWidth = 0f;
        List<float> mobWidths = new();

        foreach (var m in theMobs)
        {
            float width = m.GetComponent<Renderer>().bounds.size.x;
            mobWidths.Add(width);
            totalWidth += width;
        }

        if (totalWidth > tileWidth)
        {
            Debug.LogWarning($"Not enough space to place {theMobs.Count} mobs. Required: {totalWidth}, Available: {tileWidth}");
            //theMobs.Remove(mob); 
            return false;
        }
        //theMobs.Remove(mob);
        return true;
    }


    public void ArrangeEnemyMobs(EnemyMob enemy){
        enemyMobs.Add(enemy); // Always add the mob first

        Vector3[] tileCorners = BetterGridOverlay.Instance.GetTileCorners(this);
        Vector3 bottomLeft = tileCorners[3];
        Vector3 bottomRight = tileCorners[2];

        Vector3 xDir = (bottomRight - bottomLeft).normalized;
        float tileWidth = Vector3.Distance(bottomLeft, bottomRight);

        // Measure total required width
        float totalWidth = 0f;
        List<float> mobWidths = new();

        foreach (var m in enemyMobs)
        {
            float width = m.GetComponent<Renderer>().bounds.size.x;
            mobWidths.Add(width);
            totalWidth += width;
        }

        if (totalWidth > tileWidth)
        {
            Debug.LogWarning($"Not enough space to place {mobs.Count} mobs. Required: {totalWidth}, Available: {tileWidth}");
            enemyMobs.Remove(enemy); // Roll back
            return;
        }



        Vector3 tileCenter = (bottomLeft + bottomRight) * 0.5f;
        Vector3 groupStart = tileCenter - xDir * (totalWidth / 2f);

        // Place all mobs side by side starting from topLeft
        Vector3 cursor = groupStart;//topLeft;
        Debug.Log("No building on tile");
        
        

        for (int i = 0; i < enemyMobs.Count; i++)
        {
            float width = mobWidths[i];
            Vector3 offset = xDir * (width / 2f);
            Vector3 spawnPos = cursor + offset;
            spawnPos.y = enemyMobs[i].transform.position.y;

            enemyMobs[i].transform.position = spawnPos;
            enemyMobs[i].currentTile = this;

            // Advance the cursor by full width
            cursor += xDir * width;
        }
    }

    public virtual bool CanEnemyMobsBeArrangedChecker(){                               
        List<EnemyMob> theMobs = new List<EnemyMob>(enemyMobs);

        if(theMobs.Count == 0){  //You can arrange if no mob on tile. 
            return true;
        }

        EnemyMob mob = theMobs[0];

        theMobs.Add(mob); 

        Vector3[] tileCorners = BetterGridOverlay.Instance.GetTileCorners(this);
        Vector3 bottomLeft = tileCorners[3];
        Vector3 bottomRight = tileCorners[2];

        Vector3 xDir = (bottomRight - bottomLeft).normalized;
        float tileWidth = Vector3.Distance(bottomLeft, bottomRight);

       
        float totalWidth = 0f;
        List<float> mobWidths = new();

        foreach (var m in theMobs)
        {
            float width = m.GetComponent<Renderer>().bounds.size.x;
            mobWidths.Add(width);
            totalWidth += width;
        }

        if (totalWidth > tileWidth)
        {
            Debug.LogWarning($"Not enough space to place {theMobs.Count} mobs. Required: {totalWidth}, Available: {tileWidth}");
            //theMobs.Remove(mob); 
            return false;
        }
        //theMobs.Remove(mob);
        return true;
    }






    public void RemoveMob(DefaultMob mob){
        mobs.Remove(mob);
    }


    public bool CheckIfMobOnTile(DefaultMob mob){
        return mobs.Contains(mob);
    }





    


    public List<DefaultMob> GetListOfMobs(){
        return mobs;
    }

    public int GetAmountOfMobs(){
        if (this == null) {
            Debug.LogError("Tile object is null.");
        }
        if (mobs == null) {
            Debug.LogError("mobs list is null.");
        }
        return mobs.Count;
    }


    private DefaultMob GetLastMob(){
        return mobs.Count >= 1 ? mobs[mobs.Count - 1] : null;
    }
}
