using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    private void Update()
    {
        // Require a new tap, so a finger that's still down when dying doesn't restart instantly
        if (TapInput.WasPressedThisFrame())
        {
            RedMan.restarted = true;
            SceneManager.LoadScene("Main");
        }
    }
}
