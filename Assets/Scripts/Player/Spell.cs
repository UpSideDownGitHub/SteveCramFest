using UnityEditor.Build.Content;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float fireRate;
    protected float _timeOfNextFire;

    public float fireForce;
    public GameObject[] firePoints;
    public GameObject projectile;


    public virtual void Initilise()
    {

    }

    public virtual void UpdateFrame(int fireDirection)
    {

    }
}