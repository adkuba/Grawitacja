using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
    public Sprite[] infoSlides;
    private int iterator = 0;

    private GameObject highscoreTextGO;
    private GameObject infoButtonGO;
    private GameObject tapTextGO;
    private GameObject infoImageGO;
    private GameObject menuBGGO;
    private Image infoImage;
    private Text highscoreText;
    private Text infoButtonText;
    private Canvas canvas;
    private Gyroscope gyro;
    
    private RectTransform infoButtonRect;
    private RectTransform tapTextRect;
    private RectTransform highscoreTextRect;
    

    void Start()
    {
        highscoreTextGO = GameObject.FindGameObjectWithTag("Highscore");
        infoImageGO = GameObject.FindGameObjectWithTag("InfoImage");
        infoButtonGO = GameObject.FindGameObjectWithTag("InfoButton");
        tapTextGO = GameObject.FindGameObjectWithTag("TapText");
        menuBGGO = GameObject.FindGameObjectWithTag("MenuBG");

        canvas = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<Canvas>();
        highscoreText = highscoreTextGO.GetComponent<UnityEngine.UI.Text>();
        infoButtonText = infoButtonGO.GetComponentInChildren<UnityEngine.UI.Text>();
        infoImage = infoImageGO.GetComponent<Image>();
        Button infoButton = infoButtonGO.GetComponent<UnityEngine.UI.Button>();
        infoButtonRect = infoButtonGO.GetComponent<RectTransform>();
        tapTextRect = tapTextGO.GetComponent<RectTransform>();
        highscoreTextRect = highscoreTextGO.GetComponent<RectTransform>();

        infoImage.sprite = infoSlides[iterator];
        infoImageGO.SetActive(false);
        infoButton.onClick.AddListener(TaskOnInfoClick);
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);

        //jesli nie przekroczylismy hs to wyjebane
        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 0)
        {
            highscore = PlayerPrefs.GetInt("highscore", 0);
            highscoreText.text = highscore.ToString();
        }
        //jesli przekroczylismy
        else
        {
            hsCounter = PlayerPrefs.GetInt("lastHighscore", 0);
            highscore = PlayerPrefs.GetInt("lastHighscore", 0);
            highscoreText.text = highscore.ToString();
        }

        //gyroscope
        //NASZ SAMSUNG NIE MA GYRO
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }

        animWait = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //to nie dziala
        //Quaternion q = Input.gyro.attitude;
        float xRotation = 0;
        float yRotation = 0;

        //infoButtonText.text = xRotation.ToString();

        float BGposx = (xRotation / 180) * 10;
        float BGposy = (yRotation / 180) * 10;

        menuBGGO.transform.position = new Vector3(BGposx + canvas.transform.position.x, BGposy + canvas.transform.position.y, 0);

        //zakres -8 8
        float Iposx = (xRotation / 180) * 16;
        float Iposy = (yRotation / 180) * 16;

        float Hposx = (xRotation / 180) * 16;
        float Hposy = (yRotation / 180) * 16;

        float Tposx = (xRotation / 180) * 16;
        float Tposy = (yRotation / 180) * 16;

        infoButtonRect.anchoredPosition = new Vector3(Iposx - 70, Iposy - 50, 0);
        highscoreTextRect.anchoredPosition = new Vector3(Hposx + 70, Hposy - 50, 0);
        tapTextRect.anchoredPosition = new Vector3(Tposx, Tposy + 100, 0);

        //jesli przekroczylismy hs to animacja
        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 1)
        {
            //+ init wait
            if (!textUp && Time.time - animWait > 1)
            {
                if (highscoreText.fontSize < 45)
                {
                    highscoreText.fontSize += 1;

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
                    highscoreText.text = highscore.ToString();
                    
                } else
                {
                    animWait = Time.time;
                    count = true;
                }
            }

            if (!textDown && count && textUp && Time.time - animWait > 1)
            {
                if (highscoreText.fontSize > 35)
                {
                    highscoreText.fontSize -= 1;

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

        //jak nacisniemy spacje to otwiera sie gra
        //Input.GetKey("space")
        //jesli nie jest otwarte menu
        if (!infoIsOpen)
        {
            //jesli klikniecie
            if (Input.touchCount > 0)
            {
                //to sprawdzamy czy nie na info
                Touch touch = Input.GetTouch(0);
                Vector2 pos = touch.position;
                //x sie zgadza
                if (pos.x < infoButtonGO.transform.position.x - (infoButtonRect.rect.width / 2) || pos.x > infoButtonGO.transform.position.x + (infoButtonRect.rect.width / 2))
                {
                    //y sie zgadza
                    if (pos.y < infoButtonGO.transform.position.y - (infoButtonRect.rect.height / 2) || pos.y > infoButtonGO.transform.position.y + (infoButtonRect.rect.height / 2))
                    {
                        //resetuje addScore
                        PlayerPrefs.SetInt("addScore", 0);
                        SceneManager.LoadScene("Game");
                    }
                }
            }
        }
        //jak jest otwarte menu to mozemy zmieniac info grafike
        else
        {
            //DOPRACOWAC BO COS NADAL NIE JEST OK
            float infIWidth = infoImage.sprite.rect.width;
            float infIHeight = infoImage.sprite.rect.height;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 pos = touch.position;
                //y klika w przedziale obrazka
                if (pos.y < (Screen.height / 2) + (infIHeight / 2) && pos.y > (Screen.height / 2) - (infIHeight / 2)) 
                {
                    //x klikamy na lewa polowke
                    if (pos.x < Screen.width / 2 && pos.x > (Screen.width / 2) - (infIWidth / 2)) 
                    {
                        iterator--;
                        if (iterator == -1)
                        {
                            iterator = 0;
                        }
                        infoImage.sprite = infoSlides[iterator];
                    }
                    //x klikamy na prawa polowke
                    if (pos.x > Screen.width / 2 && pos.x < (Screen.width / 2) + (infIWidth / 2)) 
                    {
                        iterator++;
                        if (iterator == 2)
                        {
                            iterator = 1;
                        }
                        infoImage.sprite = infoSlides[iterator];
                    }
                }

            }     
        }
    }

    void TaskOnInfoClick()
    {
        //zamykamy menu
        if (infoIsOpen)
        {
            highscoreTextGO.SetActive(true);
            tapTextGO.SetActive(true);
            infoImageGO.SetActive(false);
            infoButtonText.text = "Info";
            infoIsOpen = false;

        }
        //otwieramy menu
        else
        {
            highscoreTextGO.SetActive(false);
            tapTextGO.SetActive(false);
            infoImageGO.SetActive(true);
            infoButtonText.text = "Back";
            infoIsOpen = true;
        }
    }
}


