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
    private int atractedTo = -1;
    private int lastAtracttedTo = -1;
    private float lastAtracttedToSize = -1;
    private float usedFuel = 0;
    private float orbitTime = 0;
    private float fromLastBoost;
    public float fuelTank = 400f;
    private bool exiting;
    private int score = 0;
    private int highscore;
    private float timeFromSpawn;
    private Vector3 desiredScale;
    private bool adYesClicked = false;
    private bool adNoClicked = false;
    private int addScore = 0;
    //highScoreAnimationCompleted
    private bool hsAnimationComp = false;
    //animacje
    private float waitAnimation = 0;
    private bool cockpitUp = false;
    private bool textUp = false;
    private bool textDown = false;
    private bool cockpitDown = false;

    private GameObject[] planets;
    private GameObject scoreGameObject;
    private GameObject fuelGameObject;
    private GameObject boostGameObject;
    private GameObject spawn;
    private GameObject cockpitImage;
    private GameObject adManager;
    private GameObject adYes;
    private GameObject adNo;
    private GameObject shell;
    private GameObject engine;
    private GameObject fuelEffect;
    private Rigidbody rb;
    private RectTransform cockpitImageRect;
    private UnityEngine.UI.Image fuelImage;
    private UnityEngine.UI.Image boostImage;
    private UnityEngine.UI.Text scoreText;


    // Start is called before the first frame update
    void Start()
    {
        scoreGameObject = GameObject.FindGameObjectWithTag("Score");
        shell = GameObject.FindGameObjectWithTag("Shell");
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        cockpitImage = GameObject.FindGameObjectWithTag("Cockpit");
        fuelGameObject = GameObject.FindGameObjectWithTag("Fuel");
        boostGameObject = GameObject.FindGameObjectWithTag("Boost");
        adManager = GameObject.FindGameObjectWithTag("AdManager");
        adYes = GameObject.FindGameObjectWithTag("AdYes");
        adNo = GameObject.FindGameObjectWithTag("AdNo");
        engine = GameObject.FindGameObjectWithTag("Engine");
        fuelEffect = GameObject.FindGameObjectWithTag("FuelEffect");
        
        //odtwarzamy efekt spawn
        //kolejnosc tych komend jest wazna
        transform.localScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        exiting = true;
        spawn.GetComponent<ParticleSystem>().Play();
        StartCoroutine(StartingCoroutine());
        desiredScale = new Vector3(0.28f, 0.94f, 0.16f);
        
        rb = GetComponent<Rigidbody>();
        cockpitImageRect = cockpitImage.GetComponent<RectTransform>();
        fuelImage = fuelGameObject.GetComponent<UnityEngine.UI.Image>();
        boostImage = boostGameObject.GetComponent<UnityEngine.UI.Image>();
        scoreText = scoreGameObject.GetComponent<UnityEngine.UI.Text>();

        // nazwa i default value
        fuelEffect.SetActive(false);
        highscore = PlayerPrefs.GetInt("highscore", 0);
        addScore = PlayerPrefs.GetInt("addScore", 0);
        fromLastBoost = Time.time;
        timeFromSpawn = Time.time;
        adYes.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(AdYesButtonClicked);
        adNo.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(AdNoButtonClicked);
        adManager.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;
        //rozmiar playera jest zły
        if (Vector3.Distance(scale, desiredScale) > 0.1 )
        {
            //wylaczam silnik
            engine.gameObject.SetActive(false);
            //playera trzeba zwiekszyc
            if (scale.x < desiredScale.x) 
            {
                transform.localScale *= 1.05f;

            }
            //playera trzeba zmniejszyc
            else
            {
                transform.localScale *= .95f;
            }
        } else
        {
            //potencjalny zamulacz kodu bo sie ciagle wywoluje
            if (!exiting)
            {
                //wlaczam silnik
                engine.SetActive(true);
                shell.SetActive(true);

            }
        }

        //to musze miec dla efektu start (nie chce miec sterowania przez pierwsza sekunde) zmienic nazyw zmiennych! 
        if (exiting)
        {
            timeFromSpawn = Time.time;
        }
        //paliwo
        float force = Vector3.Distance(Vector3.zero, targetForceSteer);
        //jesli dziala sila
        if (force > 0.01f) 
        {
            //tu zamiast deltatime musze miec chyba mala wartosc bo delta time sie zmienia
            usedFuel += force * 0.005f;
        }
        float percent = (fuelTank - usedFuel) * 100 / fuelTank;
        if (percent <= 0)
        {
            //zabezpieczam sie przed minusowymi procentami ktore moga powstac
            percent = 0;
        }


        //wynik, paliwo, boost
        //fillAmount od 0 do 1
        fuelImage.fillAmount = percent / 100;
        score = Convert.ToInt32(Vector3.Distance(Vector3.zero, transform.position) * 0.1f) + addScore;
        scoreText.text = score.ToString();
        float boostPassedTime = Time.time - fromLastBoost;
        if (boostPassedTime < 7)
        {
            boostImage.fillAmount = boostPassedTime / 7;

        }
        //boost jest wiekszy niz 10s
        else
        {
            boostImage.fillAmount = 1;
        }


        //animacja highscore, wywoluje sie dopoki nie skonczymy animacji
        //wywola sie tylko raz!
        if (score > highscore && !hsAnimationComp)
        {
            //jesli jeszcze nie podnioslem kokpitu
            if (!cockpitUp)
            {
                if (cockpitImageRect.anchoredPosition.y < -10)
                {
                    cockpitImageRect.anchoredPosition += new Vector2(0, 30 * Time.deltaTime);

                } else
                {
                    cockpitUp = true;
                }
            }

            //jesli jeszcze nie podnioslem tekstu ale kokpit jest juz podniesiony
            if (!textUp && cockpitUp)
            {
                if (scoreText.fontSize < 32)
                {
                    scoreText.fontSize += 1;
                    waitAnimation = Time.time;

                } else
                {
                    textUp = true;
                }
            }

            //dodatkowo czekamy sekunde
            if (!textDown && cockpitUp && textUp && Time.time - waitAnimation > 1)
            {
                if (scoreText.fontSize > 23)
                {
                   scoreText.fontSize -= 1;

                } else
                {
                    textDown = true;
                }
            }

            if (!cockpitDown && cockpitUp && textUp && textDown)
            {
                if (cockpitImageRect.anchoredPosition.y > -25)
                {
                    cockpitImageRect.anchoredPosition -= new Vector2(0, 30 * Time.deltaTime);

                } else
                {
                    cockpitDown = true;
                }
            }

            //koniec animacji
            if (cockpitUp && textUp && textDown && cockpitDown)
            {
                hsAnimationComp = true;
            }
        }


        //sprawdzanie planet
        lastAtracttedTo = atractedTo;
        atractedTo = -1;
        planets = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planets)
        {
            //jesli odległość jest mniejsza niz promien grawitacji, grawiacja to 2x planeta
            if (Vector3.Distance(transform.position, planet.transform.position) < planet.transform.localScale.x * 2f) 
            {
                atractedTo = Array.IndexOf(planets, planet);
                break;
            }
        }

        //obrot obiektu i kamera
        if (!exiting)
        {
            //ustalamy wielkosc kamery
            float cameraSize = rb.velocity.magnitude * 0.28f + 16f;
            //min
            if (cameraSize < 20) 
            {
                cameraSize = 20;
            }
            //max
            if (cameraSize > 30) 
            {
                cameraSize = 30;
            }
            Camera.main.orthographicSize = cameraSize;

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


        //ograniczenie max predkosci, można sterować jesli mamy paliwo
        //mniejsze ograniczenie na skrecanie
        //sila do w boki - y moze byc wieksza
        //Input.touchCount > 0
        if (targetForceSteer.x < 100 && targetForceSteer.y < 5 && targetForceSteer.x > -100 && targetForceSteer.y > -5 && usedFuel < fuelTank && Time.time - timeFromSpawn > 1 && Input.touchCount > 0) 
        {
            
            Touch touch = Input.GetTouch(0);
            Vector2 pos = touch.position;
            //Input.GetKey("right")
            //Input.GetKey("left")
            //pos.x > Screen.width / 2
            //pos.x < Screen.width / 2

            //duzy boost do przodu
            if (pos.x > Screen.width / 2 && pos.x < Screen.width / 2)
            {
                //co 10s mozna zastosowac duzego boosta
                if (boostPassedTime > 7 && usedFuel + 40 <= fuelTank)
                {
                    targetForceSteer += new Vector3(0, 25, 0);
                    fromLastBoost = Time.time;
                    //dodatkowy koszt boosta
                    usedFuel += 40; 
                }
            }
            else if (pos.x > Screen.width / 2)
            {
                //kluczowe deltaTime
                targetForceSteer += new Vector3(70 * Time.deltaTime, 0, 0);
                shell.transform.Rotate(new Vector3(0, -1, 0) * 70 * Time.deltaTime);
            }

            else if (pos.x < Screen.width / 2)
            {
                //kluczowe deltaTime
                targetForceSteer += new Vector3(-70 * Time.deltaTime, 0, 0); 
                shell.transform.Rotate(new Vector3(0, 1, 0) * 70 * Time.deltaTime);
            }
        }

        //wejscie wyjscie z orbity
        //wlasnie wchodzimy do pola
        if (lastAtracttedTo != atractedTo && lastAtracttedTo == -1) 
        {
            orbitTime = Time.time;
            //zapisuje wielkosc ostatnio odwiedzanej planety
            lastAtracttedToSize = planets[atractedTo].transform.localScale.x; 
        }

        //wlasnie wyszlismy z pola planety trzba dodac boosta
        if (lastAtracttedTo != atractedTo && lastAtracttedTo != -1)
        {
            //wylaczam efekt tankowania
            fuelEffect.SetActive(false);

            //jezeli czas orbity wiekszy niz 4s
            if (Time.time - orbitTime > 4f)
            {
                //boost zalezny od wielkosci planety
                targetForceSteer += new Vector3(0, 2, 0) * lastAtracttedToSize;
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
                //dodaje efekt tankowania
                fuelEffect.SetActive(true);
                //upewniam sie ze efekt silnika jest wlaczony (moze zdarzyc sie ze zatankuje po wylaczeniu silnika)
                engine.SetActive(true);
                usedFuel -= 0.15f; //tankuje
            } 
            else if (usedFuel <= 0)
            {
                fuelEffect.SetActive(false); //pelny bak wylaczam efekt tankowania
            }
            planetForce = planets[atractedTo].transform.position - transform.position;
            //SILA przyciagania jest wieksza wraz ze zmniejszeniem sie dystansu
            float strengthOfAttraction = planets[atractedTo].transform.localScale.x * 1f;
            targetForce = planetForce * strengthOfAttraction / Vector3.Distance(transform.position, planets[atractedTo].transform.position);
        

        }
        //jak jestesmy poza dzialaniem planety to wylaczamy sile planety
        else
        {
            if (Vector3.Distance(targetForce, Vector3.zero) > 0.0001f)
            {
                //Vector3.zero;
                targetForce -= targetForce * 0.02F;
            }
        }

        //wyciszamy ogolna predkosc i sile ze sterowania
        if (targetForceSteer.magnitude > 0.0001f)
        {
            targetForceSteer -= targetForceSteer * 0.025f;
        }
        //predkosc wyciszamy tylko do pewnego momentu, dziala jezeli mamy paliwo!
        if (rb.velocity.magnitude > 20 && usedFuel < fuelTank)
        {
            rb.velocity -= rb.velocity * 0.0013F;
        }
        //predkosc nigdy nie moze byc mniejsza niz 15, jezeli mamy paliwo i nie odtwarzamy spawn
        else if (rb.velocity.magnitude < 15 && usedFuel < fuelTank && !exiting) 
        {
            //przyspieszam
            targetForceSteer += new Vector3(0, 0.8f, 0); 
        }

        //stosujemy sile
        rb.AddForce(targetForce);
        //SILA RELATYWNY KIERUNEK, teraz nie musze obliczac tych katow i wgl
        rb.AddRelativeForce(targetForceSteer);


        //jesli nie mamy paliwa to wyciszamy ruch
        if (usedFuel > fuelTank) 
        {
            //wylaczam efekt silnika
           engine.SetActive(false);

            rb.velocity -= rb.velocity * 0.002f;
            //jesli sie zatrzymalismy to koniec gry
            if (rb.velocity.magnitude < 7) 
            {
                endSequence();
            }
        }

        //jesli wychodzimy i kliknelismy ktorys z guzikow
        //DODAC OGRANICZENIE NA ILOSC KLIKANIA REKLAM - np tylko jedna w sesji
        if (exiting)
        {
            if (adNoClicked)
            {
                //konczymy gre
                adManager.SetActive(false);
                endSequenceFinal();
            }
            if (adYesClicked)
            {
                adManager.SetActive(false);
                //MUSZE WYSWIETLIC REKLAME
                endReload();
            }
        }
    }

    //kat, TO MUSI BYC Z VELOCITY WYLICZANE
    private float steer(float x1, float y1) 
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
        endSequence();
    }

    void endReload()
    {
        //zapisuje wynik
        PlayerPrefs.SetInt("addScore", score);
        
        //odtwarzamy efekt spawn
        if (!spawn.GetComponent<ParticleSystem>().isPlaying)
        {
            spawn.GetComponent<ParticleSystem>().Play();
        }

        //reload
        StartCoroutine(ReloadingCoroutine());
    }

    //wykonujemy na koniec
    void endSequence()
    {
        //zatrzymujemy playera
        exiting = true;
        rb.isKinematic = true;

        //wybor czy wyswietlamy reklame
        adManager.SetActive(true);
        //jesli juz raz wyswietlilismy reklame
        if (PlayerPrefs.GetInt("addScore", 0) > 0)
        {
            adYes.SetActive(false);
        }
    }

    //finalne wychodzenie
    void endSequenceFinal()
    {
        //zapisywanie highscore
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore", highscore);
            PlayerPrefs.SetInt("highscoreChanged", 1);
        }
        //odtwarzamy efekt spawn
        //aaaaa to jest zajebiscie wazne, opis w #18!!!!
        if (!spawn.GetComponent<ParticleSystem>().isPlaying) 
        {
            spawn.GetComponent<ParticleSystem>().Play();
        }
        StartCoroutine(ExitingCoroutine());
    }

    IEnumerator ReloadingCoroutine()
    {
        desiredScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator ExitingCoroutine()
    {
        desiredScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator StartingCoroutine()
    {
        //czekamy az spawn sie odtworzy
        yield return new WaitForSeconds(0.5f); 
        exiting = false;
        yield break; //wychodzimy z coroutine
    }

    //lisener dla buttonow od reklam
    void AdYesButtonClicked()
    {
        adYesClicked = true;
    }

    void AdNoButtonClicked()
    {
        adNoClicked = true;
    }
}
