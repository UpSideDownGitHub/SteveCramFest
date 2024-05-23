using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public TMP_Text scoreText;
    public Animator[] Hearts;

    public Animator gemHolder;
    private int currentPowerup;

    public Image[] powerups;
    public Image[] powerupsGlow;
    public Color[] powerupColors;

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetHearts(int health)
    {
        if (health == 5)
            return;
        Hearts[health].SetTrigger("Lose Health");
        //for (int i = 0; i < Hearts.Length; i++)
        //{
        //    if (health > i)
        //        Hearts[i].SetTrigger("Gain Health");
        //    else
        //        Hearts[i].SetTrigger("Lose Health");
        //}
    }

    public void SetPowerups(BulletType[] types)
    {
        for (int i = 0; i < powerups.Length; i++)
        {
            if (types.Length > i)
            {
                powerups[i].gameObject.SetActive(true);

                powerups[i].color = powerupColors[(int)types[i]];
                powerupsGlow[i].color = powerupColors[(int)types[i]];
            }
            else
            {
                powerups[i].gameObject.SetActive(false);

                powerups[i].color = powerupColors[0];
                powerupsGlow[i].color = powerupColors[0];
            }
        }
    }

    public IEnumerator RunAfter(BulletType[] types)
    {
        yield return new WaitForSeconds(0.25f);
        SetPowerups(types);
    }

    public void SetActiveGem(BulletType[] types)
    {
        gemHolder.SetTrigger("Spin");
        powerups[0].gameObject.SetActive(false);
        StartCoroutine(RunAfter(types));

        //powerups[currentPowerup].gameObject.SetActive(false);
        //currentPowerup++;

        //if (currentPowerup > 2)
        //    currentPowerup = 0;

        //powerupsGlow[currentPowerup].gameObject.SetActive(true);
    }
}