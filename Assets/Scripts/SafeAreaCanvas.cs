using UnityEngine;

// Fits the world space canvas to the part of the camera view that isn't covered by notches, status bars, etc.
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
public class SafeAreaCanvas : MonoBehaviour
{
    public Camera cam;

    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        Apply();
    }

    private void Update()
    {
        // Safe area can change on rotation or when system bars appear/disappear
        if (Screen.safeArea != lastSafeArea || Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            Apply();
        }
    }

    private void Apply()
    {
        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        var min = cam.ScreenToWorldPoint(lastSafeArea.min);
        var max = cam.ScreenToWorldPoint(lastSafeArea.max);
        var scale = rectTransform.lossyScale;

        rectTransform.position = new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, rectTransform.position.z);
        rectTransform.sizeDelta = new Vector2((max.x - min.x) / scale.x, (max.y - min.y) / scale.y);
    }
}
