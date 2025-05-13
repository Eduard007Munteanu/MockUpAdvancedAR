using UnityEngine;
using System.Collections.Generic;

public class CubePaintings : MonoBehaviour
{
    [Tooltip("List of paintings to place on the cube face")]
    [SerializeField] private List<GameObject> paintings;

    [Tooltip("The face of the cube to place paintings on (0-5: right, left, up, down, forward, back)")]
    [SerializeField] private int targetFace = 5;

    [Tooltip("Distance to offset paintings from cube surface")]
    [SerializeField] private float paintingOffset = 0.01f;

    [Tooltip("Space between paintings")]
    [SerializeField] private float spacing = 0.05f;

    private void Start()
    {
        if (paintings == null || paintings.Count == 0)
        {
            Debug.LogWarning("No paintings assigned.");
            return;
        }

        for (int i = 0; i < paintings.Count; i++)
        {
            AddPainting(i);
        }
    }

    private void AddPainting(int index)
    {
        GameObject prefab = paintings[index];
        if (prefab == null) return;

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

        int totalPaintings = paintings.Count;
        int rows = 2;
        int cols = Mathf.CeilToInt((float)totalPaintings / rows);
        int row = index / cols;
        int col = index % cols;

        float availableWidth = cubeSize.x * 0.8f;
        float availableHeight = cubeSize.y * 0.8f;

        float paintingWidth = (availableWidth - spacing * (cols - 1)) / cols;
        float paintingHeight = (availableHeight - spacing * (rows - 1)) / rows;

        GameObject painting = Instantiate(prefab);
        Renderer paintingRenderer = painting.GetComponent<Renderer>();
        if (paintingRenderer == null)
        {
            Debug.LogError($"Painting at index {index} must have a Renderer.");
            Destroy(painting);
            return;
        }

        Vector3 originalSize = paintingRenderer.bounds.size;
        float scaleFactor = Mathf.Min(
            paintingWidth / originalSize.x,
            paintingHeight / originalSize.y
        );
        painting.transform.localScale *= scaleFactor;

        Vector3 scaledSize = painting.GetComponent<Renderer>().bounds.size;

        float xOffset = -((cols - 1) * (paintingWidth + spacing)) / 2f + col * (paintingWidth + spacing);
        float yOffset = ((rows - 1) * (paintingHeight + spacing)) / 2f - row * (paintingHeight + spacing);

        Vector3 position = transform.position
            + faceNormal * (distanceToFace + paintingOffset + scaledSize.z / 2f)
            + faceRight * xOffset
            + faceUp * yOffset;

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
