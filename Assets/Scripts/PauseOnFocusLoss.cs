using UnityEngine;

// Freezes the game and its audio whenever the app isn't in the foreground (e.g. the Android recent apps view)
public class PauseOnFocusLoss : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        var go = new GameObject(nameof(PauseOnFocusLoss));
        go.AddComponent<PauseOnFocusLoss>();
        DontDestroyOnLoad(go);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetPaused(!hasFocus);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        SetPaused(pauseStatus);
    }

    private static void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0 : 1;
        AudioListener.pause = paused;
    }
}
