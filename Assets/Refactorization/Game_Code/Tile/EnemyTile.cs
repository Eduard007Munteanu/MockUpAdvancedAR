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
        Renderer tileRenderer = GetComponent<Renderer>();
        Renderer mobRenderer = enemyMobPrefab.GetComponent<Renderer>();

        float tileTopY = tileRenderer.bounds.max.y;

        float bottomOffset = mobRenderer.bounds.min.y - enemyMobPrefab.transform.position.y;

        float spawnY = tileTopY - bottomOffset;

        Vector3 spawnedMobPosition = new Vector3(transform.position.x, spawnY, transform.position.z);




    
        EnemyMob enemyMob = Instantiate(enemyMobPrefab, spawnedMobPosition, Quaternion.identity);
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