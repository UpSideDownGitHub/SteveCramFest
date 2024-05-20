using UnityEngine;

public class BaseSpell : Spell 
{
    public override void Initilise()
    {

    }

    public override void UpdateFrame(int fireDirection)
    {
        if (Time.time > base._timeOfNextFire)
        {
            base._timeOfNextFire = Time.time + base.fireRate;
            var tempBullet = Instantiate(base.projectile, base.firePoints[fireDirection].transform.position, base.firePoints[fireDirection].transform.rotation);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * base.fireForce);
        }
    }
}