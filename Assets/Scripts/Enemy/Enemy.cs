using JetBrains.Annotations;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy : MonoBehaviour
{
    public LevelManager levelManager;

    [Header("Health")]
    public float maxHealth;
    public float curHealth;

    [Header("Navigation")]
    public bool flying;
    public GameObject player;

    [Header("Flying Enemy")]
    public NavMeshAgent agent;
    public Vector2 noticeRange;
    private float _startTime;

    [Header("Shooting")]
    public bool shoots;
    public GameObject bullet;
    public GameObject firePoint;
    public float fireRate;
    public float fireForce;
    private float _timeOfNextFire;

    private float _scale;

    public void Start()
    {
        if (!levelManager)
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        _scale = transform.localScale.x;
        curHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        _startTime = Time.time + Random.Range(noticeRange.x, noticeRange.y);
        agent.updateUpAxis = false;
        agent.updateRotation = false;
    }

    public void Update()
    {
        if (Time.time > _startTime && player)
        {
            agent.SetDestination(player.transform.position);
        }

        if (Time.time > _timeOfNextFire && shoots)
        {
            var dir = player.transform.position - firePoint.transform.position;
            dir.Normalize();
            RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.transform.position, dir);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Ground") && hit.transform.gameObject.layer != 6)
                    break;
                if (hit.collider.CompareTag("Player"))
                {
                    _timeOfNextFire = Time.time + fireRate;
                    var bulletTemp = Instantiate(bullet, firePoint.transform.position, Quaternion.identity);
                    bulletTemp.GetComponent<Rigidbody2D>().AddForce(dir * fireForce);
                    bulletTemp.GetComponent<Projectile>().shooter = gameObject;
                }
            }
        }

        if (agent.velocity.x < 0)
            transform.localScale = new Vector3(-_scale, transform.localScale.y, transform.localScale.z);
        else if (agent.velocity.x > 0)
            transform.localScale = new Vector3(_scale, transform.localScale.y, transform.localScale.z);
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