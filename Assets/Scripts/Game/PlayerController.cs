using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PlayerController : Agent
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
    private int addScore = 0;
    private bool hsAnimationComp = false;
    private float waitAnimation = 0;
    private bool cockpitUp = false;
    private bool textUp = false;
    private bool textDown = false;
    private bool cockpitDown = false;
    private float timeFromLastPlanet;
    private bool fuelAnim = false;
    private bool hsAnimStarted = false;
    private int scoreBoost = 0;
    private float steerAddition = 0;
    private int closestAsteroid = -1;
    private bool slowdownPlanet = false;
    private float slowdownAngle = 0;
    private float desiredTimeScale;
    private float slowdownCameraAngle = -1;
    private float boostPassedTime = 0;
    private int lastScore = 0;
    private float aiOrbitTime = 0;

    private GameObject[] planets;
    private GameObject[] asteroids;
    private GameObject scoreGameObject;
    private GameObject fuelGameObject;
    private GameObject boostGameObject;
    private GameObject spawn;
    private GameObject boostAdd;
    private GameObject fuelWarning;
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

    void Start()
    {
        scoreGameObject = GameObject.FindGameObjectWithTag("Score");
        shell = GameObject.FindGameObjectWithTag("Shell");
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        cockpitImage = GameObject.FindGameObjectWithTag("Cockpit");
        fuelGameObject = GameObject.FindGameObjectWithTag("Fuel");
        boostGameObject = GameObject.FindGameObjectWithTag("Boost");
        fuelWarning = GameObject.FindGameObjectWithTag("FuelWarning");
        engine = GameObject.FindGameObjectWithTag("Engine");
        fuelEffect = GameObject.FindGameObjectWithTag("FuelEffect");
        boostAdd = GameObject.FindGameObjectWithTag("BoostAdd");
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

        //zmiana jezyka
        if (Application.systemLanguage == SystemLanguage.Polish)
        {
            fuelWarning.GetComponent<UnityEngine.UI.Text>().text = "Orbituj!";
        }
         
        fuelEffect.SetActive(false);
        highscore = PlayerPrefs.GetInt("highscore", 0);
        addScore = PlayerPrefs.GetInt("addScore", 0);
        score = addScore;
        fromLastBoost = Time.time;
        timeFromSpawn = Time.time;
        boostAdd.SetActive(false);
        fuelWarning.SetActive(false);
        timeFromLastPlanet = Time.time;
    }

    public void Steering(float[] vectorAction)
    {
        //steering, deltaTime is important!
        if (usedFuel < fuelTank && Time.time - timeFromSpawn > 1)
        {
            var action = Mathf.FloorToInt(vectorAction[0]);

            if (action == 1)
            {
                if (boostPassedTime > 7 && usedFuel + 40 <= fuelTank && atractedTo == -1 && Time.time - timeFromLastPlanet > 2)
                {
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
            else if (action == 2)
            {
                targetForceSteer += new Vector3(800 * Time.deltaTime + steerAddition, 0, 0);
                shell.transform.Rotate(new Vector3(0, -1, 0) * 180 * Time.deltaTime);
            }
            else if (action == 3)
            {
                targetForceSteer += new Vector3(-800 * Time.deltaTime - steerAddition, 0, 0);
                shell.transform.Rotate(new Vector3(0, 1, 0) * 180 * Time.deltaTime);
            }
        }
    }

    //przed rozpoczeciem iteracji
    public override void OnEpisodeBegin()
    {
        //Reset srodowisko
        transform.position = Vector3.zero;
        score = 0;
        lastScore = 0;
        addScore = 0;
        atractedTo = -1;
        lastAtracttedTo = -1;
        rb.isKinematic = false;
        fuelEffect.SetActive(false);
        fromLastBoost = Time.time;
        timeFromSpawn = Time.time;
        boostAdd.SetActive(false);
        fuelWarning.SetActive(false);
        timeFromLastPlanet = Time.time;
        usedFuel = 0;

        foreach(GameObject planet in GameObject.FindGameObjectsWithTag("Planet"))
        {
            GameObject.Destroy(planet);
        }
        foreach(GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            GameObject.Destroy(asteroid);
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        //pozycja playera - 2 floaty 
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);

        //player predkosc 2 floaty
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

        Vector3[] planetsPositions = { Vector3.zero, Vector3.zero };
        float[] planetsSize = { 0f, 0f };

        PlanetsCompare planetsCompare = new PlanetsCompare();
        if (planets != null)
        {
            GameObject[] planetsCopy = new GameObject[planets.Length];
            planets.CopyTo(planetsCopy, 0);
            Array.Sort(planetsCopy, planetsCompare);

            for (int i=0; i<planetsCopy.Length; i++)
            {
                planetsPositions[i] = planetsCopy[i].transform.position;
                planetsSize[i] = planetsCopy[i].transform.localScale.x;
                if (i == 1)
                {
                    break;
                }
            }
        }

        //pozycja i wielkosc 2 najblizszych planet 2*3=6 floatow
        sensor.AddObservation(planetsPositions[0].x);
        sensor.AddObservation(planetsPositions[0].y);
        sensor.AddObservation(planetsSize[0]);

        sensor.AddObservation(planetsPositions[1].x);
        sensor.AddObservation(planetsPositions[1].y);
        sensor.AddObservation(planetsSize[1]);



        Vector3[] asteroidsPositions = { Vector3.zero, Vector3.zero };

        if (asteroids != null)
        {
            GameObject[] asteroidsCopy = new GameObject[asteroids.Length];
            asteroids.CopyTo(asteroidsCopy, 0);
            Array.Sort(asteroidsCopy, planetsCompare);

            for (int i=0; i<asteroidsCopy.Length; i++)
            {
                asteroidsPositions[i] = asteroidsCopy[i].transform.position;
                if (i == 1)
                {
                    break;
                }
            }
        }

        //pozycja 2 najblizszych asteroid 2*2=4 floatow
        sensor.AddObservation(asteroidsPositions[0].x);
        sensor.AddObservation(asteroidsPositions[0].y);

        sensor.AddObservation(asteroidsPositions[1].x);
        sensor.AddObservation(asteroidsPositions[1].y);

        //poziom paliwa
        sensor.AddObservation(usedFuel);

        //wynik
        sensor.AddObservation(score);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Steering(vectorAction);
        int currentScore = Convert.ToInt32(score);
        if (lastScore != currentScore)
        {
            AddReward((currentScore - lastScore) * 0.01f);
            lastScore = currentScore;
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        // MOJE STEROWANIE, test srodowiska
        if (Input.GetKey("left"))
        {
            actionsOut[0] = 3;

        } else if (Input.GetKey("right"))
        {
            actionsOut[0] = 2;

        } else if (Input.GetKey("space"))
        {
            actionsOut[0] = 1;

        } else
        {
            actionsOut[0] = 0;
        }
    }

    void Update()
    {
        //player size
        Vector3 scale = transform.localScale;
        if (Vector3.Distance(scale, desiredScale) > 0.1 )
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
        if (Time.time - timeFromLastPlanet > 10 && !exiting)
        {
            usedFuel += Time.deltaTime * 5;
            fuelWarning.SetActive(true);

            //animation checking
            if (!hsAnimStarted)
            {
                fuelAnim = true;
                if (fuelImage.rectTransform.sizeDelta.x < 70)
                {
                    fuelImage.rectTransform.sizeDelta += new Vector2(10, 10) * Time.deltaTime;
                }
                if (cockpitImageRect.anchoredPosition.y < -10)
                {
                    cockpitImageRect.anchoredPosition += new Vector2(0, 10 * Time.deltaTime);
                }
            }
        }
        else if (fuelImage.rectTransform.sizeDelta.x > 50 && !hsAnimStarted)
        {
            fuelImage.rectTransform.sizeDelta -= new Vector2(10, 10) * Time.deltaTime;
        }
        else if (cockpitImageRect.anchoredPosition.y > -25 && !hsAnimStarted)
        {
            cockpitImageRect.anchoredPosition -= new Vector2(0, 10 * Time.deltaTime);
        }
        else
        {
            fuelAnim = false;
        }

        //warning remove
        if (Time.time - timeFromLastPlanet < 10 || exiting)
        {
            fuelWarning.SetActive(false);
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
        //slowdown angle is reducing with score
        slowdownAngle = score * -0.0214f + 34.29f;
        if (slowdownAngle > 30)
        {
            slowdownAngle = 30;
        }
        if (slowdownAngle < 0)
        {
            slowdownAngle = 0;
        }
        //timescale up with score
        desiredTimeScale = score * 0.000429f + 0.914f;
        if (desiredTimeScale < 1)
        {
            desiredTimeScale = 1;
        }
        if (desiredTimeScale > 1.6f)
        {
            desiredTimeScale = 1.6f;
        }
        boostPassedTime = Time.time - fromLastBoost;
        if (boostPassedTime < 7)
        {
            boostImage.fillAmount = boostPassedTime / 7;
        }
        else
        {
            boostImage.fillAmount = 1;
        }

        //highscore animation
        if (score > highscore && !hsAnimationComp && !fuelAnim)
        {
            hsAnimStarted = true;
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

            if (!textUp && cockpitUp)
            {
                if (scoreText.fontSize < 50)
                {
                    scoreText.fontSize += 1;
                    waitAnimation = Time.time;

                } else
                {
                    textUp = true;
                }
            }

            if (!textDown && cockpitUp && textUp && Time.time - waitAnimation > 1)
            {
                if (scoreText.fontSize > 45)
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

            if (cockpitUp && textUp && textDown && cockpitDown)
            {
                hsAnimationComp = true;
                hsAnimStarted = false;
            }
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
                aiOrbitTime = Time.time;
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


        asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        //moons as asteroids
        asteroids = asteroids.Concat(GameObject.FindGameObjectsWithTag("Moon")).ToArray();
        if (closestAsteroid == -1 && atractedTo == -1)
        {
            foreach (GameObject asteroid in asteroids)
            {
                if (asteroid != null)
                {
                    if (Vector3.Distance(transform.position, asteroid.transform.position) < 20)
                    {
                        Vector3 direction = asteroid.transform.position - transform.position;
                        //if small angle
                        if (Vector3.Angle(rb.velocity, direction) < slowdownAngle)
                        {
                            closestAsteroid = Array.IndexOf(asteroids, asteroid);
                            break;
                        }
                    }
                }
            }
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
        //if asteroid
        else if (closestAsteroid != -1 && fuelTank - usedFuel > 0 && !exiting)
        {
            if (closestAsteroid < asteroids.Length)
            {
                Vector3 direction = asteroids[closestAsteroid].transform.position - transform.position;
                if (Vector3.Angle(rb.velocity, direction) < 25f)
                {
                    slowdownCameraAngle = 23;
                    Time.timeScale = 0.5f;
                    steerAddition = 80 * Time.deltaTime;
                }
                else
                {
                    closestAsteroid = -1;
                }
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
            bool tank = planets[atractedTo].GetComponent<Planet>().fuel;
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

            //jesli tylko orbituje przez 30 sekund
            if (Time.time - aiOrbitTime > 30f)
            {
                EndEpisode();
            }
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
                endReload();
            }
        }
    }

    //player rotation
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

    //collision
    void OnCollisionEnter(Collision collision)
    {
        endReload();
    }

    void endReload()
    {
        //exiting = true;
        rb.isKinematic = true;

        if (!spawn.GetComponent<ParticleSystem>().isPlaying)
        {
            spawn.GetComponent<ParticleSystem>().Play();
        }
        //rozbicie lub brak paliwa
        AddReward(-1f);
        EndEpisode();
    }

    IEnumerator ReloadingCoroutine()
    {
        desiredScale = new Vector3(0.0028f, 0.0094f, 0.0016f);
        yield return new WaitForSeconds(1.5f);
        //rozbiecie lub brak paliwa
        AddReward(-1f);
        EndEpisode();
        transform.position = Vector3.zero;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}