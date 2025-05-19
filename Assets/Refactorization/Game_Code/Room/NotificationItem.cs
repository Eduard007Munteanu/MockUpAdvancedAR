using UnityEngine;
using TMPro; // Make sure TextMeshPro is imported
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class NotificationItem : MonoBehaviour
{
    [Tooltip("TextMeshPro UGUI element for the title text.")]
    public TextMeshProUGUI titleText;
    [Tooltip("TextMeshPro UGUI element for the subtitle text.")]
    public TextMeshProUGUI subtitleText;

    private CanvasGroup canvasGroup;
    private Coroutine displayCoroutine;
    
    private Transform cameraTransform;
    private float camDistance;
    private float camOffsetY;
    private float camOffsetX;

    public bool IsActive { get; private set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("NotificationItem requires a CanvasGroup component!");
            enabled = false;
        }
    }

    public void Show(string title, string sub, float holdTime, float fadeInTime, float fadeOutTime, Transform camTransform, float distance, float offsetY, float offsetX)
    {
        if (titleText == null || subtitleText == null)
        {
            Debug.LogError("NotificationItem: TitleText or SubtitleText not assigned!");
            Destroy(gameObject);
            return;
        }

        titleText.text = title;
        subtitleText.text = sub;
        this.cameraTransform = camTransform;
        this.camDistance = distance;
        this.camOffsetY = offsetY;
        this.camOffsetX = offsetX;

        IsActive = true;

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        displayCoroutine = StartCoroutine(FadeInHoldAndFadeOut(holdTime, fadeInTime, fadeOutTime));
    }

    private IEnumerator FadeInHoldAndFadeOut(float holdTime, float fadeInTime, float fadeOutTime)
    {
        canvasGroup.alpha = 0f;

        // Fade In
        float timer = 0f;
        while (timer < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade Out
        timer = 0f;
        while (timer < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        
        IsActive = false;
        // Notify manager (optional, if manager needs to know immediately beyond IsActive)
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.OnNotificationCompleted(this);
        }
        Destroy(gameObject);
    }
    
    void LateUpdate()
    {
        if (IsActive && cameraTransform != null)
        {
            Vector3 position = cameraTransform.position +
                               (cameraTransform.forward * camDistance) +
                               (cameraTransform.up * camOffsetY) +
                               (cameraTransform.right * camOffsetX);
            transform.position = position;
            transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                             cameraTransform.rotation * Vector3.up);
        }
    }

    void OnDestroy()
    {
        IsActive = false; // Ensure IsActive is false if destroyed prematurely
        // If it's part of a pool, you wouldn't destroy, but reset and return to pool.
    }
}