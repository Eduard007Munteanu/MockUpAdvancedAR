// using System.Collections.Generic;
// using UnityEngine;

// public class GridOnCubus : MonoBehaviour
// {
//     public GameObject tilePrefab;
//     public int rows = 2;
//     public int columns = 2;

//     private List<GameObject> tiles = new List<GameObject>();

//     [SerializeField] private GameObject materialManager;

//     void Start()
//     {
//         Renderer cubusRenderer = transform.parent.GetComponent<Renderer>();
//         Renderer tileRenderer = tilePrefab.GetComponent<Renderer>();

//         if (cubusRenderer == null || tileRenderer == null)
//         {
//             Debug.LogError("Renderer missing on Cubus or tilePrefab.");
//             return;
//         }

//         Vector3 cubusSize = cubusRenderer.bounds.size;
//         Vector3 tileSize = tileRenderer.bounds.size;

//         Vector3 adjustedTileScale = new Vector3(
//             (cubusSize.x / columns) / tileSize.x,
//             1,
//             (cubusSize.z / rows) / tileSize.z
//         );

//         // Correct global top surface height
//         float globalTopSurfaceY = cubusRenderer.bounds.max.y + 0.01f;

//         // Global bottom-left corner (world coordinates)
//         Vector3 globalBottomLeft = new Vector3(
//             cubusRenderer.bounds.center.x - cubusSize.x / 2 + cubusSize.x / columns / 2,
//             globalTopSurfaceY,
//             cubusRenderer.bounds.center.z - cubusSize.z / 2 + cubusSize.z / rows / 2
//         );

//         for (int x = 0; x < columns; x++)
//         {
//             for (int z = 0; z < rows; z++)
//             {
//                 Vector3 globalOffset = new Vector3(
//                     x * (cubusSize.x / columns),
//                     0,
//                     z * (cubusSize.z / rows)
//                 );

//                 Vector3 globalSpawnPos = globalBottomLeft + globalOffset;

//                 // Convert the global position into the local position of the grid GameObject
//                 Vector3 localSpawnPos = transform.InverseTransformPoint(globalSpawnPos);

//                 GameObject tile = Instantiate(tilePrefab, transform);
//                 tile.name = $"Tile_{x}_{z}";
//                 tile.transform.localPosition = localSpawnPos;
//                 tile.transform.localRotation = Quaternion.identity;
//                 tile.transform.localScale = adjustedTileScale;

//                 tiles.Add(tile);
//             }
//         }

//         materialManager.GetComponent<MaterialManager>().SetTileRenderedOnRunTime(); 


//     }

//     public Vector3[] GetTileCorners(GameObject tile)
//     {
//         Transform tileTransform = tile.transform;

//         Vector3 center = tileTransform.position;
//         Vector3 right = tileTransform.right * 0.5f * tileTransform.localScale.x * 10f;
//         Vector3 forward = tileTransform.forward * 0.5f * tileTransform.localScale.z * 10f;

//         Vector3 topLeft     = center - right + forward;
//         Vector3 topRight    = center + right + forward;
//         Vector3 bottomLeft  = center - right - forward;
//         Vector3 bottomRight = center + right - forward;

//         return new Vector3[] { topLeft /*bottomRight from board perspective  */, topRight /*bottomLeft from board perspective */, /*topLeft from board perspective */ bottomRight, /*topRight from board perspective */ bottomLeft };
//     }

//     public List<GameObject> GetTiles()
//     {
//         return tiles;
//     }
// }
