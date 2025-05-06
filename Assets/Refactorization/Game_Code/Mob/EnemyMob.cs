using Unity.VisualScripting;
using Unity.XR.OpenVR;
using UnityEditor.XR;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class EnemyMob : MonoBehaviour{

    

    private bool checkCurrentTileIfMobs;

    public DefaultTile currentTile;  //Not good, but I don't want to add Get/Set methods

    [SerializeField] public float speedFactor = 0.008f;

    private DefaultTile[] allTiles; 

    private DefaultTile lastTile;

    private bool isMoving;

    private DefaultTile lastTileInMyPath;



    void Start()
    {
        allTiles = FindObjectsOfType<DefaultTile>(); 
        


      
        int rowLength = GridOverlay.Instance.rows;



        float currentDistance = float.MaxValue;

        for(int i = 0; i < rowLength; i++){
            DefaultTile tile = GridOverlay.Instance.FindTileWithCoordinates(i, rowLength - 1);
            if(Vector3.Distance(transform.position, tile.transform.position) < currentDistance){
                currentDistance = Vector3.Distance(transform.position, tile.transform.position);
                lastTileInMyPath = tile;
            }
            
        }


        Debug.Log("LastTileInMyPath is " + lastTileInMyPath.name);


        isMoving = true; //Just for now.
    }


    void Update()
    {
        if(isMoving){
            Action();
        }
    }





    public void MoveForward(){

        

        Vector3 dir = new Vector3(0f,0f,1f);
        transform.position += dir * speedFactor;


        UpdateTileState();
        if(checkCurrentTileIfMobs){
            Debug.Log("Well, this tile has mobs, bro!");
            isMoving = false;
            currentTile.ArrangeEnemyMobs(this);
            
        }

        EndBoardReached();


        
    }



    


    private void EndBoardReached(){
        if(Vector3.Distance(transform.position, lastTileInMyPath.transform.position) < 0.1f){
            Debug.Log("We reached the end of the boardGame, boys!");
            transform.position = new Vector3(lastTileInMyPath.transform.position.x, transform.position.y, lastTileInMyPath.transform.position.z);
            isMoving = false; 
        }
    }


    private void CheckIfCurrentTileHasDefaultMobs(){
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
        if (currentTile != lastTile)
        {
            lastTile = currentTile;
            CheckIfCurrentTileHasDefaultMobs();
        }
    }



    public void Action(){
        MoveForward();
    } 






}


    


