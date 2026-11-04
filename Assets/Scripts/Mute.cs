using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Mute : MonoBehaviour
{
    private const string MutedKey = "Muted";

    public static bool muted;
    public Sprite muteSprite;
    public Sprite mutedSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        muted = PlayerPrefs.GetInt(MutedKey) == 1;

        ApplyMuted();
    }

    public void MuteSound()
    {
        muted = !muted;

        ApplyMuted();

        // Save the muted state to PlayerPrefs
        PlayerPrefs.SetInt(MutedKey, muted ? 1 : 0);
        PlayerPrefs.Save();

        // Deselect so Space (Submit) doesn't toggle the button again
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void ApplyMuted()
    {
        AudioListener.volume = muted ? 0 : 1;

        image.sprite = muted ? mutedSprite : muteSprite;
    }
}
