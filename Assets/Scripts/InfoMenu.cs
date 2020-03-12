using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenu : MonoBehaviour
{
    public GameObject infoImage;
    public GameObject highscoreText;
    public GameObject tapToPlay;
    private bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        Button info = GetComponent<Button>();
        info.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (isOpen) //zamykamy menu
        {
            highscoreText.SetActive(true);
            tapToPlay.SetActive(true);
            infoImage.SetActive(false);
            GetComponentInChildren<Text>().text = "Info";
            isOpen = false;

        } else //otwieramy menu
        {
            highscoreText.SetActive(false);
            tapToPlay.SetActive(false);
            infoImage.SetActive(true);
            GetComponentInChildren<Text>().text = "Back";
            isOpen = true;
        }
    }
}
