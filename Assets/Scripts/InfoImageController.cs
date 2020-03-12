using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoImageController : MonoBehaviour
{
    private Image image;
    public Sprite[] infoSlides;
    private int iterator;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        iterator = 0;
        image.sprite = infoSlides[iterator];
    }

    // Update is called once per frame
    void Update()
    {
        //zmienic pozniej na klikniecie na obrazek albo cos w tym stylu!!
        if (Input.GetKeyDown("up")) //zmieniamy grafike
        {
            iterator++;
            if (iterator == 2)
            {
                iterator = 0;
            }
            image.sprite = infoSlides[iterator];
        }
    }
}
