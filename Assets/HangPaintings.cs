using UnityEngine;

public class CubePaintings : MonoBehaviour
{
    [SerializeField] private GameObject painting1;
    [SerializeField] private GameObject painting2;
    [SerializeField] private GameObject painting3;
    [SerializeField] private GameObject painting4;

    [Tooltip("The face of the cube to place paintings on (0-5: right, left, up, down, forward, back)")]
    [SerializeField] private int targetFace = 5; // Default to back face

    [Tooltip("Distance to offset paintings from cube surface")]
    [SerializeField] private float paintingOffset = 0.01f;

    [Tooltip("Space between paintings")]
    [SerializeField] private float spacing = 0.05f;

    private void Start()
    {
        GameObject[] paintings = { painting1, painting2, painting3, painting4 };
        PlacePaintings(paintings);
    }

    private void PlacePaintings(GameObject[] paintings)
    {
        if (paintings.Length != 4)
        {
            Debug.LogError("Exactly 4 paintings are required.");
            return;
        }

        // Get cube dimensions
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

        // Calculate the distance from center to face
        float distanceToFace = GetDistanceToFace(cubeSize, targetFace);

        // Calculate positions for a 2x2 grid
        Vector2[] gridPositions = new Vector2[4] {
            new Vector2(-1, 1),   // Top-left
            new Vector2(1, 1),    // Top-right
            new Vector2(-1, -1),  // Bottom-left
            new Vector2(1, -1)    // Bottom-right
        };

        for (int i = 0; i < paintings.Length; i++)
        {
            if (paintings[i] == null)
            {
                Debug.LogWarning($"Painting {i + 1} is null.");
                continue;
            }

            // Instantiate the painting
            GameObject painting = Instantiate(paintings[i]);

            // Get painting size (assume it has a renderer)
            Renderer paintingRenderer = painting.GetComponent<Renderer>();
            if (paintingRenderer == null)
            {
                Debug.LogError($"Painting {i + 1} must have a Renderer component.");
                Destroy(painting);
                continue;
            }

            Vector3 paintingSize = paintingRenderer.bounds.size;

            // Calculate available space on face for paintings
            float availableWidth = cubeSize.x * 0.8f;  // Use 80% of face width
            float availableHeight = cubeSize.y * 0.8f; // Use 80% of face height

            // Calculate scaled size for each painting
            float paintingWidth = (availableWidth - spacing) / 2;
            float paintingHeight = (availableHeight - spacing) / 2;
            float scaleFactor = Mathf.Min(
                paintingWidth / paintingSize.x,
                paintingHeight / paintingSize.y
            );

            // Scale the painting
            painting.transform.localScale = painting.transform.localScale * scaleFactor;

            // Recalculate actual size after scaling
            paintingSize = painting.GetComponent<Renderer>().bounds.size;

            // Position relative to cube center
            Vector3 position = transform.position +
                               faceNormal * (distanceToFace + paintingOffset + paintingSize.z / 2) +
                               faceRight * gridPositions[i].x * (paintingWidth / 2 + spacing / 2) +
                               faceUp * gridPositions[i].y * (paintingHeight / 2 + spacing / 2);

            painting.transform.position = position;

            // Orient the painting to face outward from the cube face
            //painting.transform.rotation = Quaternion.LookRotation(-faceNormal, faceUp);

            // Make the painting a child of the cube for easier management
            painting.transform.parent = transform;
        }
    }

    private Vector3 GetFaceNormal(int face)
    {
        switch (face)
        {
            case 0: return transform.right;      // Right face (+X)
            case 1: return -transform.right;     // Left face (-X)
            case 2: return transform.up;         // Up face (+Y)
            case 3: return -transform.up;        // Down face (-Y)
            case 4: return transform.forward;    // Forward face (+Z)
            case 5: return -transform.forward;   // Back face (-Z)
            default: return -transform.forward;  // Default to back face
        }
    }

    private Vector3 GetFaceUp(int face)
    {
        switch (face)
        {
            case 0:  // Right face (+X)
            case 1:  // Left face (-X)
            case 4:  // Forward face (+Z)
            case 5:  // Back face (-Z)
                return transform.up;
            case 2:  // Up face (+Y)
                return -transform.forward;
            case 3:  // Down face (-Y)
                return transform.forward;
            default:
                return transform.up;
        }
    }

    private float GetDistanceToFace(Vector3 cubeSize, int face)
    {
        switch (face)
        {
            case 0: return cubeSize.x / 2;  // Right face (+X)
            case 1: return cubeSize.x / 2;  // Left face (-X)
            case 2: return cubeSize.y / 2;  // Up face (+Y)
            case 3: return cubeSize.y / 2;  // Down face (-Y)
            case 4: return cubeSize.z / 2;  // Forward face (+Z)
            case 5: return cubeSize.z / 2;  // Back face (-Z)
            default: return cubeSize.z / 2;
        }
    }
}