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
    private float fromLastBoost;
    public float fuelTank = 250f; //max ilosc paliwa
    private bool exiting;
    private int score = 0;
    private int highscore;
    public GameObject spawn;
    private float timeFromSpawn;
    private Vector3 desiredScale;
    public GameObject playerShell;

    // Start is called before the first frame update
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0); // nazwa i default value
        rb = GetComponent<Rigidbody>();
        scoreGameObject = GameObject.FindGameObjectWithTag("Score");
        fuelGameObject = GameObject.FindGameObjectWithTag("Fuel");
        boostGameObject = GameObject.FindGameObjectWithTag("Boost");
        fromLastBoost = Time.time;
        timeFromSpawn = Time.time;
        transform.localScale = new Vector3(0.0028f, 0.0094f, 0.0016f);

        exiting = true;
        spawn.GetComponent<ParticleSystem>().Play(); //odtwarzamy efekt spawn
        StartCoroutine(StartingCoroutine());
        desiredScale = new Vector3(0.28f, 0.94f, 0.16f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;
        if (Vector3.Distance(scale, desiredScale) > 0.1 ) //rozmiar playera jest zły
        {
            transform.GetChild(1).gameObject.SetActive(false); //wylaczam silnik
            if (scale.x < desiredScale.x) //playera trzeba zwiekszyc
            {
                transform.localScale *= 1.05f;

            } else //playera trzeba zmniejszyc
            {
                transform.localScale *= .95f;
            }
        } else
        {
            //potencjalny zamulacz kodu bo sie ciagle wywoluje
            if (!exiting)
            {
                transform.GetChild(1).gameObject.SetActive(true); //wlaczam silnik

            } else
            {
                playerShell.SetActive(false);
            }
        }


        if (exiting) //to musze miec dla efektu start (nie chce miec sterowania przez pierwsza sekunde) zmienic nazyw zmiennych! 
        {
            timeFromSpawn = Time.time;
        }
        //paliwo
        float force = Vector3.Distance(Vector3.zero, targetForceSteer);
        if (force > 0.01f) //jesli dziala sila
        {
            usedFuel += force * Time.deltaTime;
        }
        float percent = (fuelTank - usedFuel) * 100 / fuelTank;
        if (percent <= 0)
        {
            //wylaczam efekt silnika plus zabezpieczam sie przed minusowymi procentami ktore moga powstac
            transform.GetChild(1).gameObject.SetActive(false);
            percent = 0;
        }
        fuelGameObject.GetComponent<UnityEngine.UI.Text>().text = Convert.ToInt32(percent).ToString() + "%"; //wyswietlam ilosc POZOSTALEGO paliwa w procentach

        //wynik
        score = Convert.ToInt32(Vector3.Distance(Vector3.zero, transform.position));
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

        //obrot obiektu i kamera
        if (!exiting)
        {
            //ustalamy wielkosc kamery
            Camera.main.orthographicSize = rb.velocity.magnitude * 0.28f + 16f;

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
        }
 

        //pokazywanie czasu kiedy mogeuzyc boosta
        float boostPassedTime = Time.time - fromLastBoost;
        if (boostPassedTime <= 10)
        {
            boostGameObject.GetComponent<UnityEngine.UI.Text>().text = Convert.ToInt32(boostPassedTime).ToString();
        }
        //ograniczenie max predkosci, można sterować jesli mamy paliwo
        //mniejsze ograniczenie na skrecanie
        //sila do w boki - y moze byc wieksza
        if (targetForceSteer.x < 100 && targetForceSteer.y < 5 && targetForceSteer.x > -100 && targetForceSteer.y > -5 && usedFuel < fuelTank && Time.time - timeFromSpawn > 1) 
        {
            if (Input.GetKey("right") && Input.GetKey("left")) //duzy boost do przodu
            {
                if (boostPassedTime > 10 && usedFuel + 100 <= fuelTank) //co 10s mozna zastosowac duzego boosta
                {
                    targetForceSteer += new Vector3(0, 25, 0);
                    fromLastBoost = Time.time;
                    usedFuel += 100; //dodatkowy koszt boosta
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
            //wylaczam efekt tankowania
            transform.GetChild(0).gameObject.SetActive(false);

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
            if (tank && usedFuel > 0 && !exiting)
            {
                transform.GetChild(0).gameObject.SetActive(true); //dodaje efekt tankowania
                transform.GetChild(1).gameObject.SetActive(true); //upewniam sie ze efekt silnika jest wlaczony (moze zdarzyc sie ze zatankuje po wylaczeniu silnika)
                usedFuel -= 0.15f; //tankuje
            } else if (usedFuel <= 0)
            {
                transform.GetChild(0).gameObject.SetActive(false); //pelny bak wylaczam efekt tankowania
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
        if (targetForceSteer.magnitude > 0.0001f)
        {
            targetForceSteer -= targetForceSteer * 0.025f;
        }
        if (rb.velocity.magnitude > 20 && percent > 0) //predkosc wyciszamy tylko do pewnego momentu, dziala jezeli mamy paliwo!
        {
            rb.velocity -= rb.velocity * 0.0013F;
        } else if (rb.velocity.magnitude < 15 && percent > 0 && !exiting) //predkosc nigdy nie moze byc mniejsza niz 15, jezeli mamy paliwo i nie odtwarzamy spawn
        {
            targetForceSteer += new Vector3(0, 0.8f, 0); //przyspieszam
        }

        rb.AddForce(targetForce); //stosujemy sile
        rb.AddRelativeForce(targetForceSteer); //SILA RELATYWNY KIERUNEK, teraz nie musze obliczac tych katow i wgl


        //jesli nie mamy paliwa to wyciszamy ruch
        if (percent <= 0) 
        {
            rb.velocity -= rb.velocity * 0.002f;
            if (rb.velocity.magnitude < 2f) //jesli sie zatrzymalismy to koniec gry
            {
                //Tutaj dodac end sequence
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
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
        rb.isKinematic = true;
        exiting = true;
        //zapisywanie highscore
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore", highscore);
        }
        spawn.GetComponent<ParticleSystem>().Play(); //odtwarzamy efekt spawn
        StartCoroutine(ExitingCoroutine());
    }

    IEnumerator ExitingCoroutine()
    {
        desiredScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator StartingCoroutine()
    {
        yield return new WaitForSeconds(0.5f); //czekamy az spawn sie odtworzy
        exiting = false;
        yield break; //wychodzimy z coroutine
    }
}
