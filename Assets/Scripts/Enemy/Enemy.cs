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

    [Header("Patrolling Enemy")]
    public Rigidbody2D rb;
    public bool movingRight;
    public float moveSpeed;
    public GameObject directionCheck;
    public float changeCheckDistance;
    public float playerCheckingDistance;
    public float playerSeenMultiplyer;


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

        if (flying)
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;
        }
    }

    public void Update()
    {
        if (flying)
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
        else
        {
            RaycastHit2D hitPlayer;
            if (movingRight)
                hitPlayer = Physics2D.Raycast(directionCheck.transform.position, directionCheck.transform.right, playerCheckingDistance);
            else
                hitPlayer = Physics2D.Raycast(directionCheck.transform.position, -directionCheck.transform.right, playerCheckingDistance);

            if (hitPlayer.collider.CompareTag("Player"))
            {
                if (movingRight)
                    rb.velocity = new Vector2(moveSpeed * playerSeenMultiplyer, rb.velocity.y);
                else
                    rb.velocity = new Vector2(-moveSpeed * playerSeenMultiplyer, rb.velocity.y);
            }
            else
            {
                if (IsAtEdge(movingRight))
                    movingRight = !movingRight;
                if (movingRight)
                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }


            if (rb.velocity.x < 0)
                transform.localScale = new Vector3(-_scale, transform.localScale.y, transform.localScale.z);
            else if (rb.velocity.x > 0)
                transform.localScale = new Vector3(_scale, transform.localScale.y, transform.localScale.z);
        }


    }

    bool IsAtEdge(bool movingtotheright)
    {
        RaycastHit2D hitDown = Physics2D.Raycast(directionCheck.transform.position, -directionCheck.transform.up, changeCheckDistance);

        RaycastHit2D hitRight;
        if (movingtotheright) 
            hitRight = Physics2D.Raycast(directionCheck.transform.position, directionCheck.transform.right, changeCheckDistance);
        else
            hitRight = Physics2D.Raycast(directionCheck.transform.position, -directionCheck.transform.right, changeCheckDistance);

        if (!hitDown.collider || hitRight.collider)
            return true;
        return false;
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