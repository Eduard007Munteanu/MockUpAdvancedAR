using UnityEngine;

public class BetterGridOverlay : MonoBehaviour
{
    public GameObject tilePrefab;
    public int rows = 6;
    public int columns = 6;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        Bounds b = r.bounds;
        Vector3 min = b.min;
        Vector3 size = b.size;

        float h = tilePrefab.transform.localScale.y;
        float w = size.x / columns;
        float d = size.z / rows;

        for (int x = 0; x < columns; x++)
        for (int z = 0; z < rows; z++)
        {
            Vector3 worldPos = new Vector3(
            min.x + w * (x + 0.5f),
            min.y + h * 0.5f,
            min.z + d * (z + 0.5f)
            );

            
            GameObject tile = Instantiate(tilePrefab);
            tile.transform.position   = worldPos;
            tile.transform.localScale = new Vector3(w, h, d);

            
            tile.transform.SetParent(transform, true);
        }

        GetComponent<MeshRenderer>().enabled = false;

    }
}
