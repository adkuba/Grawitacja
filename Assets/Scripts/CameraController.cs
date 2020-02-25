using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*kontroler kamery
 *generator planet i asteroidow
 */
public class CameraController : MonoBehaviour
{
    public GameObject planetPrefab;
    public GameObject asteroidPrefab;
    public GameObject sunPrefab;
    private Vector2 screenBounds;
    private GameObject[] planets;
    private GameObject[] asteroids;
    private GameObject[] suns;
    public int maxPlanets = 5;
    public int maxSuns = 4;
    public float sunRespawn = 10f;
    public float respawn = 3f;
    public int maxAsteroids = 4;
    public float asteroidsRespawn = 10f;

    // Start is called before the first frame update
    void Start()
    {
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        planets = GameObject.FindGameObjectsWithTag("Planet"); //musze miec bo na starcie mam zawsze jedna planete, usunac ja pozniej
        StartCoroutine(planetWave());
        StartCoroutine(asteroidWave());
        StartCoroutine(sunWave());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0, -90);
        float playerSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity.magnitude;
        Camera.main.orthographicSize = playerSpeed * 0.28f + 16f;
        planets = GameObject.FindGameObjectsWithTag("Planet"); //wszystkie planety
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
            yield return new WaitForSeconds(respawn);
            spawnPlanets();
        }
    }

    IEnumerator asteroidWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidsRespawn);
            spawnAsteroids();
        }
    }

    private void spawnSuns()
    {
        for (int i = 0; i < 2; i++)
        {
            //gora
            generateSun(transform.position.x - screenBounds.x * 6f, transform.position.x + screenBounds.x * 6f, transform.position.y + screenBounds.y * 3f, transform.position.y + screenBounds.y * 8);
            //dol
            generateSun(transform.position.x - screenBounds.x * 6f, transform.position.x + screenBounds.x * 6f, transform.position.y - screenBounds.y * 3f, transform.position.y - screenBounds.y * 8);
        }
        for (int i = 0; i < 1; i++)
        {
            //lewo
            generateSun(transform.position.x - screenBounds.x * 6f, transform.position.x - screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
            //prawo
            generateSun(transform.position.x + screenBounds.x * 3f, transform.position.x + screenBounds.x * 6f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        }
    }

    private void spawnAsteroids()
    {
        for (int i = 0; i < 2; i++)
        {
            //gora
            generateAsteroid(transform.position.x - screenBounds.x * 3f, transform.position.x + screenBounds.x * 3f, transform.position.y + screenBounds.y * 1.6f, transform.position.y + screenBounds.y * 4);
            //dol
            generateAsteroid(transform.position.x - screenBounds.x * 3f, transform.position.x + screenBounds.x * 3f, transform.position.y - screenBounds.y * 1.6f, transform.position.y - screenBounds.y * 4);
        }
        for (int i = 0; i < 1; i++)
        {
            //lewo
            generateAsteroid(transform.position.x - screenBounds.x * 3f, transform.position.x - screenBounds.x * 1.6f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
            //prawo
            generateAsteroid(transform.position.x + screenBounds.x * 1.6f, transform.position.x + screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        }
    }

    private void spawnPlanets()
    {
        //jest 1.6f zeby nie tworzyl sie na widoku
        for (int i = 0; i < 2; i++)
        {
            //generujemy na gorze
            generatePlanet(transform.position.x - screenBounds.x * 3f, transform.position.x + screenBounds.x * 3f, transform.position.y + screenBounds.y * 1.6f, transform.position.y + screenBounds.y * 4);
            //generujemy na dole
            generatePlanet(transform.position.x - screenBounds.x * 3f, transform.position.x + screenBounds.x * 3f, transform.position.y - screenBounds.y * 1.6f, transform.position.y - screenBounds.y * 4);
        }
        for (int i = 0; i < 1; i++)
        {
            //generujemy na lewo
            generatePlanet(transform.position.x - screenBounds.x * 3f, transform.position.x - screenBounds.x * 1.6f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
            //generujemy na prawo
            generatePlanet(transform.position.x + screenBounds.x * 1.6f, transform.position.x + screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        }
    }

    //minx maxx to pozycja gdzie ma powstac planeta
    private void generatePlanet(float minx, float maxx, float miny, float maxy)
    {
        if (checkObjectsNumber(planets, maxPlanets))
        {
            //nowy obiekt
            GameObject planet = Instantiate(planetPrefab) as GameObject;

            //generowanie rozmiaru
            float rand = Random.value;
            float size = 0;
            bool find = false;
            if (rand <= .8f && !find)
            {
                size = Random.Range(12f, 14f);
                find = true;
            }
            if (rand <= .95f && !find)
            {
                size = Random.Range(14f, 16f);
                find = true;
            }
            if (!find)
            {
                size = Random.Range(16f, 19f);
            }
            planet.transform.localScale = new Vector3(size, size, size);

            //generowanie pozycji
            Vector2 xrange = new Vector2(minx, maxx);
            Vector2 yrange = new Vector2(miny, maxy);
            planet.transform.position = generateObjectPosition(xrange, yrange, planet.transform.localScale.x, planets, 57);

            //jesli nie udalo sie wygenerowac pozycji wczesniej to niszczymy obiekt
            if (planet.transform.position == Vector3.zero)
            {
                Destroy(planet);
            }

            //POTENCJALNY ZAMULACZ, ale tak jest najprosciej bo jakby planeta moze powstac szybciej niz zostanie odswiezona tablica w Update() i pozniej pojawia sie za duzo planet i w zlej odleglosci
            planets = GameObject.FindGameObjectsWithTag("Planet");
        }
    }

    private void generateAsteroid(float minx, float maxx, float miny, float maxy)
    {
        if (checkObjectsNumber(asteroids, maxAsteroids))
        {
            //nowy obiekt
            GameObject asteroid = Instantiate(asteroidPrefab) as GameObject;

            //rozmiar na razie staly
            //generowanie pozycji
            Vector2 xrange = new Vector2(minx, maxx);
            Vector2 yrange = new Vector2(miny, maxy);
            Vector3 position = generateObjectPosition(xrange, yrange, asteroid.transform.localScale.x, asteroids, 30);
            asteroid.transform.position = position + new Vector3(0, 0, -70);

            //jesli nie udalo sie wygenerowac pozycji wczesniej to niszczymy obiekt
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
            //nowy obiekt
            GameObject sun = Instantiate(sunPrefab) as GameObject;

            //rozmiar na razie staly
            //generowanie pozycji
            Vector2 xrange = new Vector2(minx, maxx);
            Vector2 yrange = new Vector2(miny, maxy);
            Vector3 position = generateObjectPosition(xrange, yrange, sun.transform.localScale.x, suns, 100);
            sun.transform.position = position + new Vector3(0, 0, 15);

            //jesli nie udalo sie wygenerowac pozycji wczesniej to niszczymy obiekt
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

    private Vector2 generateObjectPosition(Vector2 xrange, Vector2 yrange, float size, GameObject[] objectsTable, int minDistance)
    {
        int steps = 0;
        Vector2 position = Vector2.zero;

        while(steps < 10)
        {
            bool foundBad = false;
            //minx maxx, miny maxy
            position = new Vector2(Random.Range(xrange.x, xrange.y), Random.Range(yrange.x, yrange.y));
            steps++;
            foreach (GameObject gObject in objectsTable)
            {
                if (Vector2.Distance(new Vector2(gObject.transform.position.x, gObject.transform.position.y), position) - size - gObject.transform.localScale.x <= minDistance)
                {
                    foundBad = true;
                    break;
                }
            }
            //znalezlismy dobra pozycje
            if (!foundBad)
            {
                break;
            }
        }

        //jesli nie udalo sie znalezc, tbh moze byc sytuacja ze uda sie znalzc akurat w 10 kroku ale jebac
        if (steps == 10)
        {
            position = Vector2.zero;
        }

        return position;
    }
}
