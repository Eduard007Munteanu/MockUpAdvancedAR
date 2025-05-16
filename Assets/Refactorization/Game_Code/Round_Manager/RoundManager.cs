using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoundManager : MonoBehaviour{ //Here I will need to call the ticks for every ressource. 


    public static RoundManager Instance {get; private set;}

    private List<EnemyTile> enemyTiles;

    private int roundNumber = 1;

    private int numberOfEnemiesToSpawn = 1;  //Hardcoded

    private float timeToActivateRound = 30f; 

    private float timer = 0f; 

    private bool timerIncreaser = true; 

    private float timeToWait = 1f;

    private float tickTimer = 0f;
    private float tickTime = 1f;

    private ResourceDatabase resources;

    private AudioManager audioManager;
    private bool boss = false;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one RoundManager detected. Destroying duplicate.");
            Destroy(gameObject);
            return; // Return early to prevent further execution on duplicate
        }
        Instance = this;

        // It's generally safer to get instances in Start or check for null before use,
        // rather than using blocking while loops in Awake.
        // For now, assuming they will be available by Start or first Update.
    }

    void Start()
    {
        enemyTiles = new List<EnemyTile>();
        // Attempt to get instances here if not already set, or rely on them being set by their own Awake.
        if (resources == null)
        {
            resources = ResourceDatabase.Instance;
            if (resources == null)
            {
                Debug.LogError("ResourceDatabase.Instance is null in Start. Ticking will fail.");
            }
        }
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
            if (audioManager == null)
            {
                Debug.LogWarning("AudioManager.Instance is null in Start.");
            }
        }

        // Initialize roundNumber to 0 if you want the first round to be "Round 1"
        // If roundNumber starts at 1, the first spawning round will be "Round 2".
        // roundNumber = 0; // Uncomment if first round should be 1
    }

    void Update(){
        if(timerIncreaser){
            // Debug.Log("State: UpdateTime. Round: " + roundNumber + ". Timer: " + timer);
            UpdateTime();
        } else{
            // Debug.Log("State: SpawnMobs. Round: " + roundNumber + ". Enemies to spawn: " + numberOfEnemiesToSpawn);
            SpawnMobs();
        }
    }

    void UpdateTime(){
        timer += Time.deltaTime;
        if(timer >= timeToActivateRound){
            SwitchRoundToSpawnState(); // This will flip timerIncreaser to false for spawning
        }

        tickTimer += Time.deltaTime;
        if(tickTimer >= tickTime){
            if (resources != null) {
                resources.Tick();
            } else {
                Debug.LogWarning("ResourceDatabase not available for Tick in UpdateTime.");
                // Attempt to re-acquire if it was missing
                resources = ResourceDatabase.Instance; 
            }
            tickTimer = 0f;
        }
    }

    void SpawnMobs(){
        if (numberOfEnemiesToSpawn <= 0) {
            // This should ideally not be hit if the state logic is correct,
            // as SwitchRoundToSpawnState should be called when numberOfEnemiesToSpawn reaches 0.
            // This acts as a safeguard.
            if (!timerIncreaser) { // If we are in spawn state but shouldn't be
                 Debug.LogWarning("SpawnMobs called with numberOfEnemiesToSpawn <= 0 for Round " + roundNumber + ". Forcing switch to timer state.");
                 SwitchRoundToSpawnState(); // This will flip timerIncreaser to true
            }
            return;
        }
        
        timeToWait -= Time.deltaTime; 
        if(timeToWait <= 0f){
            Debug.Log("Round " + roundNumber + ": timeToWait reached 0, attempting spawn...");

            if(enemyTiles == null || enemyTiles.Count == 0){
                Debug.LogError("No enemyTile detected or enemyTiles list is null! Cannot spawn mobs for Round " + roundNumber + ".");
                // Optionally, end the round prematurely if no spawn points exist
                // SwitchRoundToSpawnState(); 
                return;
            }

            EnemyTile randomTile = enemyTiles[Random.Range(0, enemyTiles.Count)];
            Debug.Log("Spawning mob for Round " + roundNumber + " from: " + randomTile.name);
            randomTile.SetCreateMobs(true);

            numberOfEnemiesToSpawn --;
            Debug.Log("Round " + roundNumber + ": Enemies remaining to spawn: " + numberOfEnemiesToSpawn);
            
            if (numberOfEnemiesToSpawn <= 0) { // Check after decrementing
                // All enemies for this round have been spawned.
                Debug.Log("All enemies for Round " + roundNumber + " spawned.");
                
                // End-of-round actions
                if (resources != null) {
                    resources[ResourceType.EnemyMight].AddAmount(1f);
                    if (roundNumber >= 300 && !boss) { // Assuming 'boss' is a class member: private bool boss = false;
                        resources[ResourceType.EnemyMight].AddAmount(90f);
                        boss = true;
                    }
                } else {
                    Debug.LogWarning("ResourceDatabase not available for EnemyMight update at end of Round " + roundNumber);
                }

                if (audioManager != null) {
                    audioManager.PlayTheme();
                } else {
                    Debug.LogWarning("AudioManager not available at end of Round " + roundNumber);
                }
                NotificationManager.Instance.ShowNotification("Round " + roundNumber + " Complete!", "Prepare for the next wave...");
                
                SwitchRoundToSpawnState(); // End spawn phase, switch to timer phase
            } else {
                // More enemies to spawn in this round
                timeToWait = Random.Range(0f, 5f); // Reset time for next spawn in this round
                Debug.Log("Round " + roundNumber + ": Next spawn in " + timeToWait + " seconds.");
            }
        }
    }

    void SwitchRoundToSpawnState(){
        timerIncreaser = !timerIncreaser; // Toggle the state

        if (timerIncreaser) {
            // Just switched from Spawning (false) to Timer (true). Spawn round has ENDED.
            Debug.Log("Switching to Timer state. Round " + roundNumber + " ended. Preparing for next round's timer phase.");
            // Prepare for the *next* spawn round (after the timer phase).
            // Example: increase enemies based on round number.
            // numberOfEnemiesToSpawn = 1 + (roundNumber / 5); 
            numberOfEnemiesToSpawn = 1;  // Reset to 1 as per original logic for now.
            timeToWait = 1f;             // Initial delay before first spawn of next round.
        } else {
            // Just switched from Timer (true) to Spawning (false). New spawn round is STARTING.
            roundNumber += 1; // Increment round number when a new spawn round begins.
            timer = 0f;       // Reset the main round timer.
            // Ensure numberOfEnemiesToSpawn is set for the round that is about to start.
            // It would have been set when the *previous* spawn round ended and timerIncreaser became true.
            Debug.Log("Switching to Spawn state. Starting Round: " + roundNumber + ". Spawning " + numberOfEnemiesToSpawn + " enemies.");
            NotificationManager.Instance.ShowNotification("Round " + roundNumber, "Brace yourself..");
            // Any other setup for the start of the spawn round can go here.
        }
    }

    public void AddEnemyTile(EnemyTile enemyTile){
        if (enemyTiles == null) {
            enemyTiles = new List<EnemyTile>();
        }
        if (enemyTile != null && !enemyTiles.Contains(enemyTile)) {
            enemyTiles.Add(enemyTile);
            Debug.Log("Added EnemyTile: " + enemyTile.name);
        }
    }

}