using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour{ //Here I will need to call the ticks for every ressource. 


    public static RoundManager Instance {get; private set;}

    private List<EnemyTile> enemyTiles;

    private int numberOfEnemiesToSpawn = 25;  //Hardcoded

    private float timeToActivateRound = 5f; 

    private float timer = 0f; 

    private bool timerIncreaser = true; 

    private float timeToWait = 1f;


    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }



    void Start()
    {
        enemyTiles = new List<EnemyTile>();   
    }



    void Update(){
        if(timerIncreaser){
            Debug.Log("We are exactly at UpdateTime");
            UpdateTime();
        } else{
            Debug.Log("We are exactly at SpawnMobs");
            SpawnMobs();
        }
    }


    void UpdateTime(){
        timer += Time.deltaTime;
        if(timer >= timeToActivateRound){
            SwitchRoundToSpawnState();
        }
    }

    void SpawnMobs(){
        
        timeToWait -= Time.deltaTime; 
        if(timeToWait <= 0f){
            Debug.Log("timeToWait reached 0, attempting spawn...");

            if(enemyTiles.Count == 0){
                Debug.LogError("No enemyTile detected!");
                return;
            }


            EnemyTile randomTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
            Debug.Log("Spawning from: " + randomTile.name);
            randomTile.SetCreateMobs(true);

            numberOfEnemiesToSpawn --;
            Debug.Log("NumberOfEnemiesToSpawn" + numberOfEnemiesToSpawn);
            timeToWait = Random.Range(0,5);
        }
        if(numberOfEnemiesToSpawn == 0){
            timer = 0f;
            SwitchRoundToSpawnState();
        }   
    }

    void SwitchRoundToSpawnState(){
        timerIncreaser = !timerIncreaser;

        if (timerIncreaser) {
        // We just ended a spawn round, reset timer-related stuff
        numberOfEnemiesToSpawn = 25;
        timeToWait = 1f;
    }
    }








    public void AddEnemyTile(EnemyTile enemyTile){
        enemyTiles.Add(enemyTile);
    }



}