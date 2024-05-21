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

        var dir = player.transform.position - firePoint.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.transform.position, dir);
        Debug.DrawRay(firePoint.transform.position, dir, Color.red, 2f);
        print(hit.collider.tag);
        if (Time.time > _timeOfNextFire && hit.collider.CompareTag("Player"))
        {
            print("Shooting");
            _timeOfNextFire = Time.time + fireRate;
            var bulletTemp = Instantiate(bullet, firePoint.transform.position, Quaternion.Euler(dir));
            bulletTemp.GetComponent<Rigidbody2D>().AddForce(transform.right * fireForce);
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