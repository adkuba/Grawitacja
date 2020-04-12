using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Math;
using System;


public class ObjectsGenerator : MonoBehaviour
{
    private int maxPlanets;
    private int maxSuns;
    private float sunRespawn;
    private float planetRespawn;
    private int maxAsteroids;
    private float asteroidsRespawn;
    private Vector2 screenBounds;

    public GameObject planetPrefab;
    public GameObject asteroidPrefab;
    public GameObject sunPrefab;

    private GameObject[] planets;
    private GameObject[] asteroids;
    private GameObject[] suns;
    private GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);

        //important values
        maxPlanets = 6;
        maxSuns = 1;
        sunRespawn = 10;
        planetRespawn = 1;
        maxAsteroids = 5;
        asteroidsRespawn = 5;

        StartCoroutine(planetWave());
        StartCoroutine(asteroidWave());
        StartCoroutine(sunWave());
    }

    void Update()
    {
        transform.position = player.transform.position;
        planets = GameObject.FindGameObjectsWithTag("Planet");
        asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        suns = GameObject.FindGameObjectsWithTag("Sun");
    }

    IEnumerator sunWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(sunRespawn);
            spawnSuns();
        }
    }

    IEnumerator planetWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(planetRespawn);
            spawnPlanets();
        }
    }

    IEnumerator asteroidWave()
    {
        while (true)
        {
            //difficulty scaling
            float score = 0;
            if (player.GetComponent<PlayerController>() == null)
            {
                score = player.GetComponent<MenuPlayer>().getScore();
            }
            else
            {
                score = player.GetComponent<PlayerController>().getScore();
            }
            if (score > 2000)
            {
                score = 2000;
            }
            asteroidsRespawn = score * -0.002f + 5;
            maxAsteroids = Convert.ToInt32(score * 0.0175f + 5);
            yield return new WaitForSeconds(asteroidsRespawn);
            spawnAsteroids();
        }
    }

    private void spawnSuns()
    {
        Vector4 gora = new Vector4(transform.position.x - screenBounds.x * 6, transform.position.x + screenBounds.x * 6, transform.position.y + screenBounds.y * 4f, transform.position.y + screenBounds.y * 8);
        Vector4 dol = new Vector4(transform.position.x - screenBounds.x * 6, transform.position.x + screenBounds.x * 6, transform.position.y - screenBounds.y * 4f, transform.position.y - screenBounds.y * 8);
        Vector4 lewo = new Vector4(transform.position.x - screenBounds.x * 6, transform.position.x - screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        Vector4 prawo = new Vector4(transform.position.x + screenBounds.x * 3f, transform.position.x + screenBounds.x * 6, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        int startLen = suns.Length;
        for (int i = 0; i < maxSuns - startLen; i++)
        {
            float cwiatrka = UnityEngine.Random.value;
            //up
            if (cwiatrka <= .25f)
            {
                generateSun(gora.x, gora.y, gora.z, gora.w);
            }
            //down
            else if (cwiatrka <= .5f)
            {
                generateSun(dol.x, dol.y, dol.z, dol.w);
            }
            //left
            else if (cwiatrka <= .75f)
            {
                generateSun(lewo.x, lewo.y, lewo.z, lewo.w);
            }
            //right
            else
            {
                generateSun(prawo.x, prawo.y, prawo.z, prawo.w);
            }
        }
    }

    private void spawnAsteroids()
    {
        Vector4 gora = new Vector4(transform.position.x - screenBounds.x * 3f, transform.position.x + screenBounds.x * 3f, transform.position.y + screenBounds.y * 1.6f, transform.position.y + screenBounds.y * 4);
        Vector4 dol = new Vector4(transform.position.x - screenBounds.x * 3f, transform.position.x + screenBounds.x * 3f, transform.position.y - screenBounds.y * 1.6f, transform.position.y - screenBounds.y * 4);
        Vector4 lewo = new Vector4(transform.position.x - screenBounds.x * 3f, transform.position.x - screenBounds.x * 1.6f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        Vector4 prawo = new Vector4(transform.position.x + screenBounds.x * 1.6f, transform.position.x + screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        int startLen = asteroids.Length;

        for (int i = 0; i < maxAsteroids - startLen; i++)
        {
            float cwiatrka = UnityEngine.Random.value;
            //up
            if (cwiatrka <= .25f)
            {
                generateAsteroid(gora.x, gora.y, gora.z, gora.w);
            }
            //down
            else if (cwiatrka <= .5f)
            {
                generateAsteroid(dol.x, dol.y, dol.z, dol.w);
            }
            //left
            else if (cwiatrka <= .75f)
            {
                generateAsteroid(lewo.x, lewo.y, lewo.z, lewo.w);
            }
            //right
            else
            {
                generateAsteroid(prawo.x, prawo.y, prawo.z, prawo.w);
            }
        }
    }

    private void spawnPlanets()
    {
        Vector4 gora = new Vector4(transform.position.x - screenBounds.x * 4, transform.position.x + screenBounds.x * 4, transform.position.y + screenBounds.y * 3, transform.position.y + screenBounds.y * 6);
        Vector4 dol = new Vector4(transform.position.x - screenBounds.x * 4, transform.position.x + screenBounds.x * 4, transform.position.y - screenBounds.y * 3, transform.position.y - screenBounds.y * 6);
        Vector4 lewo = new Vector4(transform.position.x - screenBounds.x * 4, transform.position.x - screenBounds.x * 2, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        Vector4 prawo = new Vector4(transform.position.x + screenBounds.x * 2, transform.position.x + screenBounds.x * 4, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        int startLen = planets.Length;

        for (int i = 0; i < maxPlanets - startLen; i++)
        {
            float cwiatrka = UnityEngine.Random.value;

            if (cwiatrka <= .25f)
            {
                generatePlanet(gora.x, gora.y, gora.z, gora.w);
            }

            else if (cwiatrka <= .5f)
            {
                generatePlanet(dol.x, dol.y, dol.z, dol.w);
            }

            else if (cwiatrka <= .75f)
            {
                generatePlanet(lewo.x, lewo.y, lewo.z, lewo.w);
            }

            else
            {
                generatePlanet(prawo.x, prawo.y, prawo.z, prawo.w);
            }
        }
    }


    private void generatePlanet(float minx, float maxx, float miny, float maxy)
    {
        if (checkObjectsNumber(planets, maxPlanets))
        {
            GameObject planet = Instantiate(planetPrefab) as GameObject;

            float rand = UnityEngine.Random.value;
            float size = 0;
            bool find = false;
            if (rand <= .8f && !find)
            {
                size = UnityEngine.Random.Range(12f, 14f);
                find = true;
            }
            if (rand <= .95f && !find)
            {
                size = UnityEngine.Random.Range(14f, 16f);
                find = true;
            }
            if (!find)
            {
                size = UnityEngine.Random.Range(16f, 19f);
            }
            planet.transform.localScale = new Vector3(size, size, size);

            Vector2 xrange = new Vector2(minx, maxx);
            Vector2 yrange = new Vector2(miny, maxy);
            //80 between planet - planet, 40 betwen planet - asteroid
            planet.transform.position = generateObjectPosition(xrange, yrange, planet.transform.localScale.x, planets.Concat(asteroids).ToArray(), 80, true, 40);

            //creating went wrong
            if (planet.transform.position == Vector3.zero)
            {
                Destroy(planet);
            }

            planets = GameObject.FindGameObjectsWithTag("Planet");
        }
    }

    public void generateAsteroid(float minx, float maxx, float miny, float maxy)
    {
        if (checkObjectsNumber(asteroids, maxAsteroids))
        {
            GameObject asteroid = Instantiate(asteroidPrefab) as GameObject;

            Vector2 xrange = new Vector2(minx, maxx);
            Vector2 yrange = new Vector2(miny, maxy);
            //40 between asteroid - planet, 20 between asteroid - asteroid
            asteroid.transform.position = generateObjectPosition(xrange, yrange, asteroid.transform.localScale.x, asteroids.Concat(planets).ToArray(), 40, true, 20);

            if (asteroid.transform.position == Vector3.zero)
            {
                Destroy(asteroid);
            }

            asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        }
    }


    private void generateSun(float minx, float maxx, float miny, float maxy)
    {
        if (checkObjectsNumber(suns, maxSuns))
        {
            GameObject sun = Instantiate(sunPrefab) as GameObject;

            Vector2 xrange = new Vector2(minx, maxx);
            Vector2 yrange = new Vector2(miny, maxy);
            Vector3 position = generateObjectPosition(xrange, yrange, sun.transform.localScale.x, suns, 1000);
            sun.transform.position = position + new Vector3(0, 0, 15);

            if (sun.transform.position == Vector3.zero)
            {
                Destroy(sun);
            }

            suns = GameObject.FindGameObjectsWithTag("Sun");
        }
    }


    private bool checkObjectsNumber(GameObject[] objectsTable, int maxObjects)
    {
        if (objectsTable.Length < maxObjects)
        {
            return true;
        }
        return false;
    }


    private Vector2 generateObjectPosition(Vector2 xrange, Vector2 yrange, float size, GameObject[] objectsTable, int minDistance, bool asteroidAndPlanet = false, int asteroidMinDistance = 40)
    {
        int steps = 0;
        Vector2 position = Vector2.zero;

        while (steps < 10)
        {
            bool foundBad = false;
            position = new Vector2(UnityEngine.Random.Range(xrange.x, xrange.y), UnityEngine.Random.Range(yrange.x, yrange.y));
            steps++;
            if (!asteroidAndPlanet)
            {
                foreach (GameObject gObject in objectsTable)
                {
                    if (gObject != null)
                    {
                        if (Vector2.Distance(new Vector2(gObject.transform.position.x, gObject.transform.position.y), position) - size - gObject.transform.localScale.x <= minDistance)
                        {
                            foundBad = true;
                            break;
                        }
                    }
                }
            }
            //special for asteroid and planet
            else
            {
                foreach (GameObject gObject in objectsTable)
                {
                    if (gObject != null)
                    {
                        if (gObject.GetComponent<Planet>() != null)
                        {
                            if (Vector2.Distance(new Vector2(gObject.transform.position.x, gObject.transform.position.y), position) - size - gObject.transform.localScale.x <= minDistance)
                            {
                                foundBad = true;
                                break;
                            }
                        }
                        //asteroid in array
                        else
                        {
                            if (Vector2.Distance(new Vector2(gObject.transform.position.x, gObject.transform.position.y), position) - size - gObject.transform.localScale.x <= asteroidMinDistance)
                            {
                                foundBad = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!foundBad)
            {
                break;
            }
        }

        //max 10 attempts
        if (steps == 10)
        {
            position = Vector2.zero;
        }

        return position;
    }
}
