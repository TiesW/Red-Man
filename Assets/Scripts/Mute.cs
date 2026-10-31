using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mute : MonoBehaviour
{
    public static bool muted;
    public Sprite muteSprite;
    public Sprite mutedSprite;

    private void Awake()
    {
        gameObject.GetComponent<Image>().sprite = muted ? mutedSprite : muteSprite;
    }

    public void MuteSound()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0 : 1;
        gameObject.GetComponent<Image>().sprite = muted ? mutedSprite : muteSprite;
    }
}
