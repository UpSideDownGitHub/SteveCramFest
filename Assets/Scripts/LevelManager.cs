using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int totalEnemies;
    public int currentEnemies;

    public Door levelDoor;
    public GameObject spawnDoor;

    public void EnemyKilled()
    {
        currentEnemies--;
        if (currentEnemies == 0)
        {
            levelDoor.activated = true;
            levelDoor.GetComponent<Animator>().SetTrigger("Open");
        }
    }
}