using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RedMan : MonoBehaviour
{
    Rigidbody2D rig;
    Animator anim;
    bool jumping = false;
    bool doubleJumping = false;
    bool jump = false;
    bool doubleJump = false;
    bool gameRunning = false;
    bool gameLost = false;
    bool spawnNext = true;
    bool spawnNextPowerUp = true;
    bool particlesAppeared = false;
    public static bool restarted = false;
    System.Random rnd;
    float blockYPos;
    float powerUpYPos;
    float blockSpeed;


    //Game config:
    float jumpHeight = 4.6f;

    float minSpawnSpeed = 0.4f;
    float maxSpawnSpeed = 0.6f;
    float minPoSpawnSpeed = 4.0f;
    float maxPoSpawnSpeed = 8.0f;



    int scoreAmount = 0;
    static int highScoreAmount;

    public Rigidbody2D square;
    public Rigidbody2D powerUp1;
    public Rigidbody2D powerUp2;
    public GameObject particles;
    public TMP_Text score;
    public TMP_Text highScore;
    public TMP_Text start;
    public TMP_Text restart;
    public AudioSource sound;
    public AudioSource sound2;
    public AudioSource sound3;



    private void Start()
    {
        rnd = new System.Random();
        anim = gameObject.GetComponent<Animator>();
        rig = gameObject.GetComponent<Rigidbody2D>();

        highScoreAmount = PlayerPrefs.GetInt("Highscore");
        highScore.text = "HS: " + highScoreAmount.ToString();
    }

    void Update()
    {
        if (!gameLost)
        {
            if (gameRunning)
            {
                if (Input.touchCount >= 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        if (!jumping)
                        {
                            anim.SetBool("Jump", true);
                            anim.SetBool("Run", false);

                            jump = true;
                        }
                        else if (jumping && !doubleJumping)
                        {
                            anim.SetBool("DoubleJump", true);
                            anim.SetBool("Jump", false);
                            anim.SetBool("Run", false);
                            
                            doubleJump = true;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!jumping)
                    {
                        anim.SetBool("Jump", true);
                        anim.SetBool("Run", false);

                        jump = true;
                    }
                    else if (jumping && !doubleJumping)
                    {
                        anim.SetBool("DoubleJump", true);
                        anim.SetBool("Jump", false);
                        anim.SetBool("Run", false);

                        doubleJump = true;
                    }
                }

                if (spawnNext)
                {
                    StartCoroutine("SpawnBlocks");
                }
                if (spawnNextPowerUp)
                {
                    StartCoroutine("SpawnPowerUps");
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount >= 1 || restarted)
                {
                    start.gameObject.SetActive(false);
                    gameRunning = true;
                    anim.SetBool("Run", true);
                    anim.SetBool("Jump", false);
                    StartCoroutine("ScoreCounter");
                }
            }
        }
    }

    private void FixedUpdate() //Things involving physics (e.g. changing velocity) should be done in FixedUpdate rather than Update (which is better for input checks).
    {
        if (jump)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpHeight);
            jump = false;
            jumping = true; //Returns to false as soon as the player hits the ground.
            sound.Play();
        }
        if (doubleJump)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpHeight);
            doubleJump = false;
            doubleJumping = true;
            sound.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.name == "redManBG")
        {
            jumping = false;
            doubleJumping = false;
            if (gameRunning)
            {
                anim.SetBool("Run", true);
                anim.SetBool("Jump", false);
            }
        } else
        {
            gameRunning = false;
            gameLost = true;
            if (!particlesAppeared)
            {
                Instantiate(particles, gameObject.transform.position, Quaternion.identity);
            }
            particlesAppeared = true;
            sound3.Play();
            gameObject.SetActive(false);
            restart.gameObject.SetActive(true);
            //anim.SetBool("Died", true);

            if (scoreAmount > PlayerPrefs.GetInt("Highscore"))
            {
                PlayerPrefs.SetInt("Highscore", highScoreAmount);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Time"))
        {
            Destroy(collision.gameObject);
            StartCoroutine("SlowDownBlocks");
        } else
        {
            Destroy(collision.gameObject);
            ExplodeBlocks();
        }
    }

    void ExplodeBlocks()
    {
        foreach (var block in GameObject.FindGameObjectsWithTag("Block"))
        {
            Instantiate(particles, block.transform.position, Quaternion.identity);
            Destroy(block);
            sound2.Play();
        }
    }




    IEnumerator SpawnBlocks()
    {
        blockYPos = Random.Range(-0.13f, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        blockSpeed = rnd.Next(15, 31) * .1f; //Creates a random float between 1.5 and 3.0;

        float randomXDim = rnd.Next(27, 37) * .01f;
        float randomYDim = rnd.Next(10, 30) * .01f;

        float spawnSpeed = Random.Range(minSpawnSpeed, maxSpawnSpeed); //A randomized spawn speed within a small range, to things less predictable. The lower the spawn speed, the shorter the time between blocks spawning.

        if (gameRunning){
            spawnNext = false;

            yield return new WaitForSeconds(spawnSpeed);

            Rigidbody2D clone = Instantiate(square, new Vector3(3.0f, blockYPos, 0), Quaternion.identity);
            clone.linearVelocity = transform.TransformDirection(Vector3.left * blockSpeed);
            clone.transform.localScale = new Vector3(randomXDim, randomYDim);

            spawnNext = true;
        }
    }

    IEnumerator SpawnPowerUps()
    {
        powerUpYPos = Random.Range(-0.13f, 1.54f);  //The positions the player can reach.
        blockSpeed = rnd.Next(15, 31) * .1f; //Creates a random float between 1.5 and 3.0;

        Rigidbody2D powerUp;
        if(blockSpeed > 1.9f) //A simple way to somewhat randomly choose which powerup will spawn. I've set it so that the hammer powerup will be more rare.
        {
            powerUp = powerUp1;
        } else
        {
            powerUp = powerUp2;
        }

        float poSpawnSpeed = Random.Range(minPoSpawnSpeed, maxPoSpawnSpeed);

        if (gameRunning)
        {
            spawnNextPowerUp = false;

            yield return new WaitForSeconds(poSpawnSpeed);

            Rigidbody2D clonePO = Instantiate(powerUp, new Vector3(3.0f, powerUpYPos, 0), Quaternion.identity);
            clonePO.linearVelocity = transform.TransformDirection(Vector3.left * blockSpeed);

            spawnNextPowerUp = true;
        }
    }

    IEnumerator SlowDownBlocks()
    {
        //Keep in mind that the blocks are moving to the left, so slowing them down means increasing their x-velocity.

        float slowDownDuration = 3.0f;

        while (slowDownDuration >= 0.0f)
        {
            slowDownDuration -= Time.deltaTime;
            foreach(var block in GameObject.FindGameObjectsWithTag("Block")){
                if (block.tag != "AlreadySlowedDown") //Only go through the blocks once, so their velocity isn't exponentionally halved. This also makes sure that blocks that spawn in the meantime will also be slowed down, since this loop keeps going until the slowdown time is over.
                {
                    block.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(block.GetComponent<Rigidbody2D>().linearVelocity.x / 2, 0f); //Simply halve each block's particular velocity, then later double it again. This way none of the blocks will completely come to a halt, and they will each slow down relative to their own speed as well.
                    block.tag = "AlreadySlowedDown";
                }
            }
            yield return null;
        }
        //After the slow-down time has passed, make the blocks return to their original velocity:
        foreach (var block in GameObject.FindGameObjectsWithTag("AlreadySlowedDown"))
        {
            block.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(block.GetComponent<Rigidbody2D>().linearVelocity.x * 2, 0f);
            block.tag = "Block"; //Set the tag back to Block in case the player picks up two hourglasses in quick succession where previously slowed down blocks are still on screen, so that they will slow down again.
        }
    }

    IEnumerator ScoreCounter()
    {
        while (gameRunning)
        {
            scoreAmount++;
            score.text = "S: " + scoreAmount.ToString();

            if(scoreAmount > highScoreAmount)
            {
                highScoreAmount++;
                highScore.text = "HS: " + highScoreAmount.ToString();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }



    //void MayhemMode() //Call this in Update
    //{
    //    for (int i = 0; i < 20; i++)
    //    {
    //        Rigidbody2D clone = Instantiate(square, transform.position, transform.rotation);
    //        clone.transform.position = new Vector3(i * 0.1f, 0, 0);

    //        clone.velocity = transform.TransformDirection(Vector3.left * 10);
    //    }
    //}

    //Another mayhem thing:
    //  minSpawnSpeed /= 2;
    //  maxSpawnSpeed /= 2;
}
