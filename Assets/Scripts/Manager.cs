using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Level Manager")]
    public LevelList levels;
    public GameObject currentLevel;
    public GameObject Fade;
    public bool changingLevel;

    [Header("Player")]
    public GameObject[] players;

    public void SpawnNewLevel()
    {
        if (!changingLevel)
        {
            changingLevel = true;
            StartCoroutine(LevelChange());
        }
    }

    public IEnumerator LevelChange()
    {
        Fade.SetActive(true);

        Destroy(currentLevel);

        currentLevel = Instantiate(levels.levels[Random.Range(0, levels.levels.Length)]);

        var spawnPosition = currentLevel.GetComponent<LevelManager>().spawnDoor.transform.position;
        foreach (var player in players)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Player>().freeze = true;
            player.transform.position = spawnPosition;
        }
        yield return new WaitForSeconds(1f);
        foreach (var player in players)
        {
            player.GetComponent<Player>().freeze = false;
        }
        Fade.SetActive(false);
        changingLevel = false;
    }
}