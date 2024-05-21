using UnityEngine;
using UnityEngine.AI;

public enum BulletType
{
    NORMALSHOT,
    FIREBALL,
    SEEKER,
    LIGHTNING,
    ICESPIKE,
    SPLITSHOT,
    BOUNCESHOT,
    DEATHBRINGER
}

public class Projectile : MonoBehaviour
{
    public bool playerOwned;
    public float damage;
    public BulletType type;
    public GameObject shooter;

    
    [Header("Seeker & Lightning")]
    public GameObject target;

    [Header("Seeker")]
    public NavMeshAgent agent;

    [Header("Lightning")]
    public int maxHits;
    public int curHits;
    public Rigidbody2D rb;
    public float movementSpeed;

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
            if (dist < closest)
            {
                closest = dist;
                closestEnemy = enemy;
            }
        }
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
            if (dist < closest)
            {
                closest = dist;
                closestEnemy = enemy;
            }
        }
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
                    curHits++;
                    if (curHits == maxHits)
                        Destroy(gameObject);
                    target = FindClosestEnemy(target);
                    var dir = target.transform.position - transform.position;
                    rb.velocity = Vector2.zero;
                    rb.AddForce(dir * movementSpeed);
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
                Destroy(gameObject);
            }
        }
    }
}