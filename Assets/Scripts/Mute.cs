using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Mute : MonoBehaviour
{
    public static bool muted;
    public Sprite muteSprite;
    public Sprite mutedSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        UpdateSprite();
    }

    public void MuteSound()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0 : 1;
        UpdateSprite();

        // Deselect so Space (Submit) doesn't toggle the button again
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void UpdateSprite()
    {
        image.sprite = muted ? mutedSprite : muteSprite;
    }
}
