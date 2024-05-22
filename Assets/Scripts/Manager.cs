using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [Header("Level Manager")]
    public LevelList levels;
    public GameObject currentLevel;
    public GameObject Fade;
    public bool changingLevel;

    [Header("Player")]
    public GameObject[] players;
    public int currentPlayersDead;

    public void PlayerDied()
    {
        currentPlayersDead++;
        if (currentPlayersDead == players.Length)
        {
            PlayerPrefs.SetInt("ScoreP1", players[0].GetComponent<Player>().points);
            PlayerPrefs.SetInt("ScoreP2", players[1].GetComponent<Player>().points);
            PlayerPrefs.SetInt("Multiplayer", 1);
            SceneManager.LoadScene("Results");
        }
    }

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
            if (player.GetComponent<Player>().currentHealth == 0)
            {
                player.SetActive(true);
                player.GetComponent<Player>().currentHealth++;
            }
        }
        Fade.SetActive(false);
        changingLevel = false;
    }
}