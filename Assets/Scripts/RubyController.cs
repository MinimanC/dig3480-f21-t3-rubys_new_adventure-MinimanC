using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public int health { get {return currentHealth; }}
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public GameObject projectilePrefab;

    public static int level = 1;
    bool oneComplete = false;

    public Text score;
    private int scoreValue = 0;
    public Text winText;
    bool gameOver = false;
    public Text ammo;
    private int ammoValue = 4;
    private int sideQuest = 0;

    AudioSource audioSource;
    public AudioClip cogAudio;
    public AudioClip damage;
    public AudioClip loseMusic;
    public AudioClip winMusic;
    public AudioClip scoreAudio;

    public ParticleSystem healEffect;
    public ParticleSystem damageEffect;

    public GameObject backgroundAudio;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        score.text = "Score: " + scoreValue.ToString();
        winText.text = "";
        ammo.text = "Cogs: " + ammoValue.ToString();

        gameOver = false;
        oneComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (ammoValue > 0)
            {
                Launch();

                audioSource.PlayOneShot(cogAudio);

                ammoValue = ammoValue - 1;
                ammo.text = "Cogs: " + ammoValue.ToString();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if (character.tag == "Jambi")
                    {
                        if (oneComplete == true)
                        {
                            level = level + 1;
                            SceneManager.LoadScene("Scene2");
                        }
                        else
                        {
                            character.DisplayDialog();
                        }
                    }

                    if (character.tag == "Cat")
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount <0)
        {
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");

            audioSource.PlayOneShot(damage);
            damageEffect.Play();
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth <= 0)
        {
            winText.text = "You Lost! Press R to restart.";
            gameOver = true;
            rigidbody2d.simulated = false;

            AudioSource background = backgroundAudio.GetComponent<AudioSource>();

            background.clip = loseMusic;
            background.Play();
        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void SetCount()
    {
        scoreValue = scoreValue + 1;
        score.text = "Score: " + scoreValue.ToString();
        audioSource.PlayOneShot(scoreAudio);

        if (scoreValue >= 4)
        {
            if (level == 2 && sideQuest >= 1)
            {
                winText.text = "You Win! Came created by Casey Temple";
                gameOver = true;
                AudioSource background = backgroundAudio.GetComponent<AudioSource>();
            
                background.clip = winMusic;
                background.Play();
            }
            else
            {
                oneComplete = true;
            }
        }
    }

    public void SideQuest()
    {
        sideQuest = sideQuest + 1;

        if (scoreValue >= 4)
        {
            if (level == 2 && sideQuest >= 1)
            {
                winText.text = "You Win! Came created by Casey Temple";
                gameOver = true;
                AudioSource background = backgroundAudio.GetComponent<AudioSource>();
            
                background.clip = winMusic;
                background.Play();
            }
            else
            {
                oneComplete = true;
            }
        }
    }

    public void ChangeAmmo(int amount)
    {
        ammoValue = ammoValue + amount;
        ammo.text = "Cogs: " + ammoValue.ToString();
    }
}
