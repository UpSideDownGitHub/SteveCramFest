using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string singleplayerScene;
    public string multiplayerScene;

    public GameObject Main;
    public GameObject ModeSelection;
    public GameObject button1;

    public IEnumerator StartPressed()
    {
        Main.GetComponent<Animator>().SetTrigger("Play");

        yield return new WaitForSeconds(1);

        Main.SetActive(false);
        ModeSelection.SetActive(true);
        EventSystem.current.SetSelectedGameObject(button1);
    }
    public void StartButtonPressed()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        if (Main.activeInHierarchy)
        {
            Application.Quit();
        }
        Main.SetActive(true);
        ModeSelection.SetActive(false);
    }

    public IEnumerator StartGameSingleplayer()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(singleplayerScene);
    }
    public IEnumerator StartGameMultiplater()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(multiplayerScene);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}