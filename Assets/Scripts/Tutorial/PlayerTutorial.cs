using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerTutorial : MonoBehaviour
{
    private Vector3 targetForce = Vector3.zero;
    private Vector3 targetForceSteer = Vector3.zero;
    private Vector3 planetForce = Vector3.zero;
    private int atractedTo = -1;
    private int lastAtracttedTo = -1;
    private float usedFuel = 0;
    private float orbitTime = 0;
    private float fromLastBoost;
    public float fuelTank = 400f;
    private bool exiting;
    private float score = 0;
    private int highscore;
    private float timeFromSpawn;
    private Vector3 desiredScale;
    private float timeFromLastPlanet;
    private bool endSeguenceInvoked = false;
    private int scoreBoost = 0;
    private float steerAddition = 0;
    private bool slowdownPlanet = false;
    private float slowdownAngle = 0;
    private float desiredTimeScale;
    private float slowdownCameraAngle = -1;
    private int addScore = 0;
    
    private bool leftTut = false;
    private bool rightTut = false;
    private bool boostTut = false;

    private GameObject[] planets;
    private GameObject[] asteroids;
    private GameObject scoreGameObject;
    private GameObject fuelGameObject;
    private GameObject boostGameObject;
    private GameObject spawn;
    private GameObject boostAdd;
    private GameObject cockpitImage;
    private GameObject shell;
    private GameObject engine;
    private GameObject fuelEffect;
    private Rigidbody rb;
    private RectTransform cockpitImageRect;
    private UnityEngine.UI.Image fuelImage;
    private UnityEngine.UI.Image boostImage;
    private UnityEngine.UI.Text scoreText;
    private UnityEngine.UI.Text boostAddText;
    private AudioSource crashSound;
    private GameObject infoTextGO;
    private UnityEngine.UI.Text infoText;
    private Vector2 screenBounds;
    private GameObject quit;

    private string leftTutText = "Click left side of the device";
    private string rightTutText = "Click right side";
    private string boostTutText = "Click both to activate boost\nLeft circle must be full and not transparent";
    private string fuelTutText = "No orbiting = more fuel used\nGo after the pointer!";
    private string orbitTutText = "Some planets have fuel\nFly as far as possible";
    private string finalTutText = "Great that's it!";

    public GameObject planetPrefab;


    void Start()
    {
        scoreGameObject = GameObject.FindGameObjectWithTag("Score");
        shell = GameObject.FindGameObjectWithTag("Shell");
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        cockpitImage = GameObject.FindGameObjectWithTag("Cockpit");
        fuelGameObject = GameObject.FindGameObjectWithTag("Fuel");
        boostGameObject = GameObject.FindGameObjectWithTag("Boost");
        engine = GameObject.FindGameObjectWithTag("Engine");
        fuelEffect = GameObject.FindGameObjectWithTag("FuelEffect");
        boostAdd = GameObject.FindGameObjectWithTag("BoostAdd");
        infoTextGO = GameObject.FindGameObjectWithTag("InfoText");
        quit = GameObject.FindGameObjectWithTag("Quit");

        crashSound = GetComponent<AudioSource>();

        //spawn effect, order of code is important!
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
        boostAddText = boostAdd.GetComponent<UnityEngine.UI.Text>();
        infoText = infoTextGO.GetComponent<UnityEngine.UI.Text>();
        quit.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(QuitButton);

        infoTextGO.SetActive(false);
        fuelEffect.SetActive(false);
        highscore = PlayerPrefs.GetInt("highscore", 0);
        fromLastBoost = Time.time;
        timeFromSpawn = Time.time;
        boostAdd.SetActive(false);
        timeFromLastPlanet = Time.time;
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);

        //zmiana jezyka
        if (Application.systemLanguage == SystemLanguage.Polish)
        {
            quit.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Wyjdz";
            leftTutText = "Kliknij lewa strone urzadzenia";
            rightTutText = "Kliknij prawa strone";
            boostTutText = "Kliknij obydwie aby aktywowac boost\nLewe kolo musi byc pelne i aktywne";
            fuelTutText = "Brak orbitowania = wiecej zuzytego paliwa\nLec za wskaznikiem!";
            orbitTutText = "Niektore planety maja paliwo\nDolec jak najdalej potrafisz";
            finalTutText = "Super to wszystko!";
        }
    }


    void Update()
    {
        //tutorial
        desiredTimeScale = 0.8f;
        if (Time.time - timeFromSpawn > 2)
        {
            if (PlayerPrefs.GetInt("onlyPTut", 0 ) == 1)
            {
                leftTut = true;
                rightTut = true;
                boostTut = true;
            }

            infoText.text = leftTutText;
            if (!boostTut)
            {
                usedFuel = 0;
            }
            infoTextGO.SetActive(true);
            //zrobiony skret w lewo
            if (leftTut)
            {
                infoText.text = rightTutText;
                //zrobiony skret w prawo
                if (rightTut)
                {
                    infoText.text = boostTutText;
                    //dodatkowe info o tym kiedy mozna to uzywac
                    //zrobiny boost
                    if (boostTut)
                    {
                        //musze zmniejszyc paliwo
                        infoText.text = fuelTutText;
                        //musze wygenerowac planete
                        planets = GameObject.FindGameObjectsWithTag("Planet");
                        if (planets.Length == 0 && Time.time - fromLastBoost > 3)
                        {
                            GameObject planet = Instantiate(planetPrefab) as GameObject;
                            float size = UnityEngine.Random.Range(14f, 16f);
                            planet.transform.localScale = new Vector3(size, size, size);
                            float minx = transform.position.x + screenBounds.x + 100;
                            float maxx = minx + 10;
                            float miny = transform.position.y + screenBounds.y + 100;
                            float maxy = miny + 10;
                            Vector2 xrange = new Vector2(minx, maxx);
                            Vector2 yrange = new Vector2(miny, maxy);
                            //80 between planet - planet, 40 betwen planet - asteroid
                            Vector2 position = new Vector2(UnityEngine.Random.Range(xrange.x, xrange.y), UnityEngine.Random.Range(yrange.x, yrange.y));
                            planet.transform.position = position;

                            GameObject planet2 = Instantiate(planetPrefab) as GameObject;
                            size = UnityEngine.Random.Range(14f, 16f);
                            planet2.transform.localScale = new Vector3(size, size, size);
                            maxx = transform.position.x - screenBounds.x - 100;
                            minx = maxx - 10;
                            maxy = transform.position.y + screenBounds.y + 100;
                            miny = maxy - 10;
                            xrange = new Vector2(minx, maxx);
                            yrange = new Vector2(miny, maxy);
                            //80 between planet - planet, 40 betwen planet - asteroid
                            Vector2 position2 = new Vector2(UnityEngine.Random.Range(xrange.x, xrange.y), UnityEngine.Random.Range(yrange.x, yrange.y));
                            planet2.transform.position = position2;
                        }

                        //orbituje od 2 sek
                        if (atractedTo != -1)
                        {
                            infoText.text = orbitTutText;
                            if (Time.time - orbitTime > 4)
                            {
                                infoText.text = finalTutText;
                            }
                            if (Time.time - orbitTime > 7)
                            {
                                if (!endSeguenceInvoked)
                                {
                                    endSeguenceInvoked = true;
                                    crashSound.Play();
                                    exiting = true;
                                    rb.isKinematic = true;

                                    if (!spawn.GetComponent<ParticleSystem>().isPlaying)
                                    {
                                        spawn.GetComponent<ParticleSystem>().Play();
                                    }
                                    StartCoroutine(ExitingCoroutine());
                                }
                            }
                        }
                    }
                }
            }
        }

        //player size
        Vector3 scale = transform.localScale;
        if (Vector3.Distance(scale, desiredScale) > 0.1)
        {
            engine.gameObject.SetActive(false);

            if (scale.x < desiredScale.x)
            {
                transform.localScale *= 1f + (2 * Time.deltaTime);
            }
            else
            {
                transform.localScale *= 1f - (2 * Time.deltaTime);
            }
        }
        else
        {
            //optimalization? (constantly invoking)
            if (!exiting)
            {
                engine.SetActive(true);
                shell.SetActive(true);
            }
        }

        //time for steering start
        if (exiting)
        {
            timeFromSpawn = Time.time;
        }

        //fuel
        float force = Vector3.Distance(Vector3.zero, targetForceSteer);
        if (force > 0.01f)
        {
            usedFuel += force * Time.deltaTime * 0.2f;
        }
        float percent = (fuelTank - usedFuel) * 100 / fuelTank;
        if (percent <= 0)
        {
            percent = 0;
        }

        //more fuel when not orbiting
        if (Time.time - timeFromLastPlanet > 10 && !exiting && boostTut)
        {
            usedFuel += Time.deltaTime * 5;

            if (fuelImage.rectTransform.sizeDelta.x < 70)
            {
                fuelImage.rectTransform.sizeDelta += new Vector2(10, 10) * Time.deltaTime;
            }
            if (cockpitImageRect.anchoredPosition.y < -10)
            {
                cockpitImageRect.anchoredPosition += new Vector2(0, 10 * Time.deltaTime);
            }

        }
        else if (fuelImage.rectTransform.sizeDelta.x > 50)
        {
            fuelImage.rectTransform.sizeDelta -= new Vector2(10, 10) * Time.deltaTime;
        }
        else if (cockpitImageRect.anchoredPosition.y > -25)
        {
            cockpitImageRect.anchoredPosition -= new Vector2(0, 10 * Time.deltaTime);
        }

        //score, boost, fuel cockpit
        fuelImage.fillAmount = percent / 100;
        int desiredScore = Convert.ToInt32(Vector3.Distance(Vector3.zero, transform.position) * 0.4f) + addScore;
        if (score < desiredScore)
        {
            score += 20 * Time.deltaTime;
            if (score > desiredScore)
            {
                score = desiredScore;
            }
        }
        else if (score > desiredScore)
        {
            score -= 20 * Time.deltaTime;
            if (score < desiredScore)
            {
                score = desiredScore;
            }
        }
        scoreText.text = Convert.ToInt32(score).ToString();
        slowdownAngle = 45f;
        float boostPassedTime = Time.time - fromLastBoost;
        if (boostPassedTime < 7)
        {
            boostImage.fillAmount = boostPassedTime / 7;
        }
        else
        {
            boostImage.fillAmount = 1;
        }

        //planet checking
        lastAtracttedTo = atractedTo;
        atractedTo = -1;
        planets = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planets)
        {
            //gravity is 2x planet radius
            if (Vector3.Distance(transform.position, planet.transform.position) < planet.transform.localScale.x * 2f)
            {
                atractedTo = Array.IndexOf(planets, planet);
                break;
            }
        }

        //player rotation and camera
        if (!exiting)
        {
            float cameraSize = 0;
            if (slowdownCameraAngle == -1)
            {
                cameraSize = rb.velocity.magnitude * 0.21f + 21.58f;
                if (cameraSize < 26)
                {
                    cameraSize = 26;
                }
                if (cameraSize > 30)
                {
                    cameraSize = 30;
                }
            }
            //focus on slowdown
            else
            {
                cameraSize = slowdownCameraAngle;
            }
            float actualCameraSize = Camera.main.orthographicSize;
            if (actualCameraSize < cameraSize)
            {
                actualCameraSize += 20 * Time.deltaTime;
                if (actualCameraSize > cameraSize)
                {
                    actualCameraSize = cameraSize;
                }
            }
            if (actualCameraSize > cameraSize)
            {
                actualCameraSize -= 20 * Time.deltaTime;
                if (actualCameraSize < cameraSize)
                {
                    actualCameraSize = cameraSize;
                }
            }

            Camera.main.orthographicSize = actualCameraSize;

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


        //steering, deltaTime is important!
        if (usedFuel < fuelTank && Time.time - timeFromSpawn > 1 && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 pos = touch.position;

            if (Input.touchCount > 1)
            {
                Touch touch2 = Input.GetTouch(1);
                Vector2 pos2 = touch2.position;

                //boost if can use, min time not orbiting
                if ((pos.x > Screen.width / 2 && pos2.x < Screen.width / 2) || (pos2.x > Screen.width / 2 && pos.x < Screen.width / 2))
                {
                    if (boostPassedTime > 7 && usedFuel + 40 <= fuelTank && atractedTo == -1 && Time.time - timeFromLastPlanet > 2)
                    {
                        {
                            if (rightTut)
                            {
                                boostTut = true;
                                targetForceSteer += new Vector3(0, 50, 0);
                                fromLastBoost = Time.time;
                                scoreBoost = UnityEngine.Random.Range(0, 101);
                                boostAddText.text = "+" + scoreBoost.ToString();
                                boostAdd.SetActive(true);
                                addScore += scoreBoost;
                                scoreBoost = 0;
                                usedFuel += 40;
                            }
                        }
                    }
                }
            }
            else if (pos.x > Screen.width / 2)
            {
                if (leftTut)
                {
                    rightTut = true;
                    targetForceSteer += new Vector3(90 * Time.deltaTime + steerAddition, 0, 0);
                    shell.transform.Rotate(new Vector3(0, -1, 0) * 90 * Time.deltaTime);
                }
            }
            else if (pos.x < Screen.width / 2)
            {
                //dla tutoriala
                if (Time.time - timeFromSpawn > 2)
                {
                    leftTut = true;
                }

                targetForceSteer += new Vector3(-90 * Time.deltaTime - steerAddition, 0, 0);
                shell.transform.Rotate(new Vector3(0, 1, 0) * 90 * Time.deltaTime);
            }
        }

        //boost add text
        if (boostAdd.activeSelf && Time.time - fromLastBoost > 4)
        {
            boostAdd.SetActive(false);
        }

        //coming to orbit
        if (lastAtracttedTo != atractedTo && lastAtracttedTo == -1)
        {
            orbitTime = Time.time;
            Vector3 direction = planets[atractedTo].transform.position - transform.position;
            if (Vector3.Angle(rb.velocity, direction) < slowdownAngle)
            {
                slowdownPlanet = true;
            }
            else
            {
                slowdownPlanet = false;
            }
        }

        //out of orbit, boost
        if (lastAtracttedTo != atractedTo && lastAtracttedTo != -1)
        {
            fuelEffect.SetActive(false);

            if (Time.time - orbitTime > 5f)
            {
                //optimalization, it counts to fuel use!
                targetForceSteer += new Vector3(0, 20, 0);
            }
            orbitTime = 0;
        }

        //slowdown
        //if planet
        if (atractedTo != -1 && slowdownPlanet && fuelTank - usedFuel > 0 && !exiting)
        {
            Vector3 direction = planets[atractedTo].transform.position - transform.position;
            //this angle constant
            if (Vector3.Angle(rb.velocity, direction) < 40f)
            {
                slowdownCameraAngle = 23;
                Time.timeScale = 0.4f;
                steerAddition = 220 * Time.deltaTime;
            }
            else
            {
                slowdownPlanet = false;
            }
        }
        //if no slowdown
        else
        {
            slowdownCameraAngle = -1;
            Time.timeScale = desiredTimeScale;
            steerAddition = 0;
        }


        //gravity
        if (atractedTo != -1)
        {
            bool tank = planets[atractedTo].GetComponent<TutPlanet>().fuel;
            if (tank && usedFuel > 0 && !exiting)
            {
                fuelEffect.SetActive(true);
                engine.SetActive(true);
                usedFuel -= Time.deltaTime * 10;
            }
            else if (usedFuel <= 0)
            {
                fuelEffect.SetActive(false);
            }

            var tempColor = boostImage.color;
            tempColor.a = 0.25f;
            boostImage.color = tempColor;

            planetForce = planets[atractedTo].transform.position - transform.position;
            float strengthOfAttraction = planets[atractedTo].transform.localScale.x * 100 * Time.deltaTime;
            if (strengthOfAttraction > 40)
            {
                strengthOfAttraction = 40;
            }
            targetForce = planetForce * strengthOfAttraction / Vector3.Distance(transform.position, planets[atractedTo].transform.position);
            timeFromLastPlanet = Time.time;
        }
        //out of orbit
        else
        {
            targetForce = Vector3.zero;
        }

        //no fuel for boost
        if (fuelTank - usedFuel < 40)
        {
            var tempColor = boostImage.color;
            tempColor.a = 0.25f;
            boostImage.color = tempColor;
        }
        else
        {
            if (atractedTo == -1 && Time.time - timeFromLastPlanet > 2)
            {
                var tempColor = boostImage.color;
                tempColor.a = 0.78f;
                boostImage.color = tempColor;
            }
        }

        //speed and force softening
        if (targetForceSteer.magnitude > 0.0001f)
        {
            targetForceSteer -= targetForceSteer * Time.deltaTime * 1.5f;
        }
        if (rb.velocity.magnitude > 21 && usedFuel < fuelTank)
        {
            rb.velocity -= rb.velocity * Time.deltaTime * 0.3f;
        }
        else if (rb.velocity.magnitude < 17 && usedFuel < fuelTank && !exiting)
        {
            targetForceSteer += new Vector3(0, 1, 0) * 70 * Time.deltaTime;
        }

        //force
        rb.AddForce(targetForce);
        rb.AddRelativeForce(targetForceSteer);

        //no fuel
        if (usedFuel > fuelTank)
        {
            engine.SetActive(false);
            rb.velocity -= rb.velocity * Time.deltaTime * 0.4f;
            if (rb.velocity.magnitude < 7 && atractedTo == -1)
            {
                endSequence();
            }
        }

    }

    //player rotation
    private float steer(float x1, float y1)
    {
        if (x1 == 0 || y1 == 0)
        {
            return 0f;
        }
        double sin = y1 / Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2));
        double alfa = (Math.Asin(sin) * 180) / Math.PI;
        return (float)alfa;
    }

    //collision
    void OnCollisionEnter(Collision collision)
    {
        endSequence();
    }

    void endSequence()
    {
        if (!endSeguenceInvoked)
        {
            if (boostTut)
            {
                PlayerPrefs.SetInt("onlyPTut", 1);
            }

            endSeguenceInvoked = true;
            crashSound.Play();
            exiting = true;
            rb.isKinematic = true;

            if (!spawn.GetComponent<ParticleSystem>().isPlaying)
            {
                spawn.GetComponent<ParticleSystem>().Play();
            }
            StartCoroutine(ReloadingCoroutine());
        }
    }

    IEnumerator ExitingCoroutine()
    {
        desiredScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator StartingCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        exiting = false;
        yield break;
    }

    public int getAttractedTo()
    {
        return atractedTo;
    }

    public float getScore()
    {
        return score;
    }

    private void QuitButton()
    {
        if (!endSeguenceInvoked)
        {
            endSeguenceInvoked = true;
            crashSound.Play();
            exiting = true;
            rb.isKinematic = true;

            if (!spawn.GetComponent<ParticleSystem>().isPlaying)
            {
                spawn.GetComponent<ParticleSystem>().Play();
            }
            StartCoroutine(ExitingCoroutine());
        }
    }

    IEnumerator ReloadingCoroutine()
    {
        desiredScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}