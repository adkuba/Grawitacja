using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using static System.Math;

public class MenuController : MonoBehaviour
{
    private int highscore;
    private bool infoIsOpen = false;
    private bool textUp = false;
    private bool count = false;
    private bool textDown = false;
    private float animWait = 0;
    private float hsCounter = 0;
    private Vector2 canvasSize;
    private Vector2 firstTouchPos;
    private Vector2 lastTouchPos;
    private float dragDistance;
    private int delta;
    private int easterEggCounter = 0;
    public Sprite easterEggImage;

    private GameObject highscoreTextGO;
    private GameObject infoButtonGO;
    private GameObject tapTextGO;
    private GameObject soundButtonGO;
    private Text highscoreText;
    private Text infoButtonText;
    private Canvas canvas;
    public Sprite soundOn;
    public Sprite soundOFF;
    private Image soundImage;
    private Button soundButton;
    private RectTransform soundButtonRect;
    private GameObject tutorialManager;
    private GameObject tutorialYesGO;
    private GameObject tutorialBGGO;
    private Image tutorialBG;
    

    void Start()
    {
        highscoreTextGO = GameObject.FindGameObjectWithTag("Highscore");
        tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager");
        tutorialYesGO = GameObject.FindGameObjectWithTag("TutorialYes");
        infoButtonGO = GameObject.FindGameObjectWithTag("InfoButton");
        tapTextGO = GameObject.FindGameObjectWithTag("TapText");
        soundButtonGO = GameObject.FindGameObjectWithTag("SoundB");
        tutorialBGGO = GameObject.FindGameObjectWithTag("TutorialBG");

        //swipe size
        dragDistance = (Screen.width + Screen.height) * 0.1f / 2f;
        canvas = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<Canvas>();
        highscoreText = highscoreTextGO.GetComponent<UnityEngine.UI.Text>();
        infoButtonText = infoButtonGO.GetComponentInChildren<UnityEngine.UI.Text>();
        Button infoButton = infoButtonGO.GetComponent<UnityEngine.UI.Button>();
        Button tutorialYesButton = tutorialYesGO.GetComponent<UnityEngine.UI.Button>();
        soundButton = soundButtonGO.GetComponent<UnityEngine.UI.Button>();
        soundImage = soundButton.GetComponent<UnityEngine.UI.Image>();
        tutorialBG = tutorialBGGO.GetComponent<UnityEngine.UI.Image>();
        soundButtonRect = soundButtonGO.GetComponent<RectTransform>();

        tutorialManager.SetActive(false);
        infoButton.onClick.AddListener(TaskOnInfoClick);
        tutorialYesButton.onClick.AddListener(YesTut);
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
        soundButton.onClick.AddListener(SoundButton);

        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 0)
        {
            highscore = PlayerPrefs.GetInt("highscore", 0);
            highscoreText.text = highscore.ToString();
        }
        //highscore passed
        else
        {
            hsCounter = PlayerPrefs.GetInt("lastHighscore", 0);
            highscore = PlayerPrefs.GetInt("lastHighscore", 0);
            highscoreText.text = highscore.ToString();
        }

        if (PlayerPrefs.GetInt("canPlayMusic", 1) == 1)
        {
            soundImage.sprite = soundOn;
        }
        else
        {
            soundImage.sprite = soundOFF;
        }
        animWait = Time.time;
        delta = PlayerPrefs.GetInt("highscore", 0) - highscore;

        //first open
        if (PlayerPrefs.GetInt("hasPlayed1_1", 0) == 0)
        {
            PlayerPrefs.SetInt("hasPlayed1_1", 1);
            TaskOnInfoClick();
        }
    }


    void Update()
    {
        //highscore animation
        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 1)
        {
            if (!textUp && Time.time - animWait > 1)
            {
                if (highscoreText.fontSize < 40)
                {
                    highscoreText.fontSize += 1;
                } 
                else
                {
                    animWait = Time.time;
                    textUp = true;
                }
            }

            if (!count && textUp && Time.time - animWait > 1)
            {
                if (highscore < PlayerPrefs.GetInt("highscore", 0))
                {
                    hsCounter += (delta * 0.24f + 15.6f) * Time.deltaTime;
                    if (hsCounter > PlayerPrefs.GetInt("highscore", 0))
                    {
                        hsCounter = PlayerPrefs.GetInt("highscore", 0);
                    }
                    highscore = Convert.ToInt32(hsCounter);
                    highscoreText.text = highscore.ToString();
                } 
                else
                {
                    animWait = Time.time;
                    count = true;
                }
            }

            if (!textDown && count && textUp && Time.time - animWait > 1)
            {
                if (highscoreText.fontSize > 30)
                {
                    highscoreText.fontSize -= 1;
                }
                else
                {
                    textDown = true;
                }
            }

            if (textUp && count && textDown)
            {
                PlayerPrefs.SetInt("highscoreChanged", 0);
            }
        }

        if (!infoIsOpen)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    firstTouchPos = touch.position;
                    lastTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    lastTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    lastTouchPos = touch.position;

                    //swipe detection
                    if (Math.Abs(lastTouchPos.x - firstTouchPos.x) < dragDistance && Math.Abs(lastTouchPos.y - firstTouchPos.y) < dragDistance)
                    {
                        //not clicking buttons
                        //touch is in pixels!!!
                        float percent = (canvasSize.y + soundButtonRect.anchoredPosition.y - 3 * (soundButtonRect.rect.height / 2) / 2) / canvasSize.y;
                        float pixelValue = Screen.height * percent;
                        if (lastTouchPos.y < pixelValue)
                        {
                            PlayerPrefs.SetInt("addScore", 0);
                            SceneManager.LoadScene("Game");
                        }
                    }
                }
            }
        }
    }

    void TaskOnInfoClick()
    {
        easterEggCounter++;
        if (easterEggCounter == 20)
        {
            tutorialBG.sprite = easterEggImage;
        }
        if (infoIsOpen)
        {
            highscoreTextGO.SetActive(true);
            tapTextGO.SetActive(true);
            soundButtonGO.SetActive(true);
            tutorialManager.SetActive(false);
            infoButtonText.text = "Tutorial";
            infoIsOpen = false;
        }
        else
        {
            soundButtonGO.SetActive(false);
            highscoreTextGO.SetActive(false);
            tapTextGO.SetActive(false);
            tutorialManager.SetActive(true);
            infoButtonText.text = "Back";
            infoIsOpen = true;
        }
    }


    void YesTut()
    {
        SceneManager.LoadScene("Tutorial");
    }


    void SoundButton()
    {
        if (PlayerPrefs.GetInt("canPlayMusic", 1) == 1)
        {
            PlayerPrefs.SetInt("canPlayMusic", 0);
            soundImage.sprite = soundOFF;
        }
        else
        {
            PlayerPrefs.SetInt("canPlayMusic", 1);
            soundImage.sprite = soundOn;
        }
    }
}