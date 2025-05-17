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
        float selfDepth = selfBounds.size.z; // Get the depth (Z size) of the calling object

        if (selfDepth == 0)
        {
            Debug.LogWarning("Base object has zero depth, cannot calculate scaling ratio.");
            return;
        }

        List<GameObject> objectsToPlace = new List<GameObject>();
        List<float> objectOriginalDepths = new List<float>();
        List<Vector3> objectOriginalScales = new List<Vector3>();
        List<Vector3> objectOriginalPositions = new List<Vector3>(); // Store original positions for Y calculation

        // Collect valid objects, their original depths, scales, and positions
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

            // IMPORTANT: Use original bounds BEFORE any scaling
            Bounds placeBounds = placeRenderer.bounds;

            objectsToPlace.Add(obj);
            objectOriginalDepths.Add(placeBounds.size.z);
            objectOriginalScales.Add(obj.transform.localScale); // Store original scale
            objectOriginalPositions.Add(obj.transform.position); // Store original position
        }

        if (objectsToPlace.Count == 0)
        {
            Debug.Log("No objects found on the target layer.");
            return;
        }

        // Calculate total depth needed using original depths
        float totalDepth = 0f;
        foreach (float depth in objectOriginalDepths)
        {
            totalDepth += depth;
        }
        totalDepth += (objectsToPlace.Count - 1) * depthSpacing;

        // Calculate starting Z position to center objects
        float currentZ = centerXZ.y - (totalDepth / 2f); // centerXZ.y is actually the Z coordinate

        // Place each object
        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            GameObject obj = objectsToPlace[i];
            float objOriginalDepth = objectOriginalDepths[i];
            Vector3 originalScale = objectOriginalScales[i];
            Vector3 originalPos = objectOriginalPositions[i];

            // Calculate scaling ratio based on original depths
            float scaleRatio = objOriginalDepth / selfDepth;

            // Calculate and apply the new scale (scaling X and Y by the ratio)
            float minimumSize = Mathf.Min(originalScale.x * scaleRatio, originalScale.z * scaleRatio);
            Vector3 newScale = new Vector3(minimumSize, originalScale.y * scaleRatio, minimumSize);
            obj.transform.localScale = newScale;

            // Re-get bounds AFTER scaling to get the potentially adjusted min.y for vertical placement.
            // However, the previous logic using originalPos and original bounds min.y was better
            // to place the *original* bottom at the target Y. Let's stick to that for consistency
            // with the Y placement logic. The scaling only affects X and Y, so original bounds
            // min.y relative to original position is correct for initial vertical alignment.
            Renderer placeRenderer = obj.GetComponentInChildren<Renderer>();
            if (placeRenderer == null) // Should not happen based on initial check, but safety
            {
                Debug.LogWarning($"Object '{obj.name}' is missing Renderer during placement loop; skipping.");
                continue;
            }
            Bounds placeBounds = placeRenderer.bounds; // Get bounds after scaling if needed for position calculation

            // Calculate new Y position to place the bottom of the original object bounds at topY
            // Using originalPos and original bounds min.y relative to origin is key here.
            // placeBounds.min.y is relative to world space center of the scaled object.
            // A more robust way might be to calculate the displacement from the object's pivot
            // to its *original* bottom and add that to topY.
            // Let's refine the Y calculation to use the original object's local bottom point
            // projected to world space and align that with topY.

            // Calculate the local bottom point of the original object (assuming pivot is somewhat central)
            // This is tricky without knowing the pivot. The original Y calculation relies on
            // (originalPos.y - placeBounds.min.y) which is the distance from the original object's
            // pivot Y to its world-space bounds bottom Y. This distance is what needs to be added
            // to topY. This should remain valid even if X/Y scale changes.

            float distanceToOriginalBottomY = originalPos.y - placeBounds.min.y; // Distance from original pivot Y to original world bounds bottom Y
            float newY = topY + distanceToOriginalBottomY;


            // Calculate new Z position based on the stacking logic using the *original* depth
            Vector3 newPosition = new Vector3(
                centerXZ.x,
                newY,
                currentZ + (objOriginalDepth / 2f)
            );

            obj.transform.position = newPosition;
            obj.transform.rotation = transform.rotation; // Set rotation to match calling object

            currentZ += objOriginalDepth + depthSpacing; // Increment currentZ using the *original* depth

            Debug.Log($"Placed '{obj.name}' at ({newPosition.x:F2}, {newY:F2}, {newPosition.z:F2}) with scale ({newScale.x:F2}, {newScale.y:F2}, {newScale.z:F2})");
        }
    }
}