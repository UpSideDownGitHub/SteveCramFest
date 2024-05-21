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

    [Header("Jumping")]
    public float floorCheckDistance = 0.1f;
    public float jumpForce;

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

    public float closeTime;
    public float _timeofclose;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;

    private float _movement;
    private Collider2D _currentPlatform;
    private float _scale;
    [SerializeField] private bool _downPressed;
    [SerializeField] private bool _shootPressed;


    public void ShootPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            _shootPressed = true;
            _timeofclose = Time.time + closeTime;
        }
        else if (ctx.action.WasReleasedThisFrame())
        {
            _shootPressed = false;
            if (Time.time > _timeofclose)
            {
                Application.Quit();
                Debug.Log("QUITTING...");
            }
        }
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

    public bool Grounded
    {
        get
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, -transform.up, floorCheckDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Ground"))
                    return true;
            }
            return false;
        }
    }

    public void Start()
    {
        _scale = transform.localScale.x;
        currentHealth = maxHealth;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == platformLayer)
            _currentPlatform = collision.collider;
        if (collision.gameObject.layer == 7)
        {
            currentHealth  = currentHealth - 1 <= 0 ? 0 : currentHealth - 1;
            if (currentHealth == 0)
            {
                // END GAME CODE GOES HERE
                SceneManager.LoadScene("MainMenu");
            }
            Destroy(collision.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            if (parryList.Count < 3)
            {
                parryList.Enqueue(collision.GetComponent<Pickup>().type);
                Destroy(collision.gameObject);
            }
        }
    }

    public void Update()
    {
        if (_shootPressed)
            ShootPressed();
    }

    public void FixedUpdate()
    {
        if (movementSpeed != 0)
        {
            if (_movement < 0)
                transform.localScale = new Vector3(-_scale, transform.localScale.y, transform.localScale.z);
            else if (_movement > 0)
                transform.localScale = new Vector3(_scale, transform.localScale.y, transform.localScale.z);


            rb.AddForce(transform.right * movementSpeed * _movement * Time.deltaTime, ForceMode2D.Impulse);
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
        if (Grounded)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
