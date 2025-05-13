using UnityEngine;

public class CubePaintings : MonoBehaviour
{
    [SerializeField] private GameObject painting1;
    [SerializeField] private GameObject painting2;
    [SerializeField] private GameObject painting3;
    [SerializeField] private GameObject painting4;

    [Tooltip("The face of the cube to place paintings on (0-5: right, left, up, down, forward, back)")]
    [SerializeField] private int targetFace = 5;

    [Tooltip("Distance to offset paintings from cube surface")]
    [SerializeField] private float paintingOffset = 0.01f;

    [Tooltip("Space between paintings")]
    [SerializeField] private float spacing = 0.05f;

    private GameObject[] paintingPrefabs;
    private Vector2[] gridPositions = new Vector2[4] {
        new Vector2(-1,  1), // top-left
        new Vector2( 1,  1), // top-right
        new Vector2(-1, -1), // bottom-left
        new Vector2( 1, -1)  // bottom-right
    };

    private void Start()
    {
        paintingPrefabs = new GameObject[] { painting1, painting2, painting3, painting4 };

        for (int i = 0; i < paintingPrefabs.Length; i++)
        {
            AddPainting(i);
        }
    }

    private void AddPainting(int index)
    {
        if (paintingPrefabs.Length != 4)
        {
            Debug.LogError("Exactly 4 paintings are required.");
            return;
        }

        GameObject prefab = paintingPrefabs[index];
        if (prefab == null)
        {
            Debug.LogWarning($"Painting prefab at index {index} is null.");
            return;
        }

        Renderer cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer == null)
        {
            Debug.LogError("Cube must have a Renderer component.");
            return;
        }

        Vector3 cubeSize = cubeRenderer.bounds.size;
        Vector3 faceNormal = GetFaceNormal(targetFace);
        Vector3 faceUp = GetFaceUp(targetFace);
        Vector3 faceRight = Vector3.Cross(faceUp, -faceNormal).normalized;
        float distanceToFace = GetDistanceToFace(cubeSize, targetFace);

        GameObject painting = Instantiate(prefab);
        Renderer paintingRenderer = painting.GetComponent<Renderer>();
        if (paintingRenderer == null)
        {
            Debug.LogError($"Painting at index {index} must have a Renderer component.");
            Destroy(painting);
            return;
        }

        Vector3 paintingSize = paintingRenderer.bounds.size;
        float availableWidth = cubeSize.x * 0.8f;
        float availableHeight = cubeSize.y * 0.8f;
        float paintingWidth = (availableWidth - spacing) / 2f;
        float paintingHeight = (availableHeight - spacing) / 2f;
        float scaleFactor = Mathf.Min(
            paintingWidth / paintingSize.x,
            paintingHeight / paintingSize.y
        );

        painting.transform.localScale *= scaleFactor;
        paintingSize = painting.GetComponent<Renderer>().bounds.size;

        Vector2 gridPos = gridPositions[index];
        Vector3 position = transform.position
            + faceNormal * (distanceToFace + paintingOffset + paintingSize.z / 2f)
            + faceRight * (gridPos.x * (paintingWidth / 2f + spacing / 2f))
            + faceUp * (gridPos.y * (paintingHeight / 2f + spacing / 2f));

        painting.transform.position = position;
        painting.transform.parent = transform;
    }

    private Vector3 GetFaceNormal(int face)
    {
        switch (face)
        {
            case 0: return transform.right;
            case 1: return -transform.right;
            case 2: return transform.up;
            case 3: return -transform.up;
            case 4: return transform.forward;
            case 5: return -transform.forward;
            default: return -transform.forward;
        }
    }

    private Vector3 GetFaceUp(int face)
    {
        switch (face)
        {
            case 0:
            case 1:
            case 4:
            case 5: return transform.up;
            case 2: return -transform.forward;
            case 3: return transform.forward;
            default: return transform.up;
        }
    }

    private float GetDistanceToFace(Vector3 cubeSize, int face)
    {
        switch (face)
        {
            case 0:
            case 1: return cubeSize.x / 2f;
            case 2:
            case 3: return cubeSize.y / 2f;
            case 4:
            case 5: return cubeSize.z / 2f;
            default: return cubeSize.z / 2f;
        }
    }
}
