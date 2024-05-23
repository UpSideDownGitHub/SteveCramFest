using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BulletType
{
    NORMALSHOT, // normal shot
    FIREBALL, // large explosion
    SEEKER,
    LIGHTNING, // larger lightning
    ICESPIKE, // ice explosion // freeze enemy
    SPLITSHOT, // many many bullets
    BOUNCESHOT, 
    DEATHBRINGER
}

public class Projectile : MonoBehaviour
{
    public bool playerOwned;
    public float damage;
    public BulletType type;
    public GameObject shooter;

    [Header("Normal")]
    public GameObject normalEffect;

    [Header("Fire & Ice")]
    public GameObject fireEffect;
    public GameObject iceEffect;
    public float explosionRange;
    
    [Header("Seeker & Lightning")]
    public GameObject target;

    [Header("Seeker")]
    public NavMeshAgent agent;

    [Header("Lightning")]
    public int maxHits;
    public int curHits;
    public Rigidbody2D rb;
    public float movementSpeed;
    public List<string> visited;

    public void Start()
    {
        if (playerOwned)
        {
            switch (type)
            {
                case BulletType.SEEKER:
                    // find enemy targt and move towards it
                    agent.updateUpAxis = false;
                    agent.updateRotation = false;
                    target = FindClosestEnemy();
                    break;
                case BulletType.LIGHTNING:
                    // shoot to a enemy then more and more
                    target = FindClosestEnemy();
                    curHits = 0;
                    var dir = target.transform.position - transform.position;
                    rb.AddForce(dir * movementSpeed);
                    break;
            }
        }
    }
    public GameObject FindClosestEnemy(GameObject notThis)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closest = float.MaxValue;
        GameObject closestEnemy = null;
        foreach (var enemy in enemies)
        {
            if (enemy == notThis)
                continue;
            var dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closest && !visited.Contains(enemy.name))
            {
                closest = dist;
                closestEnemy = enemy;
            }
        }
        visited.Add(closestEnemy.name);
        return closestEnemy;
    }
    public GameObject FindClosestEnemy()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closest = float.MaxValue;
        GameObject closestEnemy = null;
        foreach (var enemy in enemies)
        {
            var dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closest && !visited.Contains(enemy.name))
            {
                closest = dist;
                closestEnemy = enemy;
            }
        }
        visited.Add(closestEnemy.name);
        return closestEnemy;
    }

    public void Update()
    {
        switch (type)
        {
            case BulletType.SEEKER:
                // find enemy targt and move towards it
                if (!target)
                    target = FindClosestEnemy();
                if (!target)
                    Destroy(gameObject);
                agent.SetDestination(target.transform.position);
                break;
            case BulletType.LIGHTNING:
                // shoot to a enemy then more and more
                if (Vector3.Distance(transform.position, target.transform.position) < 1f)
                {
                    target.GetComponent<Enemy>().TakeDamage(damage);
                    target = FindClosestEnemy(target);
                    if (target == null)
                        Destroy(gameObject);
                    var dir = target.transform.position - transform.position;
                    rb.velocity = Vector2.zero;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                    rb.AddForce(transform.right * movementSpeed);
                }
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerOwned)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);

                if (type == BulletType.FIREBALL)
                {
                    Instantiate(fireEffect, transform.position, Quaternion.identity);
                    var enemies = GetEnemyWithinRange(explosionRange);
                    foreach (var enemy in enemies)
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(damage);
                    }
                }
                else if (type == BulletType.ICESPIKE)
                {
                    Instantiate(iceEffect, transform.position, Quaternion.identity);
                    var enemies = GetEnemyWithinRange(explosionRange);
                    foreach (var enemy in enemies)
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(damage);
                    }
                }
                else if (type == BulletType.NORMALSHOT)
                {
                    Instantiate(normalEffect, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }

    public List<GameObject> GetEnemyWithinRange(float range)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> enemyList = new();
        foreach (var enemy in enemies)
        {
            if (Vector2.Distance(enemy.transform.position, transform.position) < range)
                enemyList.Add(enemy);
        }
        return enemyList;
    }
}