using Unity.VisualScripting;
using UnityEngine;

public class EnemyTile : DefaultTile
{

    [SerializeField] private EnemyMob enemyMobPrefab;


    private float spawnTimer = 0f;
    private float spawnInterval = 10f;


    


    void Update()
    {
        spawnTimer += Time.deltaTime;

        if(spawnTimer >= spawnInterval){
            SpawnEnemyMobAtSomePointInTime();
            spawnTimer = 0f;
        }
    }


    public void SpawnEnemyMobAtSomePointInTime(){
        float mobHeight = enemyMobPrefab.gameObject.GetComponent<Renderer>().bounds.size.y;  //move to start, but carefull about overriding. 
        float spawnPositionY = transform.position.y + (GetTileHeight() + (mobHeight / 2)); 
        Vector3 spawnedMobPosition = new Vector3(transform.position.x, spawnPositionY  ,transform.position.z);

        float spawnThreshold = randomGenerationFloat();  //Can be modified given other factors from the game!
    

        if(spawnThreshold > 0.5){
            GameObject enemyMobObj = Instantiate(enemyMobPrefab.gameObject, spawnedMobPosition, Quaternion.identity);
            Debug.Log("Yeah, we added the enemyMob brothers!");
        }


        //Let's, to begin with, can be discussed, spawn given threshold met.


    }


    private float randomGenerationFloat(){
        return Random.value;
    }



    public override bool CanMobBeArrangedChecker(DefaultMob mob){ 
        return false;
    }

}