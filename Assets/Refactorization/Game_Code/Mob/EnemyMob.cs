using Unity.VisualScripting;
using Unity.XR.OpenVR;
using UnityEditor.XR;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UIElements;

public class EnemyMob : MonoBehaviour{

    

    private bool checkCurrentTileIfMobs;

    

    [SerializeField] public float speedFactor = 0.008f;

    private DefaultTile[] allTiles; 

    private DefaultTile lastTile;

    private bool isMoving;

    private DefaultTile lastTileInMyPath;

    public DefaultTile currentTile;  //Not good, but I don't want to add Get/Set methods

    private List<DefaultTile> cachMemoryPathTiles;

    private EnemyTile myCreator;


    private bool hasTarget = false; 
    private Vector3 target;  



    void Start()
    {
        allTiles = FindObjectsOfType<DefaultTile>(); 
        cachMemoryPathTiles = new List<DefaultTile>();
        


      
        int rowLength = GridOverlay.Instance.rows;
        int columnLength = GridOverlay.Instance.columns;



        float currentDistance = float.MaxValue;
        int tileRow = -1;

        for(int i = 0; i < rowLength; i++){
            DefaultTile tile = GridOverlay.Instance.FindTileWithCoordinates(i, rowLength - 1);
            if(Vector3.Distance(transform.position, tile.transform.position) < currentDistance){
                currentDistance = Vector3.Distance(transform.position, tile.transform.position);
                tileRow = i;
                lastTileInMyPath = tile;
            } 
        }



        for(int i=0; i < columnLength; i++){
            DefaultTile tile = GridOverlay.Instance.FindTileWithCoordinates(tileRow, i);
            cachMemoryPathTiles.Add(tile);
        }


        Debug.Log("LastTileInMyPath is " + lastTileInMyPath.name);
        Debug.Log("CachMemoryPathTiles is " + cachMemoryPathTiles);


        isMoving = true; //Just for now.
    }


    void Update()
    {
        Action();
    }



    public void MovementLogic(){
        if(isMoving && !hasTarget){
            Debug.Log("We are at MovementLogic");
            MoveForward();
            UpdateTileState();
            CheckTileFullAndAction();
            EndBoardReached();
         } else if (hasTarget && isMoving){
             MoveToTarget();
             UpdateTileState();
             CheckTileFullAndAction();
         }   
        
    }


    public void MoveForward(){
        if(isMoving){
            Debug.Log("From MoveForward, isMoving is true, so we continue going");
            Vector3 dir = new Vector3(0f,0f,1f);
            transform.position += dir * speedFactor;   
        } else {
            Debug.Log("From MoveForward, isMoving is false, we are stationary for now to check the tiles");
        }
    }

    public void MoveToTarget(){
        if(isMoving){
            target.y = transform.position.y;                                     //I guess that this is correct, we know the y position of the mob. 
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * speedFactor;
            if(Vector3.Distance(transform.position, target) < 0.1f){
                //Position should also be modified, applying the attack to the MainMob
                //It's also important to check the tiles while going to the target MainBuild, because there can be mobs / buildings inbetween. 
                isMoving = false;
                hasTarget = false;
            }
        }
    }


    public void CheckTileFullAndAction(){
        isMoving = false;
        if(checkCurrentTileIfMobs){
            Debug.Log($"Combat triggered on tile {currentTile.name} with {currentTile.GetAmountOfMobs()} mobs.");
            bool canEnemyBePlaced = currentTile.CanEnemyMobsBeArranged(this); 
            if(canEnemyBePlaced){
                currentTile.ArrangeEnemyMobs(this);
                return;
            } else{ 
                Debug.Log("We call TellOurCreatorToNotCreateMoreOfUs");
                TellOurCreatorToNotCreateMoreOfUs();
                return;
            }
        }
        isMoving = true;

    }


    public void TellOurCreatorToNotCreateMoreOfUs(){
        myCreator.StopCreatingMobs();
    }



    private void EndBoardReached(){
        if(Vector3.Distance(transform.position, lastTileInMyPath.transform.position) < 0.1f){
            Debug.Log("We reached the end of the boardGame, boys!");
            transform.position = new Vector3(lastTileInMyPath.transform.position.x, transform.position.y, lastTileInMyPath.transform.position.z);
            //isMoving = false; 
            InitiallyTargetMainBuilding();
            

        }
    }


    private void InitiallyTargetMainBuilding(){
        //cachMemoryPathTiles

        int rowLength = GridOverlay.Instance.rows;
        int columnLength = GridOverlay.Instance.columns;


        cachMemoryPathTiles.Clear();

        for(int i=0; i < rowLength; i++){
            var coordinates = GridOverlay.Instance.FindCoordinatesWithTile(currentTile);
            int columnCoordinate = -1;
            if (coordinates.HasValue)
            {
                columnCoordinate = coordinates.Value.Item2;
            }
            else
            {
                Debug.LogError("Coordinates for the current tile are null.");
            }

            DefaultTile tile = GridOverlay.Instance.FindTileWithCoordinates(i, columnCoordinate);
            cachMemoryPathTiles.Add(tile);
        }

        
        for(int i=0; i < cachMemoryPathTiles.Count; i++){
            DefaultTile checkThisTile = cachMemoryPathTiles[i];
            DefaultBuild checkBuild = checkThisTile.GetBuilding();
            if (checkBuild == null) {
                //Debug.LogError("CheckBuild is actually null");
                continue; 
            }

            string buildClass = checkBuild.GetBuildingClass();
            if (buildClass == null) {
                continue;
            }

            if (buildClass == "Main") {
                Debug.Log("We found the main building");
                hasTarget = true;
                target = checkBuild.transform.position;
            }
            
        }

        isMoving = true;
    }


    private void CheckIfCurrentTileHasDefaultMobs(){
        if(currentTile == null){
            Debug.LogError("CurrentTile is null at CheckIfCurrenTileHasDefaultMobs");
            return;
        }
        int amountOfDefaultMobs = currentTile.GetAmountOfMobs();
        if(amountOfDefaultMobs != 0){
            checkCurrentTileIfMobs = true;
            return;
        }
        checkCurrentTileIfMobs = false;

    }


    public DefaultTile FindClosestTouchingTile()
    {
        DefaultTile closestTouchingTile = null;
        float closestDistance = float.MaxValue;

        Vector3 myPosition = transform.position;
        Bounds myBounds = GetComponent<Collider>().bounds;

        foreach (DefaultTile tile in allTiles)
        {
            if (tile == null) continue;

            Bounds tileBounds = tile.GetComponent<Collider>().bounds;

            // Check if tile is touching or overlapping on XZ plane (ignore Y)
            bool touchingX = Mathf.Abs(tileBounds.center.x - myBounds.center.x) <= (tileBounds.extents.x + myBounds.extents.x);
            bool touchingZ = Mathf.Abs(tileBounds.center.z - myBounds.center.z) <= (tileBounds.extents.z + myBounds.extents.z);

            if (touchingX && touchingZ)
            {
                float dist = Vector2.Distance(new Vector2(myPosition.x, myPosition.z), new Vector2(tile.transform.position.x, tile.transform.position.z));

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTouchingTile = tile;
                }
            }
        }


        Debug.Log("ClosestTouchingFile is " + closestTouchingTile);

        return closestTouchingTile;
    }

    void UpdateTileState() {
        Debug.Log("Combat triggered we UpdateTileState");
        currentTile = FindClosestTouchingTile();
        if(currentTile == null){
            Debug.LogError("Closest touching tile is null");
        }
        if (currentTile != lastTile)
        {
            lastTile = currentTile;
            CheckIfCurrentTileHasDefaultMobs();
        }
    }



    public void Action(){
        MovementLogic();
    } 



    public void HeIsMyCreator(EnemyTile enemyTile){
        myCreator = enemyTile;
    }

}


    


