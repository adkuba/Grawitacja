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
    private Vector3 lastDevAcc = Vector3.zero;

    private GameObject highscoreTextGO;
    private GameObject infoButtonGO;
    private GameObject tapTextGO;
    private GameObject infoImageGO;
    private GameObject menuBGGO;
    private Image infoImage;
    private Text highscoreText;
    private Text infoButtonText;
    private Canvas canvas;
    
    private RectTransform infoButtonRect;
    private RectTransform tapTextRect;
    private RectTransform highscoreTextRect;
    private RectTransform infoImageRect;
    

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
        infoImageRect = infoImageGO.GetComponent<RectTransform>();

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

        animWait = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deviceAcc = Input.acceleration;
        //max magnitude = 1
        deviceAcc.Normalize();

        //uzytkownik zazwyczaj trzyba telefon w pozycji 45 stopni wiec dodaje troche do y pozycji bo punkt "zero" jest jak telefon jest na plasko
        //0.5 to jest 45 stopni ja robie troche mniej
        float yOffset = 0.2f * 30;

        //UWAGA NA TE DODANE WARTOSCI, powinieniem je zgarniac w Start() i tutaj dodawac, żeby nie trzeba bylo zmieniac wartosci i w unity i w skrypcie
        //REFAKTORYZACJA
        Vector3 BGdesired = new Vector3(deviceAcc.x * 20 + canvas.transform.position.x, deviceAcc.y * 20 + canvas.transform.position.y + yOffset, 0);
        Vector2 IBDesired = new Vector2(deviceAcc.x * 30 - 100, deviceAcc.y * 30 - 40 + yOffset);
        Vector2 HSDesired = new Vector2(deviceAcc.x * 30 + 100, deviceAcc.y * 30 - 40 + yOffset);
        Vector2 TDesired = new Vector2(deviceAcc.x * 30, deviceAcc.y * 30 + 60 + yOffset);
        Vector2 IIDesired = new Vector2(deviceAcc.x * 30, deviceAcc.y * 30 + yOffset);

        //musze przesunac bg
        if (Vector3.Distance(menuBGGO.transform.position, BGdesired) > 0.1)
        {
            Vector3 difference = BGdesired - menuBGGO.transform.position;
            menuBGGO.transform.position += difference * Time.deltaTime * 20;
        }

        //infoButton
        if (Vector2.Distance(infoButtonRect.anchoredPosition, IBDesired) > 0.1)
        {
            Vector2 difference = IBDesired - infoButtonRect.anchoredPosition;
            infoButtonRect.anchoredPosition += difference * Time.deltaTime * 20;
        }

        //highscore
        if (Vector2.Distance(highscoreTextRect.anchoredPosition, HSDesired) > 0.1)
        {
            Vector2 difference = HSDesired - highscoreTextRect.anchoredPosition;
            highscoreTextRect.anchoredPosition += difference * Time.deltaTime * 20;
        }

        //tap
        if (Vector2.Distance(tapTextRect.anchoredPosition, TDesired) > 0.1)
        {
            Vector2 difference = TDesired - tapTextRect.anchoredPosition;
            tapTextRect.anchoredPosition += difference * Time.deltaTime * 20;
        }

        //infoImage
        if (Vector2.Distance(infoImageRect.anchoredPosition, IIDesired) > 0.1)
        {
            Vector2 difference = IIDesired - infoImageRect.anchoredPosition;
            infoImageRect.anchoredPosition += difference * Time.deltaTime * 20;
        }


        //jesli przekroczylismy hs to animacja
        if (PlayerPrefs.GetInt("highscoreChanged", 0) == 1)
        {
            //+ init wait
            if (!textUp && Time.time - animWait > 1)
            {
                if (highscoreText.fontSize < 40)
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
                if (highscoreText.fontSize > 30)
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


