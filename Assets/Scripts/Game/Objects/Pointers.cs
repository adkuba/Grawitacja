using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

class PlanetsCompare : IComparer<GameObject>
{
    private GameObject player = GameObject.FindGameObjectWithTag("Player");

    //gameobject distance from player compare
    public int Compare(GameObject planet1, GameObject planet2)
    {
        if (planet1 == null && planet2 == null)
        {
            return 0;
        }
        else if (planet1 == null)
        {
            //second is closer
            return 1;
        }
        else if (planet2 == null)
        {
            return -1;
        }

        //no nulls
        float distance1 = Vector3.Distance(player.transform.position, planet1.transform.position);
        float distance2 = Vector3.Distance(player.transform.position, planet2.transform.position);
        if (distance1 < distance2)
        {
            return -1;
        }
        else if (distance1 > distance2)
        {
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

    void Start()
    {
        firstPointer = GameObject.FindGameObjectWithTag("Pointer1");
        secondPointer = GameObject.FindGameObjectWithTag("Pointer2");
        firstPointerImage = firstPointer.GetComponent<UnityEngine.UI.Image>();
        secondPointerImage = secondPointer.GetComponent<UnityEngine.UI.Image>();
        firstPRect = firstPointer.GetComponent<RectTransform>();
        secondPRect = secondPointer.GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<Canvas>();
        canvasSize = new Vector2(canvas.GetComponent<RectTransform>().rect.width / 2, canvas.GetComponent<RectTransform>().rect.height / 2);
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
    } 

    void Update()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        PlanetsCompare planetsCompare = new PlanetsCompare();
        Array.Sort(planets, planetsCompare);

        //2 planets required, only objects further than screen.magnitude + offsets
        if (planets.Length < 2)
        {
            firstPointer.SetActive(false);
            secondPointer.SetActive(false);

        }
        else
        {
            int i = 0;
            int posIdx = 0;
            Vector2[] positions = new Vector2[2] { Vector2.zero, Vector2.zero };
            float[] alpha = new float[2] { 0, 0 };
            float[] angles = new float[2] { 0, 0 };
            while (i < planets.Length)
            {
                //19 is the max planet diameter
                if (Vector3.Distance(player.transform.position, planets[i].transform.position) > screenBounds.magnitude + 19)
                {
                    Vector3 direction = planets[i].transform.position - player.transform.position;
                    direction.Normalize();
                    direction *= canvasSize.magnitude;

                    //max values +- offset
                    float offset = 20;

                    if (direction.x > canvasSize.x - offset)
                    {
                        direction.x = canvasSize.x - offset;
                    }
                    if (direction.x < -canvasSize.x + offset)
                    {
                        direction.x = -canvasSize.x + offset;
                    }
                    if (direction.y > canvasSize.y - offset)
                    {
                        direction.y = canvasSize.y - offset;
                    }
                    if (direction.y < -canvasSize.y + offset)
                    {
                        direction.y = -canvasSize.y + offset;
                    }
                    positions[posIdx] = direction;

                    //angle
                    angles[posIdx] = Vector3.Angle(Vector3.up, direction);
                    if (player.transform.position.x < planets[i].transform.position.x)
                    {
                        angles[posIdx] *= -1;
                    }

                    //alpha value
                    alpha[posIdx] = Vector3.Distance(player.transform.position, planets[i].transform.position) * (-0.00714f) + 1f;
                    posIdx++;
                }
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
                firstPRect.rotation = Quaternion.Euler(0, 0, angles[0]);
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
                secondPRect.rotation = Quaternion.Euler(0, 0, angles[1]);
                Color theColorToAdjust = secondPointerImage.color;
                theColorToAdjust.a = alpha[1];
                secondPointerImage.color = theColorToAdjust;
            }

        }
    }
}
