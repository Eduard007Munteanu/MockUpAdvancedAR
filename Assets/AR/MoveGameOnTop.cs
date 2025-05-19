using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceObjectsFromLayerOnSelf : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float depthSpacing = 0.5f;

    public enum FacingDirection
    {
        North,
        East,
        South,
        West
    }

    [SerializeField] FacingDirection objectFacingDirection = FacingDirection.North;

    private float rotationOffset = 90.0f;
    private float xOffset = 0.0f;
    private float zOffset = 0.03f;
    private float yOffset = - 0.07f;

    private void Start()
    {
        StartCoroutine(WaitForTableReadyAndPlace());
    }

    private IEnumerator WaitForTableReadyAndPlace()
    {
        Renderer selfRenderer = GetComponentInChildren<Renderer>();
        if (selfRenderer == null)
        {
            Debug.LogWarning("Missing Renderer on the base object.");
            yield break;
        }

        // Wait until the table has a usable size (i.e., XR system has scaled it)
        while (selfRenderer.bounds.size.magnitude < 0.1f)
            yield return null;

        yield return new WaitForSeconds(0.1f); // optional safety delay

        PlaceObjects(selfRenderer);
    }

    private void PlaceObjects(Renderer selfRenderer)
    {
        Bounds selfBounds = selfRenderer.bounds;
        float topY = selfBounds.max.y;
        Vector3 selfCenter = selfBounds.center;

        List<GameObject> objectsToPlace = new List<GameObject>();

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (((1 << obj.layer) & targetLayer) == 0 || !obj.activeInHierarchy || obj == gameObject)
                continue;

            if (obj.GetComponentInChildren<Renderer>() == null)
            {
                Debug.LogWarning($"Object '{obj.name}' has no Renderer; skipping.");
                continue;
            }

            objectsToPlace.Add(obj);
        }

        if (objectsToPlace.Count == 0)
        {
            Debug.Log("No valid objects found on the target layer.");
            return;
        }

        Quaternion baseRotation = transform.rotation * GetRotationFromDirection(objectFacingDirection) * Quaternion.Euler(0, rotationOffset, 0);

        foreach (GameObject obj in objectsToPlace)
        {
            // 1. Scale uniformly to fit within table bounds
            Bounds originalBounds = GetTotalBounds(obj);
            float widthRatio = selfBounds.size.x / originalBounds.size.x;
            float depthRatio = selfBounds.size.z / originalBounds.size.z;
            float fitScaleRatio = Mathf.Min(widthRatio, depthRatio);
            fitScaleRatio = Mathf.Clamp(fitScaleRatio, 0.2f, 1.0f); // prevent absurd scaling

            obj.transform.localScale *= fitScaleRatio;

            // 2. Apply rotation before bounds re-check
            obj.transform.rotation = baseRotation;

            // 3. Get bounds after scaling/rotation
            Bounds groupBounds = GetTotalBounds(obj);

            // 4. Compute offset from pivot to bounds center
            Vector3 pivotToCenterOffset = groupBounds.center - obj.transform.position;

            // 5. Target world position
            Vector3 targetPos = selfCenter - new Vector3(pivotToCenterOffset.x + xOffset, 0, pivotToCenterOffset.z + zOffset);

            // 6. Align bottom to tabletop
            float verticalOffset = selfBounds.max.y - groupBounds.min.y;
            targetPos.y = obj.transform.position.y + verticalOffset + yOffset;

            obj.transform.position = targetPos;

            Debug.Log($"Placed '{obj.name}' at {obj.transform.position} with bounds {groupBounds.size} and scale {obj.transform.localScale}");
        }
    }

    private Quaternion GetRotationFromDirection(FacingDirection direction)
    {
        switch (direction)
        {
            case FacingDirection.North: return Quaternion.Euler(0, 0, 0);
            case FacingDirection.East:  return Quaternion.Euler(0, 90, 0);
            case FacingDirection.South: return Quaternion.Euler(0, 180, 0);
            case FacingDirection.West:  return Quaternion.Euler(0, 270, 0);
            default: return Quaternion.identity;
        }
    }

    private Bounds GetTotalBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds totalBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            totalBounds.Encapsulate(renderers[i].bounds);

        return totalBounds;
    }
}
