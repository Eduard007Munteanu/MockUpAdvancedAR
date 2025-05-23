using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoundManager : MonoBehaviour{ //Here I will need to call the ticks for every ressource. 


    public static RoundManager Instance {get; private set;}

    private List<EnemyTile> enemyTiles;

    private int roundNumber = 1;

    private bool round1Showed = false;

    private int numberOfEnemiesToSpawn = 1;  //Hardcoded

    private float timeToActivateRound = 60f; 

    private float timer = 0f; 

    private bool timerIncreaser = true; 

    private float roundTime = 30f;

    private float timeToWait = 1f;

    private float tickTimer = 0f;
    private float tickTime = 1f;

    private ResourceDatabase resources;


    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        while (resources == null){
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }
    }



    void Start()
    {
        

        enemyTiles = new List<EnemyTile>();   
    }



    int getRound(){
        return roundNumber;
    }



    void Update(){
        if(timerIncreaser){


            if (roundNumber == 1 && !round1Showed)
            {
                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification("Round: ", $"We are at Round:  {roundNumber}");
                    round1Showed = true;
                }
                else
                {
                    Debug.LogWarning("NotificationManager instance not found. Cannot show 'Arts Advancement' notification.");
                }    
            }
            


            Debug.Log("We are exactly at UpdateTime");
            UpdateTime();
        } else{ // new round
            
            
            Debug.Log("We are at roundnumber: " + roundNumber);
            Debug.Log("We are exactly at SpawnMobs");
            
            SpawnMobs();
        }
    }


    void UpdateTime(){
        timer += Time.deltaTime;
        if(timer >= timeToActivateRound){
            SwitchRoundToSpawnState();
        }

        tickTimer += Time.deltaTime;
        if(tickTimer >= tickTime){
            // Call the tick for every resource here
            resources.Tick();                                             //I comented it out. Leads to error.
            tickTimer = 0f; // Reset the timer after calling the tick
        }
    }

    void SpawnMobs(){
        
        timeToWait -= Time.deltaTime; 
        if(timeToWait <= 0f){

            roundNumber += 1;
            resources[ResourceType.EnemyMight].AddAmount(5f); // increase enemy might each round
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Round: ", $"We are at Round:  {roundNumber}");
            }
            else
            {
                Debug.LogWarning("NotificationManager instance not found. Cannot show 'Arts Advancement' notification.");
            }

            Debug.Log("timeToWait reached 0, attempting spawn...");

            if(enemyTiles.Count == 0){
                Debug.LogError("No enemyTile detected!");
                return;
            }


            EnemyTile randomTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
            // EnemyTile randomTile = enemyTiles[0];
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

        if (timerIncreaser)
        {
            // We just ended a spawn round, reset timer-related stuff
            numberOfEnemiesToSpawn = 1;
            timeToWait = roundTime;
            roundTime -= 2f;
        }
    }








    public void AddEnemyTile(EnemyTile enemyTile){
        enemyTiles.Add(enemyTile);
    }



}