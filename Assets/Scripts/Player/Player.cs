using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;

    [Header("Jumping")]
    public bool grounded;
    public float jumpForce;

    [Header("Movement")]
    public float movementSpeed;
    public float maxSpeed;

    [Header("Platformer")]
    public int platformLayer;
    public float passthroughTime;

    [Header("Spells (Attacking)")]
    public Spell[] spells;

    private float _movement;
    private Collider2D _currentPlatform;
    private float _scale;

    [SerializeField] private bool _downPressed;
    [SerializeField] private bool _upPressed;
    [SerializeField] private bool _shootPressed;


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
        {
            if (_downPressed)
            {
                StopCoroutine(DownPressed());
                StartCoroutine(DownPressed());
            }
            else
                JumpPressed();
        }
    }

    public void UpPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            _upPressed = true;
        else if (ctx.action.WasReleasedThisFrame())
            _upPressed = false;

    }

    public void DownPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            _downPressed = true;
        else if (ctx.action.WasReleasedThisFrame())
            _downPressed = false;
    }

    public void MovementPressed(InputAction.CallbackContext ctx)
    {
        _movement = ctx.ReadValue<float>();
    }

    public void Start()
    {
        _scale = transform.localScale.x;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = true;
        if (collision.gameObject.layer == platformLayer)
            _currentPlatform = collision.collider;
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = false;
    }



    public void Update()
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

        if (_shootPressed)
            ShootPressed();
    }

    public IEnumerator DownPressed()
    {
        if (_currentPlatform)
        {
            Physics2D.IgnoreCollision(col, _currentPlatform, true);
            yield return new WaitForSeconds(passthroughTime);
            Physics2D.IgnoreCollision(col, _currentPlatform, false);
        }
    }

    public void ShootPressed()
    {
        foreach (Spell spell in spells)
        {
            spell.UpdateFrame((_upPressed ? 0 : _downPressed ? 2 : 1) + (transform.localScale.x > 0 ? 0 : 3));
        }
    }

    public void JumpPressed()
    {
        if (grounded)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
