using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PaintingDangler : MonoBehaviour
{
    [Header("Dangling Settings")]
    [SerializeField] private float danglingForce = 5f;    // Force applied when initialized
    [SerializeField] private float stabilizationSpeed = 2f;  // How quickly the painting stabilizes
    [SerializeField] private float maxDangleAngle = 15f;    // Maximum angle the painting can dangle
    [SerializeField] private float danglingFriction = 0.95f;  // Friction to slow down the swinging

    [Header("Random Movement")]
    [SerializeField] private bool enableRandomMovement = true;  // Enable subtle random movement
    [SerializeField] private float randomMovementStrength = 0.3f;  // Strength of random movement
    [SerializeField] private float randomMovementFrequency = 0.2f;  // How often random movement occurs

    [Header("Physics References")]
    [SerializeField] private Transform hangPoint;  // The point where the painting hangs from (top center)

    // Private variables
    private Rigidbody rb;
    private float currentAngularVelocity = 0f;
    private float targetRotation = 0f;
    private float timeSinceLastRandomMovement = 0f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private ConfigurableJoint joint;

    private void Awake()
    {
        // Get or add required components
        rb = GetComponent<Rigidbody>();

        // Set up physics
        SetupPhysics();
    }

    private void Start()
    {
        // Store original position and rotation for reference
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Apply initial force to make it dangle
        StartCoroutine(InitialDangle());
    }

    private void SetupPhysics()
    {
        // Configure the rigidbody for realistic painting physics
        rb.useGravity = false;
        rb.mass = 1f;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.constraints = RigidbodyConstraints.FreezePosition |
                        RigidbodyConstraints.FreezeRotationY |
                        RigidbodyConstraints.FreezeRotationZ;

        // Create hang point if not assigned
        if (hangPoint == null)
        {
            GameObject hangPointObj = new GameObject("HangPoint");
            hangPoint = hangPointObj.transform;
            hangPoint.SetParent(transform.parent);

            // Position it above the painting
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector3 topCenter = renderer.bounds.center + new Vector3(0, renderer.bounds.extents.y, 0);
                hangPoint.position = topCenter;
            }
            else
            {
                hangPoint.position = transform.position + new Vector3(0, 0.5f, 0);
            }
        }

        // Create a configurable joint to simulate hanging
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = null;
        joint.anchor = transform.InverseTransformPoint(hangPoint.position);

        // Lock all motion except rotation around Z-axis
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        // Create limits for the Z rotation (dangling angle)
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = maxDangleAngle;
        joint.lowAngularXLimit = limit;
        limit.limit = maxDangleAngle;
        joint.highAngularXLimit = limit;

        // Add damping and spring
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
        JointDrive drive = new JointDrive();
        drive.positionSpring = 10f;
        drive.positionDamper = 1f;
        drive.maximumForce = 10f;
        joint.zDrive = drive;
    }

    private IEnumerator InitialDangle()
    {
        // Wait a frame to make sure everything is set up
        yield return null;

        // Apply random initial force to make it dangle
        float randomForce = Random.Range(-danglingForce, danglingForce);
        rb.AddTorque(new Vector3(0, 0, randomForce), ForceMode.Impulse);

        // Enable collisions after a short delay
        // This prevents paintings from affecting each other when multiple spawn at once
        yield return new WaitForSeconds(0.5f);
        GetComponent<Collider>().enabled = true;
    }

    private void Update()
    {
        // Apply random movement occasionally if enabled
        if (enableRandomMovement)
        {
            ApplyRandomMovement();
        }
    }

    private void ApplyRandomMovement()
    {
        timeSinceLastRandomMovement += Time.deltaTime;

        // Check if it's time for random movement
        if (timeSinceLastRandomMovement >= 1f / randomMovementFrequency)
        {
            // Only apply random movement if the painting is relatively still
            if (Mathf.Abs(rb.angularVelocity.z) < 0.1f)
            {
                // Random chance to apply movement
                if (Random.value < 0.3f)
                {
                    float randomTorque = Random.Range(-randomMovementStrength, randomMovementStrength);
                    rb.AddTorque(new Vector3(0, 0, randomTorque), ForceMode.Impulse);
                }
            }

            timeSinceLastRandomMovement = 0f;
        }
    }

    // Function to make the painting dangle when external forces interact with it
    public void Disturb(float force)
    {
        rb.AddTorque(new Vector3(0, 0, force), ForceMode.Impulse);
    }

    // Optional: When another object hits the painting
    private void OnCollisionEnter(Collision collision)
    {
        // Calculate impact force direction and magnitude
        Vector3 impactForce = collision.impulse / Time.fixedDeltaTime;
        float zTorque = impactForce.magnitude * 0.01f * Mathf.Sign(Random.value - 0.5f);

        // Apply the force to make the painting dangle
        Disturb(zTorque);
    }

    // Optional: Apply force when player is near (can be called from other scripts)
    public void PlayerNearby(Vector3 playerPosition, float influenceRadius = 1.5f)
    {
        float distance = Vector3.Distance(transform.position, playerPosition);

        if (distance < influenceRadius)
        {
            // Calculate direction and strength
            Vector3 direction = transform.position - playerPosition;
            float strength = (1 - (distance / influenceRadius)) * 0.5f;

            // Apply subtle force
            Disturb(strength * Mathf.Sign(direction.x));
        }
    }
}