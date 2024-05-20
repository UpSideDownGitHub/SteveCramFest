using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public LevelManager levelManager;

    public float maxHealth;
    public float curHealth;

    public void Start()
    {
        if (!levelManager)
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        curHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        curHealth = curHealth - damage < 0 ? 0 : curHealth - damage;

        if (curHealth == 0)
        {
            levelManager.EnemyKilled();
            Destroy(gameObject);
        }
    }
}