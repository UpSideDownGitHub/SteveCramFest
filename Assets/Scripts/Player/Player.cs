using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;
    public UI ui;

    [Header("Jumping")]
    public float floorCheckDistance = 0.1f;
    public float jumpForce;
    public int jumpCount;
    private int currentJumps;

    [Header("Movement")]
    public float movementSpeed;
    public float maxSpeed;

    [Header("Platformer")]
    public int platformLayer;
    public float passthroughTime;

    [Header("Attacking")]
    public GameObject sword;
    public float attackTime;
    public Queue<BulletType> parryList = new();
    private float _timeOfNextAttack;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Iframes")]
    public float iframeTime;
    private float _timeOfNoIFrame;
    public bool invinsable;

    [Header("Points")]
    public int points;

    [Header("Quitting")]
    public string mainScene;

    private float _movement;
    private Collider2D _currentPlatform;
    private float _scale;
    [SerializeField] private bool _downPressed;
    [SerializeField] private bool _shootPressed;

    public bool freeze;


    public void ShootPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            _shootPressed = true;
        else if (ctx.action.WasReleasedThisFrame())
            _shootPressed = false;
    }

    public void JumpPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            JumpPressed();
    }

    public void DownPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame() && _currentPlatform)
        {
            StopCoroutine("PassThrough");
            StartCoroutine(PassThrough(_currentPlatform));
        }
    }

    public void MovementPressed(InputAction.CallbackContext ctx)
    {
        _movement = ctx.ReadValue<float>();
    }

    public void StartButtonPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            SceneManager.LoadSceneAsync(mainScene);
        }
    }

    public bool Grounded
    {
        get
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, -transform.up, floorCheckDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    currentJumps = jumpCount;
                    return true; 
                }
            }
            return false;
        }
    }

    public void Start()
    {
        _scale = transform.localScale.x;
        currentHealth = maxHealth;

        var playerInput = GetComponent<PlayerInput>();
        ui = GameObject.FindGameObjectWithTag("UI" + playerInput.user.index.ToString()).GetComponent<UI>();
        ui.SetScore(points);
        ui.SetPowerups(parryList.ToArray());
        ui.SetHearts(currentHealth);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == platformLayer)
            _currentPlatform = collision.collider;
        if (collision.gameObject.layer == 7)
        {
            if (invinsable)
            {
                Destroy(collision.gameObject);
                return;
            }

            currentHealth  = currentHealth - 1 <= 0 ? 0 : currentHealth - 1;
            ui.SetHearts(currentHealth);
            invinsable = true;
            _timeOfNoIFrame = Time.time + iframeTime;
            if (currentHealth == 0)
            {
                // END GAME CODE GOES HERE
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            if (parryList.Count < 3)
            {
                parryList.Enqueue(collision.GetComponent<Pickup>().type);
                ui.SetPowerups(parryList.ToArray());
                Destroy(collision.gameObject);
            }
        }
    }

    public void AddPoints(int points)
    {
        this.points += points;
        ui.SetScore(this.points);
    }

    public void Update()
    {
        if (freeze)
            return;

        if (Time.time > _timeOfNoIFrame)
        {
            invinsable = false; 
        }

        if (_shootPressed)
            ShootPressed();
    }

    public void FixedUpdate()
    {
        if (freeze)
            return;
        if (_movement != 0)
        {
            if (_movement < 0)
                transform.localScale = new Vector3(-_scale, transform.localScale.y, transform.localScale.z);
            else if (_movement > 0)
                transform.localScale = new Vector3(_scale, transform.localScale.y, transform.localScale.z);


            rb.AddForce(transform.right * movementSpeed * _movement * Time.deltaTime, ForceMode2D.Force);
            rb.velocity = new Vector2(Mathf.Abs(rb.velocity.x) > maxSpeed ? rb.velocity.x > 0 ? maxSpeed : -maxSpeed : rb.velocity.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    public IEnumerator PassThrough(Collider2D platform)
    {
        Physics2D.IgnoreCollision(col, platform, true);
        yield return new WaitForSeconds(passthroughTime);
        Physics2D.IgnoreCollision(col, platform, false);
    }

    public void ShootPressed()
    {
        if (Time.time > _timeOfNextAttack)
        {
            _timeOfNextAttack = Time.time + attackTime;
            sword.SetActive(true);
        }
    }

    public void JumpPressed()
    {
        if (freeze)
            return;

        if (Grounded || currentJumps > 0)
        {
            currentJumps--;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse); 
        }
    }
}
