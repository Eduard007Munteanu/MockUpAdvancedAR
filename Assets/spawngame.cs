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

        // Compute this object's top Y in world space using all child Renderers
        Bounds surfaceBounds = CalculateBounds(gameObject);
        float topY = surfaceBounds.max.y;

        // Compute prefab's local bottom offset (min Y) and total height
        Bounds prefabBounds = CalculateBounds(prefabToSpawn);
        float prefabMinY = prefabBounds.min.y;
        float prefabHeight = prefabBounds.size.y;

        // Determine spawn position: world XZ at this object's center,
        // Y so that prefab's bottom (at prefabMinY) sits at topY
        Vector3 spawnPosition = surfaceBounds.center;
        spawnPosition.y = topY - prefabMinY;

        // Apply any local offset in this object's space
        spawnPosition += transform.TransformVector(localPositionOffset);

        // Instantiate
        spawnedInstance = Instantiate(
            prefabToSpawn,
            spawnPosition,
            transform.rotation * Quaternion.Euler(localRotationOffset),
            null);

        // Set scale
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
        {
            // No renderers: use object's transform position as a point
            return new Bounds(root.transform.position, Vector3.zero);
        }

        Bounds bounds = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
        {
            bounds.Encapsulate(rends[i].bounds);
        }
        return bounds;
    }

    /// <summary>
    /// Destroys the spawned instance, if any.
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
    /// Returns the current spawned instance.
    /// </summary>
    public GameObject GetSpawned()
    {
        return spawnedInstance;
    }
}
