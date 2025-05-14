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

        

    [SerializeField] public float speedFactor = 0.003f;

    //private DefaultTile[] allTiles;     Probably not usefull

    private DefaultTile lastTile;

    private bool isMoving;

    private DefaultTile lastTileInMyPath;

    public DefaultTile currentTile;  //Not good, but I don't want to add Get/Set methods

    private List<DefaultTile> cachMemoryPathTiles;

    private EnemyTile myCreator;


    private bool hasTarget = false; 
    //private Vector3 target;  

    private DefaultTile targetTile;

    private bool endPointReached = false;



    private float mightPower = 20f;



    void Start()
    {
        //allTiles = FindObjectsOfType<DefaultTile>(); 
        cachMemoryPathTiles = new List<DefaultTile>();
        


      
        int rowLength = BetterGridOverlay.Instance.rows;
        int columnLength = BetterGridOverlay.Instance.columns;



        float currentDistance = float.MaxValue;
        int tileColumn = -1;

        for(int i = 0; i < columnLength; i++){
            DefaultTile tile = BetterGridOverlay.Instance.FindTileWithCoordinates(rowLength - 1, i);
            if(Vector3.Distance(transform.position, tile.transform.position) < currentDistance){
                currentDistance = Vector3.Distance(transform.position, tile.transform.position);
                tileColumn = i;
                lastTileInMyPath = tile;
            } 
        }



        for(int i=0; i < rowLength; i++){
            DefaultTile tile = BetterGridOverlay.Instance.FindTileWithCoordinates(i, tileColumn);
            cachMemoryPathTiles.Add(tile);
        }


        Debug.Log("LastTileInMyPath at ENEMYMOB is " + lastTileInMyPath.name);
        //Debug.Log("CachMemoryPathTiles at ENEMYMOB is " + cachMemoryPathTiles);
        foreach(DefaultTile enemyMOB in cachMemoryPathTiles){
            Debug.Log("CachMemoryPathTiles at ENEMYMOB is " + enemyMOB);
        }


        isMoving = true; //Just for now.
    }


    void Update()
    {
        Action();
    }

    public float GetMightPower(){
        return mightPower;
    }

    public void SetMightPower(float changedMight){
        mightPower = changedMight;
    }



    public void SetMoving(bool setToWhat){
        isMoving = setToWhat;
    }



    public void MovementLogic(){
        Debug.Log("At MovementLogic: IsMoving from MovementLogic is " + isMoving);
        Debug.Log("At MovementLogic: Has target is " + hasTarget);
        Debug.Log("At MovementLogic: end point reached is " + endPointReached);
        if(isMoving && !hasTarget && !endPointReached){
            Debug.Log("We are at MovementLogic");
            MoveForward();
            UpdateTileState();
            CheckTileFullAndAction();
            EndBoardReached();
         } else if (hasTarget && isMoving & !endPointReached){
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
            Vector3 target = targetTile.gameObject.transform.position;
            target.y = transform.position.y;                                     //I guess that this is correct, we know the y position of the mob. 
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * speedFactor;
            if(Vector3.Distance(transform.position, target) < 0.1f){
                //Position should also be modified, applying the attack to the MainMob
                //It's also important to check the tiles while going to the target MainBuild, because there can be mobs / buildings inbetween. 
                isMoving = false;
                hasTarget = false;
                endPointReached = true;

                Debug.Log("We reached the distance, stop!");
                targetTile.ArrangeEnemyMobs(this);
            }
        }
    }


    public void CheckTileFullAndAction(){
        isMoving = false;
        if(checkCurrentTileIfMobs){
            Debug.Log($"Combat triggered on tile {currentTile.name} with {currentTile.GetAmountOfMobs()} mobs.");
            bool canEnemyBePlaced = currentTile.CanEnemyMobsBeArrangedChecker(); 
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
        Debug.Log("Distance between current enemy mob and lastTileInMyPath is of " + Vector3.Distance(transform.position, lastTileInMyPath.transform.position));
        if(Vector3.Distance(transform.position, lastTileInMyPath.transform.position) < 0.1f){
            Debug.Log("We reached the end of the boardGame, boys!");
            transform.position = new Vector3(lastTileInMyPath.transform.position.x, transform.position.y, lastTileInMyPath.transform.position.z);
            //isMoving = false; 
            InitiallyTargetMainBuilding();
            

        }
    }


    private void InitiallyTargetMainBuilding(){
        Debug.Log("We are at the method InitiallyTargetMainBuilding");
        //cachMemoryPathTiles

        int rowLength = BetterGridOverlay.Instance.rows;
        int columnLength = BetterGridOverlay.Instance.columns;


        cachMemoryPathTiles.Clear();


        var coordinates = BetterGridOverlay.Instance.FindCoordinatesWithTile(lastTileInMyPath);
        int x = -1;
        if (coordinates.HasValue)
        {
            x = coordinates.Value.Item1;
        }
        else
        {
            Debug.LogError("Coordinates for the last tile in my path are null.");
        }

        for(int i=0; i < columnLength; i++){
            DefaultTile tile = BetterGridOverlay.Instance.FindTileWithCoordinates(x,  i );
            cachMemoryPathTiles.Add(tile);
        }



        //Debug.Log("CachMemoryPathTiles at InitiallyTargetMainBuilding is " + cachMemoryPathTiles);

        foreach(DefaultTile tile in cachMemoryPathTiles){
            Debug.Log("CachMemoryPathTiles at InitiallyTargetMainBuilding is " + tile);
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
                targetTile = checkBuild.GetTile();//checkBuild.transform.position;

                // Rotate towards the center of the target tile
                Vector3 targetPosition = targetTile.GetComponent<Renderer>().bounds.center;
                Vector3 enemyPosition = transform.position;

                Vector3 direction = (targetPosition - enemyPosition).normalized;

                // Optional: lock y-axis rotation only
                direction.y = 90f;

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = lookRotation;
                }
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

    private void CheckIfCurrentTileHasBuilding(){
        if(currentTile.GetBuilding() != null){
            Debug.Log("We checked if current tile has building, and it had");
            checkCurrentTileIfMobs = true;
        } else {
            checkCurrentTileIfMobs = false;
        }
        
    }


    public DefaultTile FindClosestTouchingTile()
    {
        DefaultTile closestTouchingTile = null;
        float closestDistance = float.MaxValue;

        Vector3 myPosition = transform.position;
        Bounds myBounds = GetComponent<Collider>().bounds;

        foreach (DefaultTile tile in cachMemoryPathTiles)//allTiles)
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
        CheckIfCurrentTileHasBuilding();
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


    


