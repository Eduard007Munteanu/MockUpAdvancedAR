using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit.SceneDecorator;
using Unity.VisualScripting;
using UnityEngine;

public class MainBuild : DefaultBuild//, Build
{


    [SerializeField] private GridOverlay gridOverlay;


    [SerializeField] private GameObject PanelPrefab;

    private Tile tile;

    private int Id;

    private string building_class;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init(int Id, string building_class, Tile tile = null){
        this.Id = Id;
        this.building_class = building_class;

    }

    public override Vector3 SpawnBuilding(){
        tile = TileFindCalculation();
        float tileHeight = tile.GetTileHeight();
        float buildingHeight = GetComponent<Renderer>().bounds.size.y;
        Vector3 tilePosition = ((MonoBehaviour)tile).transform.position;  //Given tile is a object



        Vector3 spawnPosition = tilePosition + Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);
        return spawnPosition;
    }

    public Tile TileFindCalculation(){
        (int x, int z) = gridOverlay.GetRowAndColumnsOfPlatform();
        (int, int) buildingSpawnPosition = (z-1, Mathf.FloorToInt(x/2));
        Tile tile = gridOverlay.FindTileWithCoordinates(buildingSpawnPosition.Item1, buildingSpawnPosition.Item2);
        return tile;


    }

    

    
}
