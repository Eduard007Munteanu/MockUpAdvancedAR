using UnityEngine;
using System.Collections.Generic;

public class PlaceObjectsFromLayerOnSelf : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float depthSpacing = 0.5f;

    // Define the possible preset facing directions
    public enum FacingDirection
    {
        North,
        East,
        South,
        West
    }

    [SerializeField] FacingDirection objectFacingDirection = FacingDirection.North; // Serialized field for direction selection

    // Helper method to convert the enum direction to a Quaternion rotation around Y
    private Quaternion GetRotationFromDirection(FacingDirection direction)
    {
        switch (direction)
        {
            case FacingDirection.North:
                return Quaternion.Euler(0, 0, 0); // 0 degrees around Y
            case FacingDirection.East:
                return Quaternion.Euler(0, 90, 0); // 90 degrees around Y
            case FacingDirection.South:
                return Quaternion.Euler(0, 180, 0); // 180 degrees around Y
            case FacingDirection.West:
                return Quaternion.Euler(0, 270, 0); // 270 degrees around Y (or -90)
            default:
                return Quaternion.identity; // Default to North (no rotation)
        }
    }

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
        Vector3 selfCenter = selfBounds.center; // Get center for X and Z
        float selfDepth = selfBounds.size.z; // Get the depth (Z size) of the calling object

        if (selfDepth <= 0)
        {
            Debug.LogWarning("Base object has zero or negative depth. Scaling based on depth ratio might be problematic.");
            // Continue execution, but note the potential issue if scaling depends critically on selfDepth > 0.
        }


        List<GameObject> objectsToPlace = new List<GameObject>();
        List<float> objectOriginalDepths = new List<float>();
        List<Vector3> objectOriginalScales = new List<Vector3>();
        List<Vector3> objectOriginalPositions = new List<Vector3>();
        List<Bounds> objectOriginalBounds = new List<Bounds>(); // Store original bounds for size calculation

        // Collect valid objects, their original depths, scales, positions, and bounds
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

            // IMPORTANT: Use original bounds BEFORE any scaling or rotation changes
            Bounds placeBounds = placeRenderer.bounds;

            objectsToPlace.Add(obj);
            objectOriginalDepths.Add(placeBounds.size.z);
            objectOriginalScales.Add(obj.transform.localScale); // Store original scale
            objectOriginalPositions.Add(obj.transform.position); // Store original position
            objectOriginalBounds.Add(placeBounds); // Store original bounds
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

        // Calculate starting Z position to center the stack on the calling object's Z center
        float stackStartZ = selfCenter.z - (totalDepth / 2f);
        float currentZ = stackStartZ;

        // Calculate the desired base rotation for placed objects (Calling object's rotation + selected direction rotation)
        Quaternion directionRotation = GetRotationFromDirection(objectFacingDirection);
        Quaternion baseRotation = transform.rotation * directionRotation;


        // Place each object
        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            GameObject obj = objectsToPlace[i];
            float objOriginalDepth = objectOriginalDepths[i];
            Vector3 originalScale = objectOriginalScales[i];
            Vector3 originalPos = objectOriginalPositions[i];
            Bounds originalBounds = objectOriginalBounds[i];


            // --- Start: Your Preferred Scaling Logic ---
            // Calculate scaling ratio based on original depth and self depth
            float scaleRatio = (selfDepth > 0) ? objOriginalDepth / selfDepth : 1.0f; // Handle division by zero
            if (selfDepth <= 0)
            {
                Debug.LogWarning($"Base object '{gameObject.name}' has zero or negative depth. Scaling ratio calculation skipped for '{obj.name}'. Using scaleRatio = 1.0f.");
            }

            // Calculate minimumSize based on original X and Z scales multiplied by scaleRatio
            float minimumSize = Mathf.Min(originalScale.x * scaleRatio, originalScale.z * scaleRatio);

            // Apply minimumSize to new X and Z, and original Y * scaleRatio to new Y
            Vector3 newScale = new Vector3(minimumSize, originalScale.y * scaleRatio, minimumSize);
            // --- End: Your Preferred Scaling Logic ---

            obj.transform.localScale = newScale; // Apply the calculated scale


            // --- Start: Correct Centering for Scaled and Rotated Object ---
            // Apply rotation *before* getting bounds if rotation affects bounds calculation.
            // The baseRotation is calculated once outside the loop. Apply it here before getting bounds.
            //Quarternion oldRotation = 
            //obj.transform.rotation = baseRotation;

            // Get bounds *after* scaling and rotation to find the new center relative to the pivot
            Renderer currentRenderer = obj.GetComponentInChildren<Renderer>();
            Bounds currentBounds = (currentRenderer != null) ? currentRenderer.bounds : new Bounds(obj.transform.position, Vector3.zero); // Fallback bounds


            // Calculate the offset from the pivot (transform.position) to the new bounds center
            Vector3 pivotToBoundsCenterOffset = currentBounds.center - obj.transform.position;

            // Calculate the target world position for the bounds center
            float targetBoundsCenterX = selfCenter.x;
            float targetBoundsCenterZ = currentZ + (objOriginalDepth / 2f); // Center based on original depth for stacking

            // Calculate the required pivot position to place the bounds center at the target X and Z
            float requiredPivotX = targetBoundsCenterX - pivotToBoundsCenterOffset.x;
            float requiredPivotZ = targetBoundsCenterZ - pivotToBoundsCenterOffset.z;

            // Calculate new Y position to place the bottom of the original object bounds at topY
            // Use the offset from the original pivot to the original world bounds bottom Y.
            // This offset is (originalPos.y - originalBounds.min.y).
            float distanceToOriginalBottomY = originalPos.y - originalBounds.min.y;
            float newY = topY + distanceToOriginalBottomY;


            // Set the object's final position using the required pivot X, the calculated Y, and the required pivot Z
            Vector3 newPosition = new Vector3(
                requiredPivotX, // Corrected X position for centering bounds
                newY,       // Y position aligning original bottom to calling object's top Y
                requiredPivotZ // Corrected Z position for centering bounds
            );

            obj.transform.position = newPosition;
            // Rotation was applied earlier before getting bounds: obj.transform.rotation = baseRotation;
            // --- End: Correct Centering for Scaled and Rotated Object ---


            currentZ += objOriginalDepth + depthSpacing; // Increment currentZ using the *original* depth

            // Log placement details, including the object's world bounds center after placement
            Debug.Log($"Placed '{obj.name}' at ({newPosition.x:F2}, {newY:F2}, {newPosition.z:F2}) World Bounds Center: ({currentBounds.center.x:F2}, {currentBounds.center.y:F2}, {currentBounds.center.z:F2}) with scale ({newScale.x:F2}, {newScale.y:F2}, {newScale.z:F2}) and rotation {obj.transform.rotation.eulerAngles:F2}");
        }
    }
}