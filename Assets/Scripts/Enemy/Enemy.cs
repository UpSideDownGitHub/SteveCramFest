using JetBrains.Annotations;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class Enemy : MonoBehaviour
{
    public LevelManager levelManager;

    [Header("Health")]
    public float maxHealth;
    public float curHealth;
    public bool canTakeDamage;
    public float dissolveTime;
    public SpriteRenderer enemySprite;
    private int _dissolveAmmount = Shader.PropertyToID("_DissolveAmmount");

    [Header("Navigation")]
    public bool flying;
    public GameObject player;

    [Header("Animations")]
    public Animator enemyAnimator;
    public float previousSpeed;
    public float afterAttackDelay;

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

    [Header("Melee")]
    public GameObject weaponTrigger;
    public float meleeDistance;
    public bool inRange;

    [Header("Multiplayer")]
    public float playerSearchTime;
    private float _timeToSearchForNextPlayer;

    [Header("Powerups")]
    public GameObject[] powerups;

    public void Start()
    {
        levelManager = GetComponentInParent<LevelManager>();
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

    public GameObject GetClosestPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        var closest = float.MaxValue;
        GameObject closestPlayer = null;
        foreach (var player in players)
        {
            var dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    public void Update()
    {
        if (Time.time > _timeToSearchForNextPlayer)
        {
            _timeToSearchForNextPlayer = Time.time + playerSearchTime;
            player = GetClosestPlayer();
        }

        if (player == null)
            return;

        if (flying)
        {
            if (Time.time > _startTime)
            {
                agent.SetDestination(player.transform.position);
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
            
            if (!hitPlayer.collider)
                return;

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
        if (!shoots)
        {
            RaycastHit2D hitRight;
            if (movingRight)
                hitRight = Physics2D.Raycast(directionCheck.transform.position, directionCheck.transform.right, meleeDistance);
            else
                hitRight = Physics2D.Raycast(directionCheck.transform.position, -directionCheck.transform.right, meleeDistance);

            if (hitRight.collider)
            {
                if (hitRight.collider.CompareTag("Player"))
                {
                    inRange = true;
                    AttackAnimation();
                }
            }
            else
                inRange = false;
        }
        else
        {
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
        }
    }

    private void AttackAnimation()
    {
        enemyAnimator.SetTrigger("Attack");
        if (moveSpeed == 0)
            return;
        moveSpeed = 0;
        StartCoroutine(StartRun());
    }

    public IEnumerator StartRun()
    {
        yield return new WaitForSeconds(0.2f);
        weaponTrigger.gameObject.SetActive(true);
        yield return new WaitForSeconds(afterAttackDelay);
        moveSpeed = previousSpeed;
        weaponTrigger.gameObject.SetActive(false);
    }


    public IEnumerator Vanish()
    {
        GetComponent<Collider2D>().enabled = false;
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(0, 1f, (elapsedTime / dissolveTime));
            enemySprite.material.SetFloat(_dissolveAmmount, lerpedDissolve);
            yield return null;
        }
        Destroy(gameObject);
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
        if (!canTakeDamage)
            return;
        curHealth = curHealth - damage < 0 ? 0 : curHealth - damage;

        if (curHealth == 0)
        {
            if (levelManager == null)
                levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

            Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, Quaternion.identity);

            levelManager.EnemyKilled();
            StartCoroutine(Vanish());
        }
    }
}