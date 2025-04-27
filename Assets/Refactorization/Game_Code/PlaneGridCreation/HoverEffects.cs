using UnityEngine;

public class HoverEffects : MonoBehaviour
{
    public GameObject effectObject; // Assign VFX, outline, glow, etc.

    public void EnableEffect()
    {
        if (effectObject != null)
            effectObject.SetActive(true);
    }

    public void DisableEffect()
    {
        if (effectObject != null)
            effectObject.SetActive(false);
    }

    private void Start()
    {
        if (effectObject != null)
            effectObject.SetActive(false); // start hidden
    }
}
