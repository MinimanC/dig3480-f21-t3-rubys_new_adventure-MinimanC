using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToughRobotScript : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    public int health = 2;

    AudioSource audioSource;
    public AudioClip damageAudio;
    
    Animator animator;

    bool broken = true;

    public ParticleSystem smokeEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!broken)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        GameObject ruby = GameObject.FindWithTag("Player");
        RubyController rubyController = ruby.GetComponent<RubyController>();
        
        health = health - 1;
        audioSource.PlayOneShot(damageAudio);
        
        if (health <= 0)
        {
            broken = false;
            rigidbody2D.simulated = false;
            smokeEffect.Stop();

            rubyController.SetCount();
        }
    }
}
