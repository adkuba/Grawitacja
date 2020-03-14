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

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
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

        infoText.transform.position = new Vector3(Iposx + canvas.transform.position.x + 333, Iposy + canvas.transform.position.y + 175, 0);
        highscoreText.transform.position = new Vector3(Hposx + canvas.transform.position.x - 231, Hposy + canvas.transform.position.y + 175, 0);
        tapText.transform.position = new Vector3(Tposx + canvas.transform.position.x, Tposy + canvas.transform.position.y - 105, 0);
    }
}
