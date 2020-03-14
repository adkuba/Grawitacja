using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBG : MonoBehaviour
{
    private Canvas canvas;
    private Vector2 canvasSize;
    public GameObject infoText;
    public GameObject tapText;
    public GameObject highscoreText;
    private RectTransform infoTextRect;
    private RectTransform tapTextRect;
    private RectTransform highscoreTextRect;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);

        infoTextRect = infoText.GetComponent<RectTransform>();
        tapTextRect = tapText.GetComponent<RectTransform>();
        highscoreTextRect = highscoreText.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //przerobic to pozniej na wersje mobilna bez myszki!
        float mousex = Input.mousePosition.x;
        float mousey = Input.mousePosition.y;
        if (mousex < 0)
        {
            mousex = 0;
        }
        if (mousey < 0)
        {
            mousey = 0;
        }
        if (mousey > canvasSize.y)
        {
            mousey = canvasSize.y;
        }
        if (mousex > canvasSize.x)
        {
            mousex = canvasSize.x;
        }
        //przetworzenie wartosci na zakres od -5 do 5
        //pozniej to powinno byc jakos zwiazane od aspect ratio ??? chyba nie
        float BGposx = (mousex / canvasSize.x) * 10 - 5;
        float BGposy = (mousey / canvasSize.y) * 10 - 5;
        transform.position = new Vector3(BGposx + canvas.transform.position.x, BGposy + canvas.transform.position.y, 0);
        
        //zakres -8 8
        float Iposx = (mousex / canvasSize.x) * 16 - 8;
        float Iposy = (mousey / canvasSize.y) * 16 - 8;
        
        float Hposx = (mousex / canvasSize.x) * 16 - 8;
        float Hposy = (mousey / canvasSize.y) * 16 - 8;

        float Tposx = (mousex / canvasSize.x) * 16 - 8;
        float Tposy = (mousey / canvasSize.y) * 16 - 8;

        infoTextRect.anchoredPosition = new Vector3(Iposx - 70, Iposy - 50, 0);
        highscoreTextRect.anchoredPosition = new Vector3(Hposx + 70, Hposy - 50, 0);
        tapTextRect.anchoredPosition = new Vector3(Tposx, Tposy + 100, 0);
    }
}
