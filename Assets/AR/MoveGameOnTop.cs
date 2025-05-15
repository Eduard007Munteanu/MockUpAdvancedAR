using UnityEngine;
using System.Collections.Generic;

public class PlaceObjectsFromLayerOnSelf : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float depthSpacing = 0.5f;

    void Start()
    {
        Renderer selfRenderer = GetComponentInChildren<Renderer>();
        if (selfRenderer == null)
        {
            Debug.LogWarning("Missing Renderer on the base object.");
            return;
        }

        Bounds selfBounds = selfRenderer.bounds;
        float topY = selfBounds.max.y;
        Vector2 centerXZ = new Vector2(selfBounds.center.x, selfBounds.center.z);

        List<GameObject> objectsToPlace = new List<GameObject>();
        List<float> objectDepths = new List<float>();

        // Collect valid objects and their depths (Z-size)
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (((1 << obj.layer) & targetLayer) == 0 || !obj.activeInHierarchy || obj == gameObject)
                continue;

            Renderer placeRenderer = obj.GetComponentInChildren<Renderer>();
            if (placeRenderer == null)
            {
                Debug.LogWarning($"Object '{obj.name}' has no Renderer; skipping.");
                continue;
            }

            objectsToPlace.Add(obj);
            objectDepths.Add(placeRenderer.bounds.size.z);
        }

        if (objectsToPlace.Count == 0)
            return;

        // Calculate total depth needed
        float totalDepth = 0f;
        foreach (float depth in objectDepths)
        {
            totalDepth += depth;
        }
        totalDepth += (objectsToPlace.Count - 1) * depthSpacing;

        // Calculate starting Z position to center objects
        float currentZ = centerXZ.y - (totalDepth / 2f);

        // Place each object
        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            GameObject obj = objectsToPlace[i];
            Renderer placeRenderer = obj.GetComponentInChildren<Renderer>();
            Bounds placeBounds = placeRenderer.bounds;
            Vector3 originalPos = obj.transform.position;

            float newY = topY + (originalPos.y - placeBounds.min.y);
            float objDepth = objectDepths[i];
            Vector3 newPosition = new Vector3(
                centerXZ.x,
                newY,
                currentZ + (objDepth / 2f)
            );

            obj.transform.position = newPosition;
            obj.transform.rotation = transform.rotation;

            currentZ += objDepth + depthSpacing;

            Debug.Log($"Placed '{obj.name}' at ({newPosition.x:F2}, {newY:F2}, {newPosition.z:F2})");
        }
    }
}
