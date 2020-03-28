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
    private GameObject infoBackButtonGO;
    private GameObject infoNextButtonGO;
    private GameObject soundButtonGO;
    private Image infoImage;
    private Text highscoreText;
    private Text infoButtonText;
    private Canvas canvas;
    public Sprite soundOn;
    public Sprite soundOFF;
    private Image soundImage;
    private Button soundButton;

    private RectTransform infoButtonRect;
    private RectTransform tapTextRect;
    private RectTransform highscoreTextRect;
    private RectTransform infoImageRect;
    private RectTransform soundButtonRect;
    

    void Start()
    {
        highscoreTextGO = GameObject.FindGameObjectWithTag("Highscore");
        infoImageGO = GameObject.FindGameObjectWithTag("InfoImage");
        infoButtonGO = GameObject.FindGameObjectWithTag("InfoButton");
        tapTextGO = GameObject.FindGameObjectWithTag("TapText");
        menuBGGO = GameObject.FindGameObjectWithTag("MenuBG");
        infoBackButtonGO = GameObject.FindGameObjectWithTag("IBack");
        infoNextButtonGO = GameObject.FindGameObjectWithTag("INext");
        soundButtonGO = GameObject.FindGameObjectWithTag("SoundB");

        canvas = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<Canvas>();
        highscoreText = highscoreTextGO.GetComponent<UnityEngine.UI.Text>();
        infoButtonText = infoButtonGO.GetComponentInChildren<UnityEngine.UI.Text>();
        infoImage = infoImageGO.GetComponent<Image>();
        Button infoButton = infoButtonGO.GetComponent<UnityEngine.UI.Button>();
        infoButtonRect = infoButtonGO.GetComponent<RectTransform>();
        Button infoBackButton = infoBackButtonGO.GetComponent<UnityEngine.UI.Button>();
        Button infoNextButton = infoNextButtonGO.GetComponent<UnityEngine.UI.Button>();
        tapTextRect = tapTextGO.GetComponent<RectTransform>();
        highscoreTextRect = highscoreTextGO.GetComponent<RectTransform>();
        infoImageRect = infoImageGO.GetComponent<RectTransform>();
        soundButtonRect = soundButtonGO.GetComponent<RectTransform>();
        soundButton = soundButtonGO.GetComponent<UnityEngine.UI.Button>();
        soundImage = soundButton.GetComponent<UnityEngine.UI.Image>();

        infoImage.sprite = infoSlides[iterator];
        infoImageGO.SetActive(false);
        infoButton.onClick.AddListener(TaskOnInfoClick);
        infoNextButton.onClick.AddListener(NextInfo);
        infoBackButton.onClick.AddListener(BackInfo);
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
        soundButton.onClick.AddListener(SoundButton);

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

        if (PlayerPrefs.GetInt("canPlayMusic", 1) == 1)
        {
            soundImage.sprite = soundOn;
        }
        else
        {
            soundImage.sprite = soundOFF;
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
        Vector2 SBDesired = new Vector2(deviceAcc.x * 30, deviceAcc.y * 30 + yOffset - 40);

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

        //soundButton
        if (Vector2.Distance(soundButtonRect.anchoredPosition, SBDesired) > 0.1)
        {
            Vector2 difference = SBDesired - soundButtonRect.anchoredPosition;
            soundButtonRect.anchoredPosition += difference * Time.deltaTime * 20;
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

        //jesli nie jest otwarte info
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
    }

    void TaskOnInfoClick()
    {
        //zamykamy menu
        if (infoIsOpen)
        {
            highscoreTextGO.SetActive(true);
            tapTextGO.SetActive(true);
            soundButtonGO.SetActive(true);
            infoImageGO.SetActive(false);
            infoButtonText.text = "Info";
            infoBackButtonGO.SetActive(false);
            infoNextButtonGO.SetActive(false);
            infoIsOpen = false;

        }
        //otwieramy menu
        else
        {
            soundButtonGO.SetActive(false);
            highscoreTextGO.SetActive(false);
            tapTextGO.SetActive(false);
            infoImageGO.SetActive(true);
            infoButtonText.text = "Back";
            //zaczynamy zawsze od pierwszego
            iterator = 0;
            infoBackButtonGO.SetActive(false);
            infoNextButtonGO.SetActive(true);
            infoIsOpen = true;
        }
    }

    void NextInfo()
    {
        iterator++;
        if (iterator == 1)
        {
            infoBackButtonGO.SetActive(true);
        }
        infoImage.sprite = infoSlides[iterator];
        if (iterator == infoSlides.Length - 1)
        {
            infoNextButtonGO.SetActive(false);
        }
    }

    void BackInfo()
    {
        iterator--;
        if (iterator == infoSlides.Length - 2)
        {
            infoNextButtonGO.SetActive(true);
        }
        infoImage.sprite = infoSlides[iterator];
        if (iterator == 0)
        {
            infoBackButtonGO.SetActive(false);
        }
    }

    void SoundButton()
    {
        //zmieniamy na nie moze grac
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


