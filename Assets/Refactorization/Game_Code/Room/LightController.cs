using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Light References")]
    [Tooltip("Drag the Candle Light GameObjects (or objects with Light components) here.")]
    public List<Light> candleLights = new List<Light>();

    [Header("Master Intensity Control")]
    [Tooltip("Global base intensity for all candles. Flicker will be relative to this.")]
    [Range(0f, 10f)] // Adjust max range as you see fit for your project
    public float masterBaseIntensity = 1.0f;
    private float _previousMasterBaseIntensity; // For Inspector updates

    [Header("Flicker Parameters")]
    [Tooltip("Minimum intensity multiplier (e.g., 0.7 means 70% of its base intensity).")]
    [Range(0f, 1f)]
    public float minFlickerMultiplier = 0.7f;

    [Tooltip("Maximum intensity multiplier (e.g., 1.3 means 130% of its base intensity).")]
    [Range(1f, 3f)] // Candles can sometimes flare up
    public float maxFlickerMultiplier = 1.3f;

    [Tooltip("Overall speed of the flicker effect.")]
    public float flickerSpeed = 5.0f;

    [Tooltip("How smoothly the light transitions to new flicker values. Smaller values = faster/jerkier, larger values = slower/smoother.")]
    [Range(0.01f, 1.0f)]
    public float flickerSmoothness = 0.1f; // Represents roughly the time it takes to reach the target

    // Internal lists to manage individual light properties
    private List<float> _lightBaseIntensities = new List<float>();
    private List<float> _perlinNoiseSeeds = new List<float>();

    private ResourceDatabase resources;
    private float happiness;
    private float agreement;

    void Awake()
    {
        while (resources == null)
        {
            Debug.Log("Waiting for ResourceDatabase to be initialized...");
            resources = ResourceDatabase.Instance;
        }

    }

    void CalculateBaseIntensity(ResourceType type, float delta)
    {
        if (type == ResourceType.Happiness)
        {
            happiness = happiness + delta; // Assuming happiness is a percentage
            happiness = resources[ResourceType.Happiness].CurrentAmount / 5;
            SetSpecificCandleBaseIntensity(0, happiness/100f);
        }
        else if (type == ResourceType.Agreement)
        {
            agreement = agreement + delta; // Assuming agreement is a percentage
            agreement = resources[ResourceType.Agreement].CurrentAmount / 5;
            SetSpecificCandleBaseIntensity(1, agreement/100f);
        }
    }

    void Start()
    {
        if (candleLights.Count == 0)
        {
            Debug.LogWarning("RoomController: No candle lights have been assigned to the candleLights list.", this);
            // You might want to disable the script if no lights are present
            // enabled = false;
            // return;
        }

        _previousMasterBaseIntensity = masterBaseIntensity;

        for (int i = 0; i < candleLights.Count; i++)
        {
            Light currentLight = candleLights[i];
            if (currentLight == null)
            {
                Debug.LogError($"RoomController: Light at index {i} in candleLights list is null. Please assign it in the Inspector.", this);
                // Add a placeholder to keep list counts aligned, or handle more robustly
                _lightBaseIntensities.Add(0);
                _perlinNoiseSeeds.Add(0);
                continue;
            }

            // Initialize each light's base intensity from the master control
            _lightBaseIntensities.Add(masterBaseIntensity);
            currentLight.intensity = masterBaseIntensity; // Set initial intensity

            // Assign a unique random seed for Perlin noise for each light for variation
            _perlinNoiseSeeds.Add(Random.Range(0f, 1000f));
        }

        happiness = resources[ResourceType.Happiness].CurrentAmount;
        agreement = resources[ResourceType.Agreement].CurrentAmount;

        Debug.Log($"RoomController: Initial happiness: {happiness}, agreement: {agreement}");

        resources[ResourceType.Agreement].OnAmountChanged += CalculateBaseIntensity;
        resources[ResourceType.Happiness].OnAmountChanged += CalculateBaseIntensity;
    }

    void Update()
    {
        // Check if the master base intensity has changed in the Inspector
        if (masterBaseIntensity != _previousMasterBaseIntensity)
        {
            SetMasterBaseIntensity(masterBaseIntensity); // This will update all _lightBaseIntensities
            _previousMasterBaseIntensity = masterBaseIntensity;
        }

        if (candleLights.Count == 0) return; // Nothing to do if no lights

        for (int i = 0; i < candleLights.Count; i++)
        {
            Light currentLight = candleLights[i];
            if (currentLight == null) continue; // Skip if a light in the list is null

            // Use Perlin noise to get a smooth, pseudo-random value between 0 and 1
            // Using two dimensions of Perlin noise (by slightly varying the y-coordinate) can give more complex patterns.
            float noiseSample = Mathf.PerlinNoise(
                _perlinNoiseSeeds[i] + (Time.time * flickerSpeed),
                _perlinNoiseSeeds[i] - (Time.time * flickerSpeed * 0.7f) // Second dimension with different time influence
            );

            // Map the 0-1 noise sample to our desired flicker intensity multiplier range
            float targetIntensityMultiplier = Mathf.Lerp(minFlickerMultiplier, maxFlickerMultiplier, noiseSample);

            // Calculate the target intensity for this flicker moment
            float targetFlickerIntensity = _lightBaseIntensities[i] * targetIntensityMultiplier;

            // Smoothly interpolate the light's current intensity towards the target flicker intensity
            // The division by flickerSmoothness effectively makes flickerSmoothness act like a duration for the lerp
            currentLight.intensity = Mathf.Lerp(currentLight.intensity, targetFlickerIntensity, Time.deltaTime / flickerSmoothness);
        }
        
        CalculateBaseIntensity(ResourceType.Happiness, 0f); // Update the base intensity based on happiness
    }

    /// <summary>
    /// Sets the master base intensity for all candle lights.
    /// Flickering will occur around this new base value.
    /// </summary>
    /// <param name="newMasterIntensity">The new base intensity for all lights.</param>
    public void SetMasterBaseIntensity(float newMasterIntensity)
    {
        masterBaseIntensity = Mathf.Max(0f, newMasterIntensity); // Ensure intensity is not negative
        _previousMasterBaseIntensity = masterBaseIntensity; // Keep Inspector tracking updated

        for (int i = 0; i < candleLights.Count; i++)
        {
            if (candleLights[i] != null)
            {
                _lightBaseIntensities[i] = masterBaseIntensity;
                // Optionally, you could snap the light's current intensity to the new base here,
                // but the flicker in Update() will naturally adjust it over time.
                // candleLights[i].intensity = masterBaseIntensity;
            }
        }
    }

    /// <summary>
    /// Sets the base intensity for a specific candle light in the list.
    /// Note: This will make this light's base intensity independent of the MasterBaseIntensity until MasterBaseIntensity is changed again.
    /// </summary>
    /// <param name="lightIndex">The index of the light in the candleLights list.</param>
    /// <param name="newSpecificBaseIntensity">The new base intensity for this light.</param>
    public void SetSpecificCandleBaseIntensity(int lightIndex, float newSpecificBaseIntensity)
    {
        if (lightIndex >= 0 && lightIndex < candleLights.Count && candleLights[lightIndex] != null && lightIndex < _lightBaseIntensities.Count)
        {
            _lightBaseIntensities[lightIndex] = Mathf.Max(0f, newSpecificBaseIntensity);
        }
        else
        {
            Debug.LogWarning($"RoomController: Could not set specific base intensity. Invalid light index: {lightIndex} or light is null.", this);
        }
    }

    /// <summary>
    /// Gets the current (flickering) intensity of a specific candle light.
    /// </summary>
    /// <param name="lightIndex">The index of the light in the candleLights list.</param>
    /// <returns>The current intensity, or -1f if the light is not found or index is invalid.</returns>
    public float GetCurrentCandleIntensity(int lightIndex)
    {
        if (lightIndex >= 0 && lightIndex < candleLights.Count && candleLights[lightIndex] != null)
        {
            return candleLights[lightIndex].intensity;
        }
        return -1f;
    }

    /// <summary>
    /// Gets the base intensity (around which flickering occurs) of a specific candle light.
    /// </summary>
    /// <param name="lightIndex">The index of the light in the candleLights list.</param>
    /// <returns>The base intensity, or -1f if the light is not found or index is invalid.</returns>
    public float GetBaseCandleIntensity(int lightIndex)
    {
        if (lightIndex >= 0 && lightIndex < _lightBaseIntensities.Count) // Assuming _lightBaseIntensities is synced
        {
            return _lightBaseIntensities[lightIndex];
        }
        return -1f;
    }
}