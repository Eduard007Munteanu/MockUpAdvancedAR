using UnityEngine;

[HelpURL("https://example.com/docs/SpawnObjectOnSurface")]
public class SpawnObjectOnSurface : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab to spawn (with children)")]
    [SerializeField] private GameObject prefabToSpawn;

    [Tooltip("Automatically spawn on Start?")]
    [SerializeField] private bool spawnOnStart = true;

    [Header("Positioning")]
    [Tooltip("Local position offset after placement")]
    [SerializeField] private Vector3 localPositionOffset = Vector3.zero;

    [Tooltip("Local rotation offset (euler angles)")]
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;

    [Header("Scaling")]
    [Tooltip("Base local scale for spawned object")]
    [SerializeField] private Vector3 spawnLocalScale = Vector3.one;

    [Tooltip("Should object scale to fit surface width?")]
    [SerializeField] private bool scaleToFitWidth = false;

    [Tooltip("Should object scale to fit surface length?")]
    [SerializeField] private bool scaleToFitLength = false;

    [Tooltip("Size margin (1.0 = exact fit, 0.8 = 20% margin)")]
    [SerializeField][Range(0.1f, 2f)] private float sizeMargin = 1.0f;

    [Header("Advanced")]
    [Tooltip("Should maintain original height when scaling?")]
    [SerializeField] private bool maintainHeight = true;

    [Tooltip("Should recalculate bounds on every spawn?")]
    [SerializeField] private bool alwaysRecalculateBounds = false;

    private GameObject spawnedInstance;
    private Bounds? cachedSurfaceBounds;
    private Bounds? cachedPrefabBounds;

    void Start()
    {
        if (spawnOnStart)
            SpawnPrefab();
    }

    /// <summary>
    /// Main spawn method with automatic positioning and scaling
    /// </summary>
    public GameObject SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError($"{name}: Prefab to spawn is not assigned!", this);
            return null;
        }

        // Get or calculate bounds
        Bounds surfaceBounds = GetSurfaceBounds();
        Bounds prefabBounds = GetPrefabBounds(prefabToSpawn);

        // Calculate scaling
        Vector3 adjustedScale = CalculateOptimalScale(
            surfaceBounds.size,
            prefabBounds.size
        );

        // Calculate final position
        Vector3 spawnPosition = CalculateSpawnPosition(
            surfaceBounds,
            prefabBounds,
            adjustedScale
        );

        // Create instance
        spawnedInstance = Instantiate(
            prefabToSpawn,
            spawnPosition,
            transform.rotation * Quaternion.Euler(localRotationOffset),
            null
        );

        spawnedInstance.transform.localScale = adjustedScale;
        return spawnedInstance;
    }

    /// <summary>
    /// Calculates optimal scale based on surface and object dimensions
    /// </summary>
    private Vector3 CalculateOptimalScale(Vector3 surfaceSize, Vector3 prefabSize)
    {
        Vector3 newScale = spawnLocalScale;

        if (scaleToFitWidth || scaleToFitLength)
        {
            float widthScale = scaleToFitWidth ?
                (surfaceSize.x * sizeMargin) / prefabSize.x : 1f;
            float lengthScale = scaleToFitLength ?
                (surfaceSize.z * sizeMargin) / prefabSize.z : 1f;

            float minScale = Mathf.Min(widthScale, lengthScale);

            newScale.x *= minScale;
            newScale.z *= minScale;

            if (maintainHeight)
            {
                // Maintain original Y scale unless specifically changed
                newScale.y = spawnLocalScale.y;
            }
            else
            {
                newScale.y *= minScale;
            }
        }

        return newScale;
    }

    /// <summary>
    /// Calculates spawn position accounting for scaled object height
    /// </summary>
    private Vector3 CalculateSpawnPosition(Bounds surfaceBounds, Bounds prefabBounds, Vector3 scale)
    {
        Vector3 position = surfaceBounds.center;

        // Position on surface top + scaled object half-height
        position.y = surfaceBounds.max.y + (prefabBounds.extents.y * scale.y);

        // Apply local offset in world space
        position += transform.TransformVector(localPositionOffset);

        return position;
    }

    /// <summary>
    /// Gets surface bounds with caching
    /// </summary>
    private Bounds GetSurfaceBounds()
    {
        if (!alwaysRecalculateBounds && cachedSurfaceBounds.HasValue)
            return cachedSurfaceBounds.Value;

        cachedSurfaceBounds = CalculateBounds(gameObject);
        return cachedSurfaceBounds.Value;
    }

    /// <summary>
    /// Gets prefab bounds with caching
    /// </summary>
    private Bounds GetPrefabBounds(GameObject prefab)
    {
        if (!alwaysRecalculateBounds && cachedPrefabBounds.HasValue)
            return cachedPrefabBounds.Value;

        cachedPrefabBounds = CalculatePrefabBounds(prefab);
        return cachedPrefabBounds.Value;
    }

    /// <summary>
    /// Calculates world-space bounds of all renderers under a GameObject
    /// </summary>
    private Bounds CalculateBounds(GameObject root)
    {
        Renderer[] rends = root.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return new Bounds(root.transform.position, Vector3.zero);

        Bounds bounds = rends[0].bounds;
        foreach (Renderer rend in rends) bounds.Encapsulate(rend.bounds);
        return bounds;
    }

    /// <summary>
    /// Calculates prefab bounds without instantiation
    /// </summary>
    private Bounds CalculatePrefabBounds(GameObject prefab)
    {
        Bounds combinedBounds = new Bounds();
        bool hasBounds = false;

        foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
            {
                Mesh mesh = null;

                if (renderer is MeshRenderer mr)
                {
                    MeshFilter mf = mr.GetComponent<MeshFilter>();
                    if (mf != null) mesh = mf.sharedMesh;
                }
                else if (renderer is SkinnedMeshRenderer smr)
                {
                    mesh = smr.sharedMesh;
                }

                if (mesh != null)
                {
                    Matrix4x4 matrix = renderer.transform.localToWorldMatrix;
                    Bounds meshBounds = TransformBounds(mesh.bounds, matrix);

                    if (!hasBounds)
                    {
                        combinedBounds = meshBounds;
                        hasBounds = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(meshBounds);
                    }
                }
            }
        }

        return hasBounds ? combinedBounds : new Bounds(Vector3.zero, Vector3.zero);
    }

    /// <summary>
    /// Transforms bounds from local to world space
    /// </summary>
    private Bounds TransformBounds(Bounds bounds, Matrix4x4 matrix)
    {
        Vector3 center = matrix.MultiplyPoint(bounds.center);
        Vector3 extents = bounds.extents;

        Vector3 axisX = matrix.MultiplyVector(Vector3.right * extents.x);
        Vector3 axisY = matrix.MultiplyVector(Vector3.up * extents.y);
        Vector3 axisZ = matrix.MultiplyVector(Vector3.forward * extents.z);

        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds(center, extents * 2);
    }

    /// <summary>
    /// Destroys the currently spawned instance
    /// </summary>
    public void DestroySpawned()
    {
        if (spawnedInstance != null)
        {
            Destroy(spawnedInstance);
            spawnedInstance = null;
        }
    }

    /// <summary>
    /// Returns the currently spawned instance
    /// </summary>
    public GameObject GetSpawned()
    {
        return spawnedInstance;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && prefabToSpawn != null)
        {
            Gizmos.color = Color.green;
            Bounds surfaceBounds = CalculateBounds(gameObject);
            Bounds prefabBounds = CalculatePrefabBounds(prefabToSpawn);
            Vector3 scale = CalculateOptimalScale(surfaceBounds.size, prefabBounds.size);
            Vector3 position = CalculateSpawnPosition(surfaceBounds, prefabBounds, scale);
            
            Gizmos.DrawWireCube(position, Vector3.Scale(prefabBounds.size, scale));
            Gizmos.DrawLine(surfaceBounds.center, position);
        }
    }
#endif
}