using UnityEngine;

public class PlaceObjectsFromLayerOnSelf : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;

    void Start()
    {
        // get the top surface of this object in world space
        Renderer selfRenderer = GetComponentInChildren<Renderer>();
        if (selfRenderer == null)
        {
            Debug.LogWarning("Missing Renderer on the base object.");
            return;
        }

        Bounds selfBounds = selfRenderer.bounds;
        float topY = selfBounds.max.y;
        Vector2 centerXZ = new Vector2(selfBounds.center.x, selfBounds.center.z);

        // find every active object on the target layer
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

            Bounds placeBounds = placeRenderer.bounds;
            Vector3 originalPos = obj.transform.position;

            // compute new Y so bottom of obj aligns with table top
            float newY = topY + (originalPos.y - placeBounds.min.y);

            // set final position & rotation
            obj.transform.position = new Vector3(centerXZ.x, newY, centerXZ.y);
            obj.transform.rotation = transform.rotation;

            Debug.Log($"Placed '{obj.name}' at ({centerXZ.x:F2}, {newY:F2}, {centerXZ.y:F2})");
        }
    }
}
