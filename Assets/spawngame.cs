using System.Collections;
using UnityEngine;

public class SpawnObjectOnSurface : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 localPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 spawnLocalScale = Vector3.one;
    [SerializeField] private bool spawnOnStart = true;

    private GameObject spawnedInstance;

    void Start()
    {
        if (spawnOnStart)
            SpawnPrefab();
    }

    public GameObject SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab to spawn is not assigned.", this);
            return null;
        }

        // Calculate surface top (using surface's bounds)
        Bounds surfaceBounds = CalculateBounds(gameObject);
        float topY = surfaceBounds.max.y;

        // Use pivot point (assumed to be at the bottom of the prefab)
        Vector3 spawnPosition = new Vector3(
            surfaceBounds.center.x,
            topY,
            surfaceBounds.center.z
        );

        // Apply offset in world space
        spawnPosition += transform.TransformVector(localPositionOffset);

        // Instantiate and configure
        spawnedInstance = Instantiate(
            prefabToSpawn,
            spawnPosition,
            transform.rotation * Quaternion.Euler(localRotationOffset),
            null
        );

        spawnedInstance.transform.localScale = spawnLocalScale;

        return spawnedInstance;
    }

    private Bounds CalculateBounds(GameObject root)
    {
        Renderer[] rends = root.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return new Bounds(root.transform.position, Vector3.zero);

        Bounds bounds = rends[0].bounds;
        foreach (Renderer rend in rends) bounds.Encapsulate(rend.bounds);
        return bounds;
    }

    private Bounds GetPrefabBounds(GameObject prefab)
    {
        Bounds combinedBounds = new Bounds();
        bool firstBound = true;

        // Analyze prefab structure without instantiation
        foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>(true))
        {
            // Calculate world bounds relative to prefab root
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            // Get local bounds and transform them
            Bounds localBounds = meshFilter.sharedMesh.bounds;
            Matrix4x4 matrix = GetTransformMatrix(renderer.transform, prefab.transform);
            localBounds = TransformBounds(localBounds, matrix);

            if (firstBound)
            {
                combinedBounds = localBounds;
                firstBound = false;
            }
            else
            {
                combinedBounds.Encapsulate(localBounds);
            }
        }

        return combinedBounds;
    }

    private Matrix4x4 GetTransformMatrix(Transform child, Transform root)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        Transform current = child;

        while (current != null && current != root)
        {
            matrix = Matrix4x4.TRS(
                current.localPosition,
                current.localRotation,
                current.localScale
            ) * matrix;

            current = current.parent;
        }

        return matrix;
    }

    private Bounds TransformBounds(Bounds bounds, Matrix4x4 matrix)
    {
        Vector3 center = matrix.MultiplyPoint(bounds.center);
        Vector3 extents = bounds.extents;

        Vector3 axisX = matrix.MultiplyVector(Vector3.right * extents.x);
        Vector3 axisY = matrix.MultiplyVector(Vector3.up * extents.y);
        Vector3 axisZ = matrix.MultiplyVector(Vector3.forward * extents.z);

        float maxX = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        float maxY = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        float maxZ = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds(center, new Vector3(maxX * 2, maxY * 2, maxZ * 2));
    }

    // Keep existing DestroySpawned and GetSpawned methods
}