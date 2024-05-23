using System.Collections;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackTime ;
    public float baseAttackDamage;
    public int bulletLayer;

    public float parryForce;

    public int splitCount = 100;

    public GameObject[] reboundTypes;
    public GameObject[] reboudEffects;

    public Player player;

    public void OnEnable()
    {
        Invoke("DiableAttack", attackTime);
    }

    public void DiableAttack()
    {
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(baseAttackDamage);
            player.AddPoints(100);
        }
        else if (collision.gameObject.layer == bulletLayer)
        {
            player.screenShake.TriggerShake();
            player.slashAnim.SetTrigger("Parry");

            // reflect the bullet
            var bulletRB = collision.GetComponent<Rigidbody2D>();
            var bulletProj = collision.GetComponent<Projectile>();
            
            if (bulletProj.shooter && !bulletProj.playerOwned)
            {
                var dir = bulletProj.shooter.transform.position - transform.position;
                SpawnBullet(dir, bulletProj.transform.position);
            }
            else
            {
                var dir = -bulletRB.velocity;
                SpawnBullet(dir, bulletProj.transform.position); 
            }
            player.AddPoints(50);
            Destroy(collision.gameObject);
        }
        DiableAttack();
    }

    public void SpawnBullet(Vector2 direction, Vector3 spawnPosition)
    {
        BulletType toSpawn;
        if (!player.parryList.TryDequeue(out toSpawn))
            toSpawn = BulletType.NORMALSHOT;
        print(toSpawn);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        var spawnEffect = Instantiate(reboudEffects[(int)toSpawn], spawnPosition, Quaternion.identity);
        spawnEffect.transform.rotation = Quaternion.Euler(0, 0, angle);

        switch (toSpawn)
        {
            case BulletType.NORMALSHOT:
                var playerBullet = Instantiate(reboundTypes[0], spawnPosition, Quaternion.identity);
                playerBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                playerBullet.GetComponent<Rigidbody2D>().AddForce(playerBullet.transform.right * parryForce);
                playerBullet.GetComponent<Projectile>().type = BulletType.NORMALSHOT;
                break;
            case BulletType.FIREBALL:
                var playerBullet2 = Instantiate(reboundTypes[1], spawnPosition, Quaternion.identity);
                playerBullet2.transform.rotation = Quaternion.Euler(0, 0, angle);
                playerBullet2.GetComponent<Rigidbody2D>().AddForce(playerBullet2.transform.right * parryForce);
                playerBullet2.GetComponent<Projectile>().type = BulletType.FIREBALL;
                break;
            case BulletType.SEEKER:
                var bullet = Instantiate(reboundTypes[2], spawnPosition, Quaternion.identity);
                bullet.GetComponent<Projectile>().type = BulletType.SEEKER;
                break;
            case BulletType.LIGHTNING:
                var bullet2 = Instantiate(reboundTypes[3], spawnPosition, Quaternion.identity);
                bullet2.GetComponent<Projectile>().type = BulletType.LIGHTNING;
                bullet2.transform.rotation = Quaternion.Euler(0, 0, angle);

                break;
            case BulletType.ICESPIKE:
                var playerBullet3 = Instantiate(reboundTypes[4], spawnPosition, Quaternion.identity);
                playerBullet3.transform.rotation = Quaternion.Euler(0, 0, angle);
                playerBullet3.GetComponent<Rigidbody2D>().AddForce(playerBullet3.transform.right * parryForce);
                playerBullet3.GetComponent<Projectile>().type = BulletType.ICESPIKE;
                break;
            case BulletType.SPLITSHOT:
                for (int i = 0; i < splitCount; i++)
                {
                    var playerBullet4 = Instantiate(reboundTypes[5], spawnPosition, Quaternion.identity);
                    var direction1 = new Vector2(direction.x + Random.Range(-10, 10), direction.y + Random.Range(-10, 10));
                    float angle2 = Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg;
                    playerBullet4.transform.rotation = Quaternion.Euler(0, 0, angle2);
                    playerBullet4.GetComponent<Rigidbody2D>().AddForce(playerBullet4.transform.right * parryForce);
                    playerBullet4.GetComponent<Projectile>().type = BulletType.SPLITSHOT;

                }
                break;
            case BulletType.BOUNCESHOT:
                var playerBullet7 = Instantiate(reboundTypes[6], spawnPosition, Quaternion.identity);
                playerBullet7.transform.rotation = Quaternion.Euler(0, 0, angle);
                playerBullet7.GetComponent<Rigidbody2D>().AddForce(playerBullet7.transform.right * parryForce * 3);
                playerBullet7.GetComponent<Projectile>().type = BulletType.BOUNCESHOT;
                break;
            case BulletType.DEATHBRINGER:
                var playerBullet8 = Instantiate(reboundTypes[7], spawnPosition, Quaternion.identity);
                playerBullet8.transform.rotation = Quaternion.Euler(0, 0, angle);
                playerBullet8.GetComponent<Rigidbody2D>().AddForce(playerBullet8.transform.right * parryForce);
                playerBullet8.GetComponent<Projectile>().type = BulletType.DEATHBRINGER;
                break;
        }
        player.ui.SetActiveGem(player.parryList.ToArray());
    }
}