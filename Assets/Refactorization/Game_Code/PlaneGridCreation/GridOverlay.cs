using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    public static GridOverlay Instance { get; private set; }

    public GameObject tilePrefab;
    public GameObject enemyTilePrefab;

    public int rows = 2;
    public int columns = 2;

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
        // Hent renderer på parent-objektet
        var parentRenderer = transform.parent.GetComponent<Renderer>();
        if (parentRenderer == null)
        {
            Debug.LogError("Renderer missing on parent.");
            return;
        }
        Vector3 platformSize = parentRenderer.bounds.size;
        float topY = parentRenderer.bounds.max.y + 0.01f;

        // Hent rå mesh-størrelse fra prefab (mesh units)
        var meshFilter = tilePrefab.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter missing on tilePrefab.");
            return;
        }
        Vector3 meshSize = meshFilter.sharedMesh.bounds.size;

        // Beregn ønsket størrelse per tile
        float tileWidth = platformSize.x / columns;
        float tileDepth = platformSize.z / rows;

        // Skaleringsfaktor så mesh dækker tileWidth x tileDepth
        Vector3 adjustedTileScale = new Vector3(
            tileWidth / meshSize.x ,
            1f,
            tileDepth / meshSize.z 
        );

        // Find bund-venstre hjørne på toppen af platformen
        Vector3 min = parentRenderer.bounds.min;
        Vector3 origin = new Vector3(min.x, topY, min.z);

        // Instantiate tiles
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                Vector3 spawnPos = origin + new Vector3(
                    tileWidth * x + tileWidth * 0.5f,
                    0f,
                    tileDepth * z + tileDepth * 0.5f
                );

                GameObject prefab = (z == 0) ? enemyTilePrefab : tilePrefab;
                var go = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
                go.name = $"Tile_{x}_{z}";
                go.transform.localScale = adjustedTileScale;
                tiles.Add(go);
            }
        }

        // Notify itemManager
        if (itemManager != null)
            itemManager.GetComponent<ItemManager>().TilesRendered();
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
        if (tiles.Count == 0) return null;
        int idx = Random.Range(0, tiles.Count);
        return tiles[idx].GetComponent<DefaultTile>();
    }
}
