using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTile : DefaultTile
{

    [SerializeField] private EnemyMob enemyMobPrefab;


    private bool createMobs = false;

    private bool canCreateMobs = true;

    private RoundManager roundManager; 

    private bool oneMobOnlyActive = false;





    void Start()
    {
        roundManager = RoundManager.Instance; 
        AddMyselfToRoundManager();
    }



    public void SetCreateMobs(bool value){
        if(canCreateMobs){
            oneMobOnlyActive = true;
            createMobs = value;
        }
    }


    void Update()
    {
        
        if(createMobs && oneMobOnlyActive){
            SpawnEnemyMobAtSomePointInTime();
            oneMobOnlyActive = false;
        }
        
    }


    public void SpawnEnemyMobAtSomePointInTime(){
        float mobHeight = enemyMobPrefab.gameObject.GetComponent<Renderer>().bounds.size.y;  //move to start, but carefull about overriding. 
        float spawnPositionY = transform.position.y + (GetTileHeight() + (mobHeight / 2)); 
        Vector3 spawnedMobPosition = new Vector3(transform.position.x, spawnPositionY  ,transform.position.z);

    
        EnemyMob enemyMob = Instantiate(enemyMobPrefab, spawnedMobPosition, enemyMobPrefab.transform.rotation);//Quaternion.identity);
        enemyMob.HeIsMyCreator(this);
        Debug.Log("Yeah, we added the enemyMob brothers!");



    }



    public override bool CanMobBeArrangedChecker(DefaultMob mob){ 
        return false;
    }

    public void StopCreatingMobs(){
        createMobs = false;
        canCreateMobs = false;

    }


    private void AddMyselfToRoundManager(){
        if(roundManager == null){
            Debug.LogError("RoundManager is actually null!");
            return;
        }
        roundManager.AddEnemyTile(this);
    }

}