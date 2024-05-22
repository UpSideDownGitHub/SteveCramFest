using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyTriggerDamage : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage();
            gameObject.SetActive(false);
        }
    }
}