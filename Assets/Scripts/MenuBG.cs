using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBG : MonoBehaviour
{
    private Canvas canvas;
    private Vector2 canvasSize;
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
        //przetworzenie wartosci na zakres od -5 do 5 oraz -3 do 3
        //pozniej to powinno byc jakos zwiazane od aspect ratio
        float posx = (mousex / canvasSize.x) * 10 - 5;
        float posy = (mousey / canvasSize.y) * 6 - 3;
        transform.position = new Vector3(posx + canvas.transform.position.x, posy + canvas.transform.position.y, 0);
    }
}
