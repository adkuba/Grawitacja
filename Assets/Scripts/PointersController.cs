using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            return 1; // pierwsza planeta nie istnieje wiec druga jest blizej
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
            return -1; //pierwsza planeta jest blizej niz druga
        }
        else if (distance1 > distance2)
        {
            return 1; //pierwsza planeta jest dalej niz druga
        }
        else
        {
            return 0;
        }
    }
}

public class PointersController : MonoBehaviour
{
    private GameObject[] planets;
    private GameObject firstPointer;
    private GameObject secondPointer;
    private GameObject player;
    private Vector2 screenBounds;

    // Start is called before the first frame update
    void Start()
    {
        firstPointer = transform.GetChild(0).gameObject; //SetActive(true);
        secondPointer = transform.GetChild(1).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        planets = GameObject.FindGameObjectsWithTag("Planet");
        PlanetsCompare planetsCompare = new PlanetsCompare();
        Array.Sort(planets, planetsCompare);

        //ustalanie pozycji pointerow, odrzucam te ktore sa blizej niz okrag o promieniu do najdalszego punktu kamery (róg ekranu)
        //tylko na poczatku planet moze byc mniej niz dwa wiec moge zrobic taki warunek
        if (planets.Length < 2)
        {
            firstPointer.SetActive(false);
            secondPointer.SetActive(false);

        } else //jesli jest wiecej niz dwie planety
        {
            int i = 0;
            int posIdx = 0;
            Vector2[] positions = new Vector2[2] { Vector2.zero, Vector2.zero };
            float[] alpha = new float[2] { 0, 0 };
            while (i < planets.Length)
            {
                if (Vector3.Distance(player.transform.position, planets[i].transform.position) > screenBounds.magnitude + 19) //19 to max promien planety
                {
                    float a = (player.transform.position.y - planets[i].transform.position.y) / (player.transform.position.x - planets[i].transform.position.x);
                    float b = player.transform.position.y - a * player.transform.position.x;
                    //mamy wzor prostej teraz czas na sprawdzenie
                    Rect cameraView = new Rect(player.transform.position.x - screenBounds.x, player.transform.position.y - screenBounds.y, screenBounds.x * 2, screenBounds.y * 2);
                    float xmin = player.transform.position.x - screenBounds.x;
                    float ymax = player.transform.position.y + screenBounds.y;
                    float xmax = player.transform.position.x + screenBounds.x;
                    float ymin = player.transform.position.y - screenBounds.y;
                    Vector2 left = new Vector2(xmin + 2, a * (xmin + 2) + b);
                    Vector2 up = new Vector2((ymax - 2 - b) / a, ymax - 2);
                    Vector2 right = new Vector2(xmax - 2, a * (xmax - 2) + b);
                    Vector2 down = new Vector2((ymin + 2 - b) / a, ymin + 2);

                    if (planets[i].transform.position.y > player.transform.position.y) //jesli planeta nad playerem
                    {
                        if (planets[i].transform.position.x > player.transform.position.x) //pierwsza cwiartka
                        {
                            if (cameraView.Contains(up))
                            {
                                positions[posIdx] = up;

                            }
                            else //jesli nie na gorze to jest po prawej
                            {
                                positions[posIdx] = right;
                            }

                        } else //druga cwiartka
                        {
                            if (cameraView.Contains(up))
                            {
                                positions[posIdx] = up;

                            }
                            else //jesli nie na gorze to jest po lewej
                            {
                                positions[posIdx] = left;
                            }
                        }

                    } else if (planets[i].transform.position.y < player.transform.position.y)  //jesli planeta ponizej
                    {
                        if (planets[i].transform.position.x > player.transform.position.x) //czwarta cwiartka
                        {
                            if (cameraView.Contains(down))
                            {
                                positions[posIdx] = down;

                            }
                            else //jesli nie na dole to jest po prawej
                            {
                                positions[posIdx] = right;
                            }

                        }
                        else //trzecia cwiartka
                        {
                            if (cameraView.Contains(down))
                            {
                                positions[posIdx] = down;

                            }
                            else //jesli nie na dole to jest po lewej
                            {
                                positions[posIdx] = left;
                            }
                        }
                    }
                    alpha[posIdx] = Vector3.Distance(player.transform.position, planets[i].transform.position) * (-0.00714f) + 0.943f; //dopracowac ta funkcje
                    posIdx++;
                }
                if (posIdx == 2) //znalezlismy wartosci
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
                firstPointer.transform.position = positions[0];
                Color theColorToAdjust = firstPointer.GetComponent<Renderer>().material.color;
                theColorToAdjust.a = alpha[0];
                firstPointer.GetComponent<Renderer>().material.color = theColorToAdjust;
            }

            if (positions[1] == Vector2.zero)
            {
                secondPointer.SetActive(false);
            }
            else
            {
                secondPointer.SetActive(true);
                secondPointer.transform.position = positions[1];
                Color theColorToAdjust = secondPointer.GetComponent<Renderer>().material.color;
                theColorToAdjust.a = alpha[1];
                secondPointer.GetComponent<Renderer>().material.color = theColorToAdjust;
            }

        }
    }
}
