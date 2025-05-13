using UnityEngine;

public class PaintingSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    public float pivotOffsetPercent = 0.1f;   // 10% from the top
    public float initialKickStrength = 50f;   // max starting angle
    public float maxRandomKick = 30f;         // max angle added during a random impulse
    public float swingDamping = 1f;           // how quickly it slows down (0 = never)
    public float swingSpeed = 3f;             // how fast it swings

    [Header("Impulse Timing")]
    public float minImpulseInterval = 3f;
    public float maxImpulseInterval = 7f;

    private Transform pivot;
    private float angle = 0f;
    private float angularVelocity = 0f;
    private float nextImpulseTime;

    void Start()
    {
        CreatePivot();
        ApplyImpulse(Random.Range(-initialKickStrength, initialKickStrength));
        ScheduleNextImpulse();
    }

    void Update()
    {
        // Pendulum physics
        float dt = Time.deltaTime;

        // Angular acceleration (restoring force)
        float angularAcceleration = -swingSpeed * angle;

        // Apply damping
        angularAcceleration -= swingDamping * angularVelocity;

        // Integrate
        angularVelocity += angularAcceleration * dt;
        angle += angularVelocity * dt;

        // Apply rotation
        pivot.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Apply random impulses
        if (Time.time >= nextImpulseTime)
        {
            ApplyImpulse(Random.Range(-maxRandomKick, maxRandomKick));
            ScheduleNextImpulse();
        }
    }

    void ApplyImpulse(float impulse)
    {
        angularVelocity += impulse;
    }

    void ScheduleNextImpulse()
    {
        nextImpulseTime = Time.time + Random.Range(minImpulseInterval, maxImpulseInterval);
    }

    void CreatePivot()
    {
        Renderer rend = GetComponent<Renderer>();
        Vector3 size = rend.bounds.size;

        Vector3 offset = new Vector3(0f, size.y * (0.5f - pivotOffsetPercent), 0f);
        Vector3 worldPivot = transform.position + offset;

        pivot = new GameObject("SwingPivot").transform;
        pivot.position = worldPivot;
        transform.SetParent(pivot);
    }

    void OnDestroy()
    {
        if (pivot) Destroy(pivot.gameObject);
    }
}
