using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;
    public bool grounded;
    public float jumpForce;

    [Header("Movement")]
    public float movementSpeed;
    public float maxSpeed;

    [Header("Platformer")]
    public int platformLayer;
    public float passthroughTime;

    private float _movement;
    private Collider2D _currentPlatform;


    public void ShootPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            ShootPressed();
    }

    public void JumpPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            JumpPressed();
    }

    public void DownPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            StopCoroutine(DownPressed());
            StartCoroutine(DownPressed());
        }

    }

    public void MovementPressed(InputAction.CallbackContext ctx)
    {
        _movement = ctx.ReadValue<float>();
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
            rb.AddForce(transform.right * movementSpeed * _movement * Time.deltaTime, ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Abs(rb.velocity.x) > maxSpeed ? rb.velocity.x > 0 ? maxSpeed : -maxSpeed : rb.velocity.x, rb.velocity.y);
        }
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
        Debug.Log("Shooting");
    }

    public void JumpPressed()
    {
        if (grounded)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
