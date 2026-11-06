using System.Collections;
using UnityEngine;

// Slows down all moving objects for a while
public class TimePowerUp : PowerUpBase
{
    private const float Duration = 3.0f;

    // The hourglass whose slow-down is currently running
    private static TimePowerUp active;

    private float endTime;

    public override void Activate(RedMan redMan)
    {
        // Picking up another hourglass while slowed down extends the duration
        if (active != null)
        {
            active.endTime = Time.time + Duration;
            Destroy(gameObject);
            return;
        }

        active = this;
        endTime = Time.time + Duration;
        Hide();
        StartCoroutine(SlowDown(redMan));
    }

    private IEnumerator SlowDown(RedMan redMan)
    {
        redMan.SetSlowedDown(true);

        while (Time.time < endTime)
        {
            yield return null;
        }

        redMan.SetSlowedDown(false);
        Destroy(gameObject);
    }

    // The picked up hourglass stays alive invisibly to run the slow-down
    private void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
    }
}
