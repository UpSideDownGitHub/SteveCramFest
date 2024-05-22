using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyTriggerDamage : MonoBehaviour
{
    [HideInInspector] public bool attacked;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !attacked)
        {
            attacked = true;
            collision.GetComponent<Player>().TakeDamage();
        }
    }
}