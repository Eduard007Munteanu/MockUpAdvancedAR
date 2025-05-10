using System.Collections.Generic;
using UnityEngine;

public class BetterGridOverlay : MonoBehaviour
{
    
    public static BetterGridOverlay Instance { get; private set; }

    public GameObject tilePrefab;

    public GameObject enemyTilePrefab;

    public int rows = 5;
    public int columns = 5;

    private List<GameObject> tiles = new List<GameObject>();


    [SerializeField] private GameObject itemManager; 



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one GridOverlay detected. Destroying duplicate.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }




    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        Bounds b = r.bounds;
        Vector3 min = b.min;
        Vector3 max = b.max;  //Added
        Vector3 size = b.size;

        float h = tilePrefab.transform.localScale.y;
        float w = size.x / columns;
        float d = size.z / rows;

        for (int x = 0; x < columns; x++)
        for (int z = 0; z < rows; z++)
        {
            Vector3 worldPos = new Vector3(
            min.x + w * (x + 0.5f),
            max.y + h * 0.5f,                     //Added
            min.z + d * (z + 0.5f)
            );


            GameObject tile;
            if(z == 0){
                tile = Instantiate(enemyTilePrefab);
            } else {
                tile = Instantiate(tilePrefab);
            }
            
            tile.name = $"Tile_{z}_{x}";                               //Such that tile row and columns are correct. 
            tile.transform.position   = worldPos;
            tile.transform.localScale = new Vector3(w, h, d);

            
            tile.transform.SetParent(transform, true);

            tiles.Add(tile);
        }

        GetComponent<MeshRenderer>().enabled = false;

        //Notify itemManager
        if (itemManager != null)
            itemManager.GetComponent<ItemManager>().TilesRendered();


        Debug.Log("Number of tiles " + tiles.Count);

    }




    public Vector3[] GetTileCorners(DefaultTile tile)
    {
        var rend = tile.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("Renderer missing on tile.");
            return new Vector3[0];
        }

        Vector3 center = rend.bounds.center;
        Vector3 ext = rend.bounds.extents;
        return new Vector3[]
        {
            center + new Vector3(-ext.x, 0, ext.z),
            center + new Vector3(ext.x, 0, ext.z),
            center + new Vector3(ext.x, 0, -ext.z),
            center + new Vector3(-ext.x, 0, -ext.z)
        };
    }

    public List<GameObject> GetTiles() => tiles;

    public (int, int) GetRowAndColumnsOfPlatform() => (rows, columns);

    public DefaultTile FindTileWithCoordinates(int x, int z)
    {
        string targetName = $"Tile_{x}_{z}";
        foreach (var tile in tiles)
        {
            if (tile != null && tile.name == targetName)
                return tile.GetComponent<DefaultTile>();
        }
        Debug.LogError($"No such tile found: {targetName}");
        return null;
    }

    public (int, int)? FindCoordinatesWithTile(DefaultTile tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("Tile reference is null.");
            return null;
        }

        var parts = tile.name.Split('_');
        if (parts.Length == 3
            && int.TryParse(parts[1], out int x)
            && int.TryParse(parts[2], out int z))
        {
            return (x, z);
        }
        Debug.LogError($"Tile name format invalid: {tile.name}");
        return null;
    }

    public int GetTilesCount() => tiles.Count;

    public DefaultTile GetRandomTile()
    {
        if (tiles.Count == 0) {
            Debug.LogError("No tiles, bruh, from GetRandomTile!");
            return null;
        }
        int idx = Random.Range(0, tiles.Count);
        return tiles[idx].GetComponent<DefaultTile>();
    }
}
