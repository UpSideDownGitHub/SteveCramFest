using UnityEngine;

public class Door : MonoBehaviour
{
    public bool activated;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
            return;

        if (collision.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>().SpawnNewLevel();
        }
    }

}