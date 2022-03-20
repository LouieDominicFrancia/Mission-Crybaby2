using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    // Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    private bool canMove = true;

    // Finite state machine
    private enum State {idle, running, jumping, falling, hurt}
    private State state = State.idle;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float hurtforce = 5f;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource gem;
    [SerializeField] private AudioSource gemEnd;
    [SerializeField] private AudioSource jump;

    

  private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

        canMove = true;
        

    }
    
    // enables player to move/ screen update per frame
    private void Update()
    {
        PermanentUI.perm.HealthAmount.text = PermanentUI.perm.health.ToString();

        if (canMove && state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); // Sets animation based on enumeration state

    }

    // Game collectibles interactions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            cherry.Play();
            Destroy(collision.gameObject);
            PermanentUI.perm.cherries += 1;
            PermanentUI.perm.cherryText.text = PermanentUI.perm.cherries.ToString();
        }
        if(collision.tag == "Powerup")
        {
            gem.Play();
            Destroy(collision.gameObject);
            jumpForce = 12f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }
    }


    // Enemy interactions
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                HandlingHealth();

                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to my right therefore I should be damaged and move left
                    rb.velocity = new Vector2(-hurtforce, rb.velocity.y);
                }
                else
                {
                    //Enemy is to my right therefore I should be damaged and move right
                    rb.velocity = new Vector2(hurtforce, rb.velocity.y);
                }
            }
            
        }
    }

    private void HandlingHealth()
    {
        PermanentUI.perm.health -= 1;
        PermanentUI.perm.HealthAmount.text = PermanentUI.perm.health.ToString();
        
        if (PermanentUI.perm.health <= 0)
        {
            SceneManager.LoadScene("TheEnd");
        }
    }


    // Player movement
    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");
        float vDirection = Input.GetAxis("Vertical");

        // moving left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        // moving right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }

        // jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();

        }
        
        if (vDirection < 0 || vDirection > 0 && state == State.jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, -5f);
        }
    }

    private void Jump()
    {
        jump.Play();
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    // Animation states updator
    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }

        else if ( state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if (state == State.hurt)
        {
            PlayerController moveScript = coll.GetComponent<PlayerController>();
            moveScript.canMove = false;
            if (Mathf.Abs(rb.velocity.x) < .0001f)
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 1f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
            PlayerController moveScript = coll.GetComponent<PlayerController>();
            moveScript.canMove = true;
        }

    }

    // Sound effect(player)
    private void Footstep()
    {
        footstep.Play();
    }

    // Powerup setting design
    private IEnumerator ResetPower()
    {
        yield return new WaitForSeconds(10);
        jumpForce = 8;
        GetComponent<SpriteRenderer>().color = Color.white;
        gemEnd.Play();
    }
}
