using UnityEngine;
using System.Collections;
using System.Collections.Generic; // If you want to queue multiple notifications

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("The UI Prefab for the notification message.")]
    public GameObject notificationPrefab;
    [Tooltip("Assign the main camera here. If not assigned, it will try to find Camera.main.")]
    public Transform mainCameraTransform;

    [Header("Display Settings")]
    [Tooltip("How far in front of the camera the notification should appear.")]
    public float displayDistance = 1.5f;
    [Tooltip("Vertical offset from the camera's forward vector center.")]
    public float displayOffsetY = 0.3f; // Adjust as needed to position above center line of sight
    [Tooltip("Horizontal offset from the camera's forward vector center.")]
    public float displayOffsetX = 0f;
    [Tooltip("How long the fade in/out animations should last.")]
    public float defaultFadeDuration = 0.3f;
    [Tooltip("Default time the notification stays fully visible (excluding fades).")]
    public float defaultHoldDuration = 2.5f;


    private NotificationItem currentNotificationItem;
    private Queue<NotificationArgs> notificationQueue = new Queue<NotificationArgs>();
    private bool isDisplayingNotification = false;


    private struct NotificationArgs
    {
        public string title;
        public string subtitle;
        public float holdTime;
        public float fadeInTime;
        public float fadeOutTime;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want it to persist across scenes
        }

        if (mainCameraTransform == null)
        {
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError("NotificationManager: Main camera not found and not assigned!");
                enabled = false; // Disable if no camera
            }
        }
    }

    void Update()
    {
        if (!isDisplayingNotification && notificationQueue.Count > 0)
        {
            StartCoroutine(ProcessQueue());
        }
    }
    
    private IEnumerator ProcessQueue()
    {
        isDisplayingNotification = true;
        NotificationArgs args = notificationQueue.Dequeue();

        if (notificationPrefab == null)
        {
            Debug.LogError("NotificationManager: Notification Prefab is not assigned!");
            isDisplayingNotification = false;
            yield break;
        }

        GameObject notificationGO = Instantiate(notificationPrefab);
        currentNotificationItem = notificationGO.GetComponent<NotificationItem>();

        if (currentNotificationItem == null)
        {
            Debug.LogError("NotificationManager: NotificationItem script not found on the prefab!");
            Destroy(notificationGO);
            isDisplayingNotification = false;
            yield break;
        }
        
        // Initial position before setup (will be updated in LateUpdate by NotificationItem)
        PlaceNotification(notificationGO.transform);

        currentNotificationItem.Show(args.title, args.subtitle, args.holdTime, args.fadeInTime, args.fadeOutTime, mainCameraTransform, displayDistance, displayOffsetY, displayOffsetX);
        
        // Wait for the current notification to finish (including fade out)
        yield return new WaitUntil(() => currentNotificationItem == null || !currentNotificationItem.IsActive);
        
        isDisplayingNotification = false; // Ready for the next one
    }


    /// <summary>
    /// Shows a notification message to the player.
    /// </summary>
    /// <param name="title">The main message (e.g., 'Arts Advancement').</param>
    /// <param name="subtitle">The secondary message (e.g., 'Draw a card!').</param>
    /// <param name="holdDuration">How long the message stays fully visible (optional, uses default if -1).</param>
    /// <param name="fadeDuration">How long fade in/out takes (optional, uses default if -1).</param>
    public void ShowNotification(string title, string subtitle, float holdDuration = -1f, float fadeDuration = -1f)
    {
        if (holdDuration < 0) holdDuration = defaultHoldDuration;
        if (fadeDuration < 0) fadeDuration = defaultFadeDuration;

        notificationQueue.Enqueue(new NotificationArgs
        {
            title = title,
            subtitle = subtitle,
            holdTime = holdDuration,
            fadeInTime = fadeDuration, // Using the same duration for in and out for simplicity
            fadeOutTime = fadeDuration
        });
    }
    
    private void PlaceNotification(Transform notificationTransform)
    {
        if (mainCameraTransform == null || notificationTransform == null) return;

        Vector3 position = mainCameraTransform.position +
                           (mainCameraTransform.forward * displayDistance) +
                           (mainCameraTransform.up * displayOffsetY) +
                           (mainCameraTransform.right * displayOffsetX);
        notificationTransform.position = position;
        notificationTransform.LookAt(notificationTransform.position + mainCameraTransform.rotation * Vector3.forward,
                                     mainCameraTransform.rotation * Vector3.up);
    }


    // Called by NotificationItem when it's destroyed or finished
    public void OnNotificationCompleted(NotificationItem item)
    {
        if (currentNotificationItem == item)
        {
            currentNotificationItem = null;
            // isDisplayingNotification will be set to false after the coroutine wait finishes
        }
    }
}