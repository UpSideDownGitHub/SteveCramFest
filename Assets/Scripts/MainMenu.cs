using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string mainScene;

    public void StartGame()
    {
        SceneManager.LoadScene(mainScene);
    }
}