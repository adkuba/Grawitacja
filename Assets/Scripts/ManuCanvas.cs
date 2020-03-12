using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManuCanvas : MonoBehaviour
{
    private int highscore;
    public GameObject highscoreText;
    public GameObject infoImage;

    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        highscoreText.GetComponent<UnityEngine.UI.Text>().text = highscore.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("space")) //jak nacisniemy spacje to otwiera sie gra
        {
            //robie cos takiego bo mialem buga ze obrazek sie pojawial przy zmianie scene 
            Destroy(infoImage);
            SceneManager.LoadScene("Game");
        }
    }
}
