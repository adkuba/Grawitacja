using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

class PlanetsCompare : IComparer<GameObject>
{
    private GameObject player = GameObject.FindGameObjectWithTag("Player");

    //porownywarka po odleglosciach od playera
    public int Compare(GameObject planet1, GameObject planet2)
    {
        if (planet1 == null && planet2 == null)
        {
            return 0;
        }
        else if (planet1 == null)
        {
            // pierwsza planeta nie istnieje wiec druga jest blizej
            return 1;
        }
        else if (planet2 == null)
        {
            return -1;
        }
        //jesli nie mamy nullow to wyliczmy

        float distance1 = Vector3.Distance(player.transform.position, planet1.transform.position);
        float distance2 = Vector3.Distance(player.transform.position, planet2.transform.position);
        if (distance1 < distance2)
        {
            //pierwsza planeta jest blizej niz druga
            return -1;
        }
        else if (distance1 > distance2)
        {
            //pierwsza planeta jest dalej niz druga
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

public class Pointers : MonoBehaviour
{
    private GameObject[] planets;
    private GameObject firstPointer;
    private GameObject secondPointer;
    private GameObject player;
    private Vector2 canvasSize;
    private Vector2 screenBounds;
    private Image firstPointerImage;
    private Image secondPointerImage;
    private Canvas canvas;
    private RectTransform firstPRect;
    private RectTransform secondPRect;

    // Start is called before the first frame update
    void Start()
    {
        //moze byc get child bo w sumie nie ma znaczenia ktory to ktory plus mamy empty gameobject ktory jest managerem
        firstPointer = transform.GetChild(0).gameObject;
        secondPointer = transform.GetChild(1).gameObject;
        firstPointerImage = firstPointer.GetComponent<UnityEngine.UI.Image>();
        secondPointerImage = secondPointer.GetComponent<UnityEngine.UI.Image>();
        firstPRect = firstPointer.GetComponent<RectTransform>();
        secondPRect = secondPointer.GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<Canvas>();
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width / 2, canvas.GetComponent<RectTransform>().rect.height / 2);
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        PlanetsCompare planetsCompare = new PlanetsCompare();
        Array.Sort(planets, planetsCompare);

        //ustalanie pozycji pointerow, odrzucam te ktore sa blizej niz okrag o promieniu do najdalszego punktu kamery (róg ekranu)
        //tylko na poczatku planet moze byc mniej niz dwa wiec moge zrobic taki warunek
        if (planets.Length < 2)
        {
            firstPointer.SetActive(false);
            secondPointer.SetActive(false);

        }
        //jesli jest wiecej niz dwie planety
        else
        {
            int i = 0;
            int posIdx = 0;
            Vector2[] positions = new Vector2[2] { Vector2.zero, Vector2.zero };
            float[] alpha = new float[2] { 0, 0 };
            while (i < planets.Length)
            {
                //19 to max promien planety
                if (Vector3.Distance(player.transform.position, planets[i].transform.position) > screenBounds.magnitude + 19)
                {
                    float a = (player.transform.position.y - planets[i].transform.position.y) / (player.transform.position.x - planets[i].transform.position.x);
                    float b = player.transform.position.y - a * player.transform.position.x;
                    //mamy wzor prostej teraz czas na sprawdzenie
                    Rect cameraView = new Rect(player.transform.position.x - screenBounds.x, player.transform.position.y - screenBounds.y, screenBounds.x * 2, screenBounds.y * 2);
                    float xmin = player.transform.position.x - screenBounds.x;
                    float ymax = player.transform.position.y + screenBounds.y;
                    float xmax = player.transform.position.x + screenBounds.x;
                    float ymin = player.transform.position.y - screenBounds.y;
                    Vector2 left = new Vector2(xmin + 4.3f, a * (xmin + 4.3f) + b);
                    Vector2 up = new Vector2((ymax - 3.1f - b) / a, ymax - 3.1f);
                    Vector2 right = new Vector2(xmax - 4.3f, a * (xmax - 4.3f) + b);
                    Vector2 down = new Vector2((ymin + 3.1f - b) / a, ymin + 3.1f);

                    //jesli planeta nad playerem
                    if (planets[i].transform.position.y > player.transform.position.y)
                    {
                        //pierwsza cwiartka
                        if (planets[i].transform.position.x > player.transform.position.x)
                        {
                            if (cameraView.Contains(up))
                            {
                                Vector2 upToCanvas = new Vector2(( (up.x - player.transform.position.x) / screenBounds.x ) * canvasSize.x, ( (up.y - player.transform.position.y) / screenBounds.y ) * canvasSize.y);
                                positions[posIdx] = upToCanvas;

                            }
                            //jesli nie na gorze to jest po prawej
                            else
                            {
                                Vector2 rightToCanvas = new Vector2(( (right.x - player.transform.position.x) / screenBounds.x ) * canvasSize.x, ( (right.y - player.transform.position.y) / screenBounds.y ) * canvasSize.y);
                                positions[posIdx] = rightToCanvas;
                            }

                        }
                        //druga cwiartka
                        else
                        {
                            if (cameraView.Contains(up))
                            {
                                Vector2 upToCanvas = new Vector2(((up.x - player.transform.position.x) / screenBounds.x) * canvasSize.x, ((up.y - player.transform.position.y) / screenBounds.y) * canvasSize.y);
                                positions[posIdx] = upToCanvas;
                            }
                            //jesli nie na gorze to jest po lewej
                            else
                            {
                                Vector2 leftToCanvas = new Vector2(( (left.x - player.transform.position.x) / screenBounds.x ) * canvasSize.x, ( (left.y - player.transform.position.y) / screenBounds.y ) * canvasSize.y);
                                positions[posIdx] = leftToCanvas;
                            }
                        }

                    }
                    //jesli planeta ponizej
                    else if (planets[i].transform.position.y < player.transform.position.y)  
                    {
                        //czwarta cwiartka
                        if (planets[i].transform.position.x > player.transform.position.x) 
                        {
                            if (cameraView.Contains(down))
                            {
                                Vector2 downToCanvas = new Vector2(( (down.x - player.transform.position.x) / screenBounds.x ) * canvasSize.x, ( (down.y - player.transform.position.y) / screenBounds.y ) * canvasSize.y);
                                positions[posIdx] = downToCanvas;
                            }
                            //jesli nie na dole to jest po prawej
                            else
                            {
                                Vector2 rightToCanvas = new Vector2(((right.x - player.transform.position.x) / screenBounds.x) * canvasSize.x, ((right.y - player.transform.position.y) / screenBounds.y) * canvasSize.y);
                                positions[posIdx] = rightToCanvas;
                            }

                        }
                        //trzecia cwiartka
                        else
                        {
                            if (cameraView.Contains(down))
                            {
                                Vector2 downToCanvas = new Vector2(((down.x - player.transform.position.x) / screenBounds.x) * canvasSize.x, ((down.y - player.transform.position.y) / screenBounds.y) * canvasSize.y);
                                positions[posIdx] = downToCanvas;
                            }
                            //jesli nie na dole to jest po lewej
                            else
                            {
                                Vector2 leftToCanvas = new Vector2(((left.x - player.transform.position.x) / screenBounds.x) * canvasSize.x, ((left.y - player.transform.position.y) / screenBounds.y) * canvasSize.y);
                                positions[posIdx] = leftToCanvas;
                            }
                        }
                    }
                    //dopracowac ta funkcje
                    alpha[posIdx] = Vector3.Distance(player.transform.position, planets[i].transform.position) * (-0.00714f) + 1f;
                    posIdx++;
                }
                //znalezlismy wartosci
                if (posIdx == 2)
                {
                    break;
                }
                i++;
            }
            
            if (positions[0] == Vector2.zero)
            {
                firstPointer.SetActive(false);
            } else
            {
                firstPointer.SetActive(true);
                firstPRect.anchoredPosition = positions[0];
                Color theColorToAdjust = firstPointerImage.color;
                theColorToAdjust.a = alpha[0];
                firstPointerImage.color = theColorToAdjust;
            }

            if (positions[1] == Vector2.zero)
            {
                secondPointer.SetActive(false);
            }
            else
            {
                secondPointer.SetActive(true);
                secondPRect.anchoredPosition = positions[1];
                Color theColorToAdjust = secondPointerImage.color;
                theColorToAdjust.a = alpha[1];
                secondPointerImage.color = theColorToAdjust;
            }

        }
    }
}
