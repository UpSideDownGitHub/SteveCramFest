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

    public void StartPressed()
    {
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

    public void StartGameSingleplayer()
    {
        SceneManager.LoadScene(singleplayerScene);
    }
    public void StartGameMultiplater()
    {
        SceneManager.LoadScene(multiplayerScene);
    }
}