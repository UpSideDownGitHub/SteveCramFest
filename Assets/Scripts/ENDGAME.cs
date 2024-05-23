using UnityEngine;
using UnityEngine.SceneManagement;

public class ENDGAME : MonoBehaviour
{
    public float uiTime;

    public void OnEnable()
    {
        Invoke("LoadResultsScreen", uiTime);
    }

    public void LoadResultsScreen()
    {
        SceneManager.LoadSceneAsync("Results");
    }
}