using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ManuCanvas : MonoBehaviour
{
    private int highscore = 0;
    public GameObject highscoreText;
    public GameObject infoImage;
    private float hsCounter = 0;

    private bool textUp = false;
    private bool count = false;
    private bool textDown = false;
    private float animWait = 0;

    void Start()
    {
        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 0) //jesli nie przekroczylismy hs to wyjebane
        {
            highscore = PlayerPrefs.GetInt("highscore", 0);
            highscoreText.GetComponent<UnityEngine.UI.Text>().text = highscore.ToString();
        }

        animWait = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 1) //jesli przekroczylismy hs to animacja
        {
            if (!textUp && Time.time - animWait > 1) //+ init wait
            {
                if (highscoreText.GetComponent<UnityEngine.UI.Text>().fontSize < 23)
                {
                    highscoreText.GetComponent<UnityEngine.UI.Text>().fontSize += 1;

                } else
                {
                    animWait = Time.time;
                    textUp = true;
                }
            }

            if (!count && textUp && Time.time - animWait > 1)
            {
                if (highscore < PlayerPrefs.GetInt("highscore", 0))
                {
                    hsCounter += 18 * Time.deltaTime;
                    highscore = Convert.ToInt32(hsCounter);
                    highscoreText.GetComponent<UnityEngine.UI.Text>().text = highscore.ToString();
                    
                } else
                {
                    animWait = Time.time;
                    count = true;
                }
            }

            if (!textDown && count && textUp && Time.time - animWait > 1)
            {
                if (highscoreText.GetComponent<UnityEngine.UI.Text>().fontSize > 16)
                {
                    highscoreText.GetComponent<UnityEngine.UI.Text>().fontSize -= 1;

                }
                else
                {
                    textDown = true;
                }
            }

            //koniec animacji
            if (textUp && count && textDown)
            {
                PlayerPrefs.SetInt("highscoreChanged", 0);
            }
        }

        if (Input.GetKey("space")) //jak nacisniemy spacje to otwiera sie gra
        {
            //resetuje addScore
            PlayerPrefs.SetInt("addScore", 0);
            //robie cos takiego bo mialem buga ze obrazek sie pojawial przy zmianie scene 
            Destroy(infoImage);
            SceneManager.LoadScene("Game");
        }
    }
}
