using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class TapInput
{
    private static readonly List<RaycastResult> UiHits = new();

    private static bool SpacePressed => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    private static bool HasTouchscreen => Touchscreen.current != null;

    // Space or a new touch this frame
    public static bool WasPressedThisFrame()
    {
        return SpacePressed || (HasTouchscreen && Touchscreen.current.primaryTouch.press.wasPressedThisFrame && !IsButtonTouch());
    }

    // Space pressed this frame or a finger currently on the screen
    public static bool IsPressedOrTouching()
    {
        return SpacePressed || (HasTouchscreen && Touchscreen.current.primaryTouch.press.isPressed && !IsButtonTouch());
    }

    // Only buttons block taps, so taps on text like the score still count
    private static bool IsButtonTouch()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            return false;
        }

        var data = new PointerEventData(eventSystem) { position = Touchscreen.current.primaryTouch.position.ReadValue() };
        eventSystem.RaycastAll(data, UiHits);
        foreach (var hit in UiHits)
        {
            if (hit.gameObject.GetComponentInParent<Selectable>() != null)
            {
                return true;
            }
        }

        return false;
    }
}
