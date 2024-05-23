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

    public GameObject endScreen;

    [Header("Player")]
    public GameObject[] players;
    public int currentPlayersDead;

    public void GetCurrentPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void PlayerDied()
    {
        currentPlayersDead++;
        if (currentPlayersDead == players.Length)
        {
            PlayerPrefs.SetInt("ScoreP1", players[0].GetComponent<Player>().points);
            PlayerPrefs.SetInt("ScoreP2", players[1].GetComponent<Player>().points);
            PlayerPrefs.SetInt("Multiplayer", 1);
            endScreen.SetActive(true);
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

        foreach (var player in players)
        {
            player.GetComponent<Player>().freeze = false;
            if (player.GetComponent<Player>().currentHealth == 0)
            {
                currentPlayersDead--;
                player.GetComponent<Collider2D>().enabled = true;
                player.GetComponent<Player>().freeze = false;
                player.GetComponent<Player>().currentHealth++;
                player.GetComponent<Player>().ui.Hearts[0].SetTrigger("Gain Health");
            }
        }
        var spawnPosition = currentLevel.GetComponent<LevelManager>().spawnDoor.transform.position;
        foreach (var player in players)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Player>().freeze = true;
            player.transform.position = spawnPosition;
        }
        var projectiles = GameObject.FindGameObjectsWithTag("Proj");
        for (int i = 0; i < projectiles.Length; i++)
        {
            Destroy(projectiles[i]);
        }
        var pickups = GameObject.FindGameObjectsWithTag("Pickup");
        for (int i = 0; i < pickups.Length; i++)
        {
            Destroy(pickups[i]);
        }
        foreach (var player in players)
        {
            player.GetComponent<Player>().invinsable = true;
        }
        yield return new WaitForSeconds(0.5f);

        Fade.SetActive(false);
        changingLevel = false;
        currentLevel.GetComponent<LevelManager>().spawnDoor.GetComponent<Animator>().SetTrigger("Open");
        currentLevel.GetComponent<LevelManager>().spawnDoor.GetComponent<Animator>().SetTrigger("Close");
        foreach (var player in players)
        {
            player.GetComponent<Player>().GiveIFrames();
            player.GetComponent<Player>().freeze = false;
        }
    }
}