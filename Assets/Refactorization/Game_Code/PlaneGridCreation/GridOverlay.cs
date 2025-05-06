using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{

    public static GridOverlay Instance {get; private set;}


    public GameObject tilePrefab;

    public GameObject enemyTilePrefab;

    public int rows = 2;
    public int columns = 2;

    private List<GameObject> tiles = new List<GameObject>();

    [SerializeField] private GameObject itemManager;


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
        Renderer cubusRenderer = transform.parent.GetComponent<Renderer>();
        Renderer tileRenderer = tilePrefab.GetComponent<Renderer>();

        if (cubusRenderer == null || tileRenderer == null)
        {
            Debug.LogError("Renderer missing on Cubus or tilePrefab.");
            return;
        }

        Vector3 cubusSize = cubusRenderer.bounds.size;
        Vector3 tileSize = tileRenderer.bounds.size;

        Vector3 adjustedTileScale = new Vector3(
            (cubusSize.x / columns) / tileSize.x,
            1,
            (cubusSize.z / rows) / tileSize.z
        );

        // Correct global top surface height
        float globalTopSurfaceY = cubusRenderer.bounds.max.y + 0.01f;

        // Global bottom-left corner (world coordinates)
        Vector3 globalBottomLeft = new Vector3(
            cubusRenderer.bounds.center.x - cubusSize.x / 2 + cubusSize.x / columns / 2,
            globalTopSurfaceY,
            cubusRenderer.bounds.center.z - cubusSize.z / 2 + cubusSize.z / rows / 2
        );

        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                Vector3 globalOffset = new Vector3(
                    x * (cubusSize.x / columns),
                    0,
                    z * (cubusSize.z / rows)
                );

                Vector3 globalSpawnPos = globalBottomLeft + globalOffset;

                // Convert the global position into the local position of the grid GameObject
                Vector3 localSpawnPos = transform.InverseTransformPoint(globalSpawnPos);


                GameObject tile = null;
                if(z == 0){
                    tile = Instantiate(enemyTilePrefab, transform);
                } else {
                    tile = Instantiate(tilePrefab, transform);
                }
                tile.name = $"Tile_{x}_{z}";
                tile.transform.localPosition = localSpawnPos;
                tile.transform.localRotation = Quaternion.identity;
                tile.transform.localScale = adjustedTileScale;

                tiles.Add(tile);
            }
        }

        // materialManager.GetComponent<MaterialManager>().SetTileRenderedOnRunTime(); 
        itemManager.GetComponent<ItemManager>().TilesRendered();


    }

    public Vector3[] GetTileCorners(DefaultTile tile)
    {
        Transform tileTransform = tile.transform;

        Vector3 center = tileTransform.position;
        Vector3 right = tileTransform.right * 0.5f * tileTransform.localScale.x * 10f;
        Vector3 forward = tileTransform.forward * 0.5f * tileTransform.localScale.z * 10f;

        Vector3 topLeft     = center - right + forward;
        Vector3 topRight    = center + right + forward;
        Vector3 bottomLeft  = center - right - forward;
        Vector3 bottomRight = center + right - forward;

        return new Vector3[] { topLeft /*bottomRight from board perspective  */, topRight /*bottomLeft from board perspective */, /*topLeft from board perspective */ bottomRight, /*topRight from board perspective */ bottomLeft };
    }

    public List<GameObject> GetTiles()
    {
        return tiles;
    }

    public (int, int ) GetRowAndColumnsOfPlatform(){
        return (rows, columns);
    }

    public DefaultTile FindTileWithCoordinates(int x, int z){
        foreach (GameObject tile in tiles){
            if(tile == null){
                Debug.Log("tile found to be null");
            }
            if(tile.name == $"Tile_{x}_{z}"){
                return tile.GetComponent<DefaultTile>();
            }
        }
        Debug.LogError("No such tile found!");
        return null;
    }


    public int GetTilesCount(){
        return tiles.Count;
    }



    public DefaultTile GetRandomTile(){
        int randomIndex = Random.Range(0, GetTilesCount());
        DefaultTile randomTile = GetTiles()[randomIndex].GetComponent<DefaultTile>();
        return randomTile;
    }
}
