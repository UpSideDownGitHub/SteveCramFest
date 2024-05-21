using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool playerOwned;
    public float damage;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerOwned)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
}