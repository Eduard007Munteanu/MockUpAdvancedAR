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
        if(isMoving){
            Debug.Log("We are at MovementLogic");
            MoveForward();
            UpdateTileState();
            CheckTileFullAndAction();
            EndBoardReached();
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


    public void CheckTileFullAndAction(){
        isMoving = false;
        if(checkCurrentTileIfMobs){
            bool canEnemyBePlaced = currentTile.CanEnemyMobsBeArranged(this); 
            if(canEnemyBePlaced){
                currentTile.ArrangeEnemyMobs(this);
                return;
            } else{ 
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
            isMoving = false; 
        }
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


    


