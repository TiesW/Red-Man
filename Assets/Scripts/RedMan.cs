using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class RedMan : MonoBehaviour
{
    private const string HighScoreKey = "Highscore";

    // Game config:
    private const float JumpHeight = 4.6f;
    private const float SpawnX = 3.0f;
    private const float MinSpawnInterval = 0.4f;
    private const float MaxSpawnInterval = 0.6f;
    private const float MinPowerUpSpawnInterval = 4.0f;
    private const float MaxPowerUpSpawnInterval = 8.0f;
    private const float MinObjectSpeed = 1.5f;
    private const float MaxObjectSpeed = 3.0f;
    private const float TimePowerUpChance = 0.7f; // the destroy powerup is more rare
    private const float SlowDownDuration = 3.0f;

    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DoubleJumpHash = Animator.StringToHash("DoubleJump");
    private static readonly WaitForSeconds ScoreInterval = new(0.1f);

    public static bool restarted = false;

    public Rigidbody2D square;
    public Rigidbody2D timePowerUp;
    public Rigidbody2D destroyPowerUp;
    public GameObject particles;
    public TMP_Text score;
    public TMP_Text highScore;
    public TMP_Text start;
    public TMP_Text restart;
    public AudioSource jumpSound;
    public AudioSource explodeSound;
    public AudioSource deathSound;

    private Rigidbody2D rig;
    private Animator anim;
    private Camera cam;
    private bool jumping;
    private bool doubleJumping;
    private bool jumpRequested;
    private bool gameRunning;
    private bool gameLost;
    private int scoreAmount;
    private int highScoreAmount;

    // Blocks and powerups that are currently moving across the screen
    private readonly List<Rigidbody2D> movingObjects = new();
    private float speedMultiplier = 1f;
    private float slowDownEndTime;
    private Coroutine slowDownRoutine;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        highScoreAmount = PlayerPrefs.GetInt(HighScoreKey);
        highScore.text = "HS: " + highScoreAmount;
    }

    private void Update()
    {
        if (gameLost)
        {
            return;
        }

        if (gameRunning)
        {
            if (TapInput.WasPressedThisFrame())
            {
                TryJump();
            }
        }
        else if (TapInput.IsPressedOrTouching() || restarted)
        {
            StartGame();
        }
    }

    // Physics changes (like velocity) are done in FixedUpdate, input is handled in Update
    private void FixedUpdate()
    {
        if (jumpRequested)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, JumpHeight);
            jumpRequested = false;
            jumpSound.Play();
        }
    }

    private void StartGame()
    {
        start.gameObject.SetActive(false);
        gameRunning = true;
        anim.SetBool(RunHash, true);
        anim.SetBool(JumpHash, false);

        StartCoroutine(ScoreCounter());
        StartCoroutine(SpawnBlocks());
        StartCoroutine(SpawnPowerUps());
    }

    private void TryJump()
    {
        if (!jumping)
        {
            jumping = true;
            anim.SetBool(JumpHash, true);
        }
        else if (!doubleJumping)
        {
            doubleJumping = true;
            anim.SetBool(DoubleJumpHash, true);
            anim.SetBool(JumpHash, false);
        }
        else
        {
            return;
        }

        anim.SetBool(RunHash, false);
        jumpRequested = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameLost)
        {
            return;
        }

        if (collision.gameObject.name == "redManBG")
        {
            // Landed on the ground
            jumping = false;
            doubleJumping = false;
            if (gameRunning)
            {
                anim.SetBool(RunHash, true);
                anim.SetBool(JumpHash, false);
            }
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        gameRunning = false;
        gameLost = true;
        Instantiate(particles, transform.position, Quaternion.identity);
        deathSound.Play();
        gameObject.SetActive(false);
        restart.gameObject.SetActive(true);

        if (scoreAmount > PlayerPrefs.GetInt(HighScoreKey))
        {
            PlayerPrefs.SetInt(HighScoreKey, scoreAmount);
            PlayerPrefs.Save();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var isTimePowerUp = collision.gameObject.name.Contains("Time");
        movingObjects.Remove(collision.attachedRigidbody);
        Destroy(collision.gameObject);

        if (isTimePowerUp)
        {
            SlowDownObjects();
        }
        else
        {
            ExplodeObjects();
        }
    }

    private void ExplodeObjects()
    {
        movingObjects.RemoveAll(o => o == null);
        if (movingObjects.Count == 0)
        {
            return;
        }

        foreach (var obj in movingObjects)
        {
            Instantiate(particles, obj.position, Quaternion.identity);
            Destroy(obj.gameObject);
        }
        movingObjects.Clear();
        explodeSound.Play();
    }

    private Rigidbody2D Spawn(Rigidbody2D prefab, float y, float speed)
    {
        // Drop objects that have been destroyed off-screen
        movingObjects.RemoveAll(x => x == null);

        var clone = Instantiate(prefab, new Vector3(SpawnX, y, 0), Quaternion.identity);
        clone.linearVelocity = Vector2.left * (speed * speedMultiplier);
        movingObjects.Add(clone);
        return clone;
    }

    private IEnumerator SpawnBlocks()
    {
        while (gameRunning)
        {
            // The shorter the interval, the less time between blocks spawning
            yield return new WaitForSeconds(Random.Range(MinSpawnInterval, MaxSpawnInterval));

            var topY = cam.ScreenToWorldPoint(new Vector2(0, Screen.height)).y;
            var block = Spawn(square, Random.Range(-0.13f, topY), Random.Range(MinObjectSpeed, MaxObjectSpeed));
            block.transform.localScale = new Vector3(Random.Range(0.27f, 0.36f), Random.Range(0.1f, 0.29f));
        }
    }

    private IEnumerator SpawnPowerUps()
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(Random.Range(MinPowerUpSpawnInterval, MaxPowerUpSpawnInterval));

            var powerUp = Random.value < TimePowerUpChance ? timePowerUp : destroyPowerUp;
            // Only spawn powerups at heights the player can reach
            Spawn(powerUp, Random.Range(-0.13f, 1.54f), Random.Range(MinObjectSpeed, MaxObjectSpeed));
        }
    }

    private void SlowDownObjects()
    {
        // Picking up another hourglass while slowed down extends the duration
        slowDownEndTime = Time.time + SlowDownDuration;

        slowDownRoutine ??= StartCoroutine(SlowDownRoutine());
    }

    private IEnumerator SlowDownRoutine()
    {
        // Halving each object's speed makes them slow down relative to their own speed. Objects spawned in the meantime use the multiplier too.
        ScaleObjectSpeeds(0.5f);

        while (Time.time < slowDownEndTime)
        {
            yield return null;
        }

        ScaleObjectSpeeds(2f);
        slowDownRoutine = null;
    }

    private void ScaleObjectSpeeds(float factor)
    {
        speedMultiplier *= factor;
        movingObjects.RemoveAll(o => o == null);

        foreach (var obj in movingObjects)
        {
            obj.linearVelocity = new Vector2(obj.linearVelocity.x * factor, 0f);
        }
    }

    private IEnumerator ScoreCounter()
    {
        while (gameRunning)
        {
            scoreAmount++;
            score.text = "S: " + scoreAmount;

            if (scoreAmount > highScoreAmount)
            {
                highScoreAmount = scoreAmount;
                highScore.text = "HS: " + highScoreAmount;
            }

            yield return ScoreInterval;
        }
    }
}
