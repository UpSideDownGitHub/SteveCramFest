using UnityEngine;

public class EnemyTriggerDamage : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        print("COLLIDING MAIN");
        if (collision.CompareTag("Player"))
        {
            print("COLLIDING");

            collision.GetComponent<Player>().TakeDamage();
            gameObject.SetActive(false);
        }
    }
}