using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount >= 1)
        {
            RedMan.restarted = true;
            SceneManager.LoadScene("Main");
        }
    }
}
