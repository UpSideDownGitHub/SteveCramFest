using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour
{
    [Header("Highscore Enter Screen")]
    public TMP_Text[] nameText = new TMP_Text[10];
    public TMP_Text[] scoreText = new TMP_Text[10];

    public string[] names = new string[10];
    public int[] scores = new int[10];

    public TMP_Text playerScoreText;

    public GameObject highscoreAchived;
    public GameObject yousuck;

    public GameObject youSuckButtonLikeReallyYouSuck;

    private int _currentScore;
    private string _currentName;
    private int _scoreID;
    public bool looser;

    public void Start()
    {
#if CLEARPLAYERPREFS
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetString("Name_" + i, "");
            PlayerPrefs.SetInt("Score_" + i, 0);
        }
#endif
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetInt("Multiplayer", 0) == 0)
            {
                names[i] = PlayerPrefs.GetString("Name_" + i, "");
                scores[i] = PlayerPrefs.GetInt("Score_" + i, 0);
            }
            else
            {
                names[i] = PlayerPrefs.GetString("Name1_" + i, "");
                scores[i] = PlayerPrefs.GetInt("Score1_" + i, 0);
            }
        }
        setText();
        if (PlayerPrefs.GetInt("Multiplayer", 0) == 0)
            gameFinished(PlayerPrefs.GetInt("ScoreP1"));
        else
            gameFinished(PlayerPrefs.GetInt("ScoreP1") + PlayerPrefs.GetInt("ScoreP2"));
    }

    public void gameFinished(int scoreAchieved)
    {
        _currentScore = scoreAchieved;
        playerScoreText.text = scoreAchieved.ToString(); 
        for (int i = 0; i < 10; i++)
        {
            if (scores[i] < scoreAchieved)
            {
                setScore(i, scoreAchieved);
                return;
            }
        }
        highscoreAchived.SetActive(false);
        yousuck.SetActive(true);
        EventSystem.current.SetSelectedGameObject(youSuckButtonLikeReallyYouSuck);
        looser = true;
    }

    public void setScore(int ID, int score)
    {
        ID++;
        // this does not work only workd fore bnumbers that are lower than the highscore
        int[] scoresArr = new int[scores.Length + 1];
        for (int i = 0; i < scores.Length + 1; i++)
        {
            if (i < ID - 1)
                scoresArr[i] = scores[i];
            else if (i == ID - 1)
            {

                scoresArr[i] = score;
            }
            else
                scoresArr[i] = scores[i - 1];
        }

        string[] namesArr = new string[names.Length + 1];
        for (int i = 0; i < names.Length + 1; i++)
        {
            if (i < ID - 1)
                namesArr[i] = names[i];
            else if (i == ID - 1)
                namesArr[i] = "";
            else
                namesArr[i] = names[i - 1];
        }

        for (int i = 0; i < 10; i++)
        {
            scores[i] = scoresArr[i];
            names[i] = namesArr[i];
        }

        //scores[ID] = score;
        //names[ID] = ""; // nothing as this is to be set by the player
        _currentName = "";
        _scoreID = --ID;
        setText();
    }

    public void setText()
    {
        for (int i = 0; i < 10; i++)
        {
            nameText[i].text = names[i];
            scoreText[i].text = scores[i].ToString();
        }

    }

    public void ButtonPressed(string button)
    {
        if (button.Equals("DEL"))
        {
            if (_currentName.Equals(""))
            {
                return;
            }
            // remove the last item of the string
            _currentName = _currentName.Remove(_currentName.Length - 1);
            nameText[_scoreID].text = _currentName;
            return;
        }
        if (button.Equals("OK"))
        {
            if (looser)
            {
                SceneManager.LoadSceneAsync("MainMenu");
            }
            // save the score and move to the next section
            if (_currentName.Equals(""))
            {
                return;
            }

            // set the new scores
            scores[_scoreID] = _currentScore;
            names[_scoreID] = _currentName;

            // save the scores
            for (int i = 0; i < 10; i++)
            {
                if (PlayerPrefs.GetInt("Multiplayer", 0) == 0)
                {
                    PlayerPrefs.SetString("Name_" + i, names[i]);
                    PlayerPrefs.SetInt("Score_" + i, scores[i]);
                }
                else
                {
                    PlayerPrefs.SetString("Name1_" + i, names[i]);
                    PlayerPrefs.SetInt("Score1_" + i, scores[i]);
                }
            }


            // ENABLE THE END SCREEN
            SceneManager.LoadSceneAsync("MainMenu");

            return;
        }
        if (_currentName.Length > 3)
            return;

        // if made it this far then we know a letter button has been pressed
        _currentName += button;
        nameText[_scoreID].text = _currentName;
    }
}
