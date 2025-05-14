using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a prefab so its bottom surface aligns with the top surface of this object,
/// accounting for arbitrary scaling, child renderers, and mixed-reality anchors.
/// </summary>
public class SpawnObjectOnSurface : MonoBehaviour
{
    [Tooltip("The prefab to spawn (with children)")]
    [SerializeField] private GameObject prefabToSpawn;

    [Tooltip("Optional local position offset after aligning bottom-top")]
    [SerializeField] private Vector3 localPositionOffset = Vector3.zero;

    [Tooltip("Optional local rotation offset")]
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;

    [Tooltip("Optional local scale for spawned object")]
    [SerializeField] private Vector3 spawnLocalScale = Vector3.one;

    [Tooltip("Automatically spawn on Start?")]
    [SerializeField] private bool spawnOnStart = true;

    private GameObject spawnedInstance;

    void Start()
    {
        if (spawnOnStart)
            SpawnPrefab();
    }

    /// <summary>
    /// Spawns the prefab and aligns its bottom to this object's top.
    /// </summary>
    public GameObject SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab to spawn is not assigned.", this);
            return null;
        }

        // Calculate surface bounds
        Bounds surfaceBounds = CalculateBounds(gameObject);
        float topY = surfaceBounds.max.y;

        // Instantiate temporary prefab to calculate bounds
        GameObject tempInstance = Instantiate(prefabToSpawn);
        tempInstance.SetActive(false); // Deactivate to prevent component logic
        Bounds prefabBounds = CalculateBounds(tempInstance);
        Destroy(tempInstance);

        float prefabMinY = prefabBounds.min.y;

        // Determine spawn position
        Vector3 spawnPosition = surfaceBounds.center;
        spawnPosition.y = topY - prefabMinY;

        // Apply local offset in world space
        spawnPosition += transform.TransformVector(localPositionOffset);

        // Instantiate and configure
        spawnedInstance = Instantiate(
            prefabToSpawn,
            spawnPosition,
            transform.rotation * Quaternion.Euler(localRotationOffset),
            null);

        spawnedInstance.transform.localScale = spawnLocalScale;

        return spawnedInstance;
    }

    /// <summary>
    /// Calculates the world-space bounds of all Renderers under a root GameObject.
    /// </summary>
    private Bounds CalculateBounds(GameObject root)
    {
        Renderer[] rends = root.GetComponentsInChildren<Renderer>(includeInactive: false);
        if (rends.Length == 0)
            return new Bounds(root.transform.position, Vector3.zero);

        Bounds bounds = rends[0].bounds;
        foreach (Renderer rend in rends)
            bounds.Encapsulate(rend.bounds);

        return bounds;
    }

    public void DestroySpawned()
    {
        if (spawnedInstance != null)
        {
            Destroy(spawnedInstance);
            spawnedInstance = null;
        }
    }

    public GameObject GetSpawned()
    {
        return spawnedInstance;
    }
}