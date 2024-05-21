using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackTime;
    public float baseAttack;
    public int bulletLayer;

    public void OnEnable()
    {
        Invoke("DiableAttack", attackTime);
    }

    public void DiableAttack()
    {
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(baseAttack);
        }
        else if (collision.gameObject.layer == bulletLayer)
        {
            // reflect the bullet
            var bulletRB = collision.GetComponent<Rigidbody2D>();
            bulletRB.velocity = new Vector2(-bulletRB.velocity.x, -bulletRB.velocity.y);
            var bulletProj = collision.GetComponent<Projectile>();
            bulletProj.playerOwned = true;
        }
    }
}