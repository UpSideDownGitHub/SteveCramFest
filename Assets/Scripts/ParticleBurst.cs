using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBurst : MonoBehaviour
{
    public ParticleSystem particles;
    public int amount;

    public void SpawnBurst()
    {
        particles.Emit(amount);
    }
}
