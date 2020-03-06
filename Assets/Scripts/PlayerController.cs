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
    private int lastAtracttedTo = -1;
    private float lastAtracttedToSize = -1;
    public Rigidbody rb;
    private float usedFuel = 0;
    private GameObject scoreGameObject;
    private GameObject fuelGameObject;
    private GameObject boostGameObject;
    private float orbitTime = 0;
    private float fromLastBoost = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreGameObject = GameObject.FindGameObjectWithTag("Score");
        fuelGameObject = GameObject.FindGameObjectWithTag("Fuel");
        boostGameObject = GameObject.FindGameObjectWithTag("Boost");
        rb.velocity = new Vector3(0, 20, 0);
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
        lastAtracttedTo = atractedTo;
        atractedTo = -1;
        planets = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planets)
        {
            if (Vector3.Distance(transform.position, planet.transform.position) < planet.transform.localScale.x * 2f) //jesli odległość jest mniejsza niz promien grawitacji, grawiacja to 2x planeta
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


        //pokazywanie czasu kiedy mogeuzyc boosta
        float boostPassedTime = Time.time - fromLastBoost;
        if (boostPassedTime <= 10)
        {
            boostGameObject.GetComponent<UnityEngine.UI.Text>().text = Convert.ToInt32(boostPassedTime).ToString();
        }
        //ograniczenie max predkosci
        //mniejsze ograniczenie na skrecanie
        if(targetForceSteer.x < 100 && targetForceSteer.y < 5 && targetForceSteer.x > -100 && targetForceSteer.y > -5) //sila do w boki - y moze byc wieksza
        {
            if (Input.GetKey("right") && Input.GetKey("left")) //duzy boost do przodu
            {
                if (boostPassedTime > 10) //co 10s mozna zastosowac duzego boosta
                {
                    targetForceSteer += new Vector3(0, 25, 0);
                    fromLastBoost = Time.time;
                    usedFuel += 40; //dodatkowy koszt boosta
                }
            }
            else if (Input.GetKey("right"))
            {
                targetForceSteer += new Vector3(70 * Time.deltaTime, 0, 0); //kluczowe deltaTime
            }

            else if (Input.GetKey("left"))
            {
                targetForceSteer += new Vector3(-70 * Time.deltaTime, 0, 0); //kluczowe deltaTime
            }
        }

        //wejscie wyjscie z orbity
        if (lastAtracttedTo != atractedTo && lastAtracttedTo == -1) //wlasnie wchodzimy do pola
        {
            orbitTime = Time.time;
            lastAtracttedToSize = planets[atractedTo].transform.localScale.x; //zapisuje wielkosc ostatnio odwiedzanej planety
        }

        if (lastAtracttedTo != atractedTo && lastAtracttedTo != -1) //wlasnie wyszlismy z pola planety trzba dodac boosta
        {
            if (Time.time - orbitTime > 4f) //jezeli czas orbity wiekszy niz 4s
            {
                targetForceSteer += new Vector3(0, 2, 0) * lastAtracttedToSize; //boost zalezny od wielkosci planety
                //uwaga to dolicza sie do zuzycia paliwa ale jest tego malo wiec w razie czego mozna dodac cos do paliwa zeby zrekompensowac
            }
            orbitTime = 0;
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
            float strengthOfAttraction = planets[atractedTo].transform.localScale.x * 1f;
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
            targetForceSteer -= targetForceSteer * 0.025f;
        }
        if (Vector3.Distance(rb.velocity, Vector3.zero) > 20) //predkosc wyciszamy tylko do pewnego momentu
        {
            rb.velocity -= rb.velocity * 0.0013F;
        } else if (Vector3.Distance(rb.velocity, Vector3.zero) < 15) //predkosc nigdy nie moze byc mniejsza niz 15
        {
            targetForceSteer += new Vector3(0, 0.8f, 0); //przyspieszam
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
