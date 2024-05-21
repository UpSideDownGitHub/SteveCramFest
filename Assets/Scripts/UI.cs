using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public TMP_Text scoreText;
    public Image[] Hearts;
    public Image[] powerups;
    public Color[] powerupColors;

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetHearts(int health)
    {
        for (int i = 0; i < Hearts.Length; i++)
        {
            if (health > i)
                Hearts[i].color = Color.red;
            else
                Hearts[i].color = Color.black;
        }
    }

    public void SetPowerups(BulletType[] types)
    {
        for (int i = 0; i < powerups.Length; i++)
        {
            if (types.Length > i)
                powerups[i].color = powerupColors[(int)types[i]];
            else
                powerups[i].color = powerupColors[0];
        }
    }
}