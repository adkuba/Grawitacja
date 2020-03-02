using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Vector3 targetForce = Vector3.zero;
    private Vector3 targetForceSteer = Vector3.zero;
    private Vector3 planetForce = Vector3.zero;
    private GameObject[] planets;
    private int atractedTo = -1;
    public Rigidbody rb;
    private float usedFuel = 0;
    private GameObject scoreGameObject;
    private GameObject fuelGameObject;
    private float yAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreGameObject = GameObject.FindGameObjectWithTag("Score");
        fuelGameObject = GameObject.FindGameObjectWithTag("Fuel");
        rb.velocity = new Vector3(0, 5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //paliwo
        float force = Vector3.Distance(Vector3.zero, targetForceSteer);
        if (force > 0.01f) //jesli dziala sila
        {
            usedFuel += force * Time.deltaTime;
        }
        fuelGameObject.GetComponent<UnityEngine.UI.Text>().text = Convert.ToInt32(usedFuel).ToString();

        //wynik
        int score = Convert.ToInt32(Vector3.Distance(Vector3.zero, transform.position));
        scoreGameObject.GetComponent<UnityEngine.UI.Text>().text = score.ToString();

        //sprawdzanie planet
        atractedTo = -1;
        planets = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planets)
        {
            if (Vector3.Distance(transform.position, planet.transform.position) < planet.transform.localScale.x * 3f / 2f) //jesli odległość jest mniejsza niz promien grawitacji
            {
                atractedTo = Array.IndexOf(planets, planet);
                break;
            }
        }

        //obrot obiektu
        transform.Rotate(new Vector3(0, 1, 0) * 30 * Time.deltaTime);
        float coordinate = steer(rb.velocity.x, rb.velocity.y);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        if (rb.velocity.x > 0 && rb.velocity.y > 0)
        {
            rotation = Quaternion.Euler(0, 0, coordinate - 90f);
        } 
        else if (rb.velocity.x < 0 && rb.velocity.y > 0)
        {
            rotation = Quaternion.Euler(0, 0, 90f - coordinate);
        }
        else if (rb.velocity.x < 0 && rb.velocity.y < 0)
        {
            rotation = Quaternion.Euler(0, 0, -coordinate + 90f);
        }
        else if (rb.velocity.x > 0 && rb.velocity.y < 0)
        {
            rotation = Quaternion.Euler(0, 0, coordinate - 90f);
        }

        transform.rotation = rotation;


        //ograniczenie max predkosci
        //mniejsze ograniczenie na skrecanie
        if(targetForceSteer.x < 100 && targetForceSteer.y < 5 && targetForceSteer.x > -100 && targetForceSteer.y > -5) //sila do w boki - y moze byc wieksza
        {
            if (Input.GetKey("right"))
            {
                targetForceSteer += new Vector3(50 * Time.deltaTime, 0, 0); //kluczowe deltaTime
            }

            if (Input.GetKey("left"))
            {
                targetForceSteer += new Vector3(-50 * Time.deltaTime, 0, 0); //kluczowe deltaTime
                /*
                double radians = Math.Atan2(rb.velocity.y, rb.velocity.x);
                double len = rb.velocity.magnitude;
                radians += (Math.PI / 180); //dodaje jeden stopnia
                rb.velocity = new Vector2((float)(Math.Cos(radians) * len), (float)(Math.Sin(radians) * len));
                usedFuel += 2 * Time.deltaTime; //zuzywam paliwo
                */
            }

            if (Input.GetKey("up"))
            {
                targetForceSteer += new Vector3(0, 10 * Time.deltaTime, 0);
            }

            if (Input.GetKey("down"))
            {
                targetForceSteer += new Vector3(0, -10 * Time.deltaTime, 0);
            }
        }

        
        //dodajemy sile od planety
        if (atractedTo != -1)
        {
            bool tank = planets[atractedTo].GetComponent<Planet>().fuel;
            if (tank && usedFuel > 0)
            {
                usedFuel -= 0.01f; //tankuje
            }
            planetForce = planets[atractedTo].transform.position - transform.position;
            //SILA przyciagania jest wieksza wraz ze zmniejszeniem sie dystansu
            float strengthOfAttraction = planets[atractedTo].transform.localScale.x * 0.8f;
            targetForce = planetForce * strengthOfAttraction / Vector3.Distance(transform.position, planets[atractedTo].transform.position);
        

        } else //jak jestesmy poza dzialaniem planety to wylaczamy sile planety
        {
            if (Vector3.Distance(targetForce, Vector3.zero) > 0.0001f)
            {
                targetForce -= targetForce * 0.02F; //Vector3.zero;
            }
        }

        //wyciszamy ogolna predkosc i sile ze sterowania
        if (Vector3.Distance(targetForceSteer, Vector3.zero) > 0.0001f)
        {
            targetForceSteer -= targetForceSteer * 0.02F;
        }
        if (Vector3.Distance(rb.velocity, Vector3.zero) > 25) //predkosc wyciszamy tylko do pewnego momentu
        {
            rb.velocity -= rb.velocity * 0.0012F;
        }

        rb.AddForce(targetForce); //stosujemy sile
        rb.AddRelativeForce(targetForceSteer); //SILA RELATYWNY KIERUNEK, teraz nie musze obliczac tych katow i wgl

        
    }

    private float steer(float x1, float y1) //kat, TO MUSI BYC Z VELOCITY WYLICZANE
    {
        if(x1 == 0 || y1 == 0)
        {
            return 0f;
        }
        double sin = y1 / Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2));
        double alfa = (Math.Asin(sin) * 180) / Math.PI;
        return (float)alfa;
    }

    //kolizja
    void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
