using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Math;

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
    public int maxPlanets = 9;
    public int maxSuns = 4;
    public float sunRespawn = 10f;
    public float respawn = 3f;
    public int maxAsteroids = 10;
    public float asteroidsRespawn = 5f;
    private float timeFromLastMovement;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        timeFromLastMovement = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        planets = GameObject.FindGameObjectsWithTag("Planet"); //musze miec bo na starcie mam zawsze jedna planete, usunac ja pozniej
        StartCoroutine(planetWave());
        StartCoroutine(asteroidWave());
        StartCoroutine(sunWave());
    }

    // Update is called once per frame
    void Update()
    {
        //wielkosc kamery w player controller
        transform.position = player.transform.position + new Vector3(0, 0, -90);
        planets = GameObject.FindGameObjectsWithTag("Planet"); //wszystkie planety
        asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        suns = GameObject.FindGameObjectsWithTag("Sun");

        if (Input.GetKey("right") || Input.GetKey("left")) //jesli kliknelismy
        {
            timeFromLastMovement = Time.time;
        }
        if (Time.time - timeFromLastMovement > 8) //jeli nic nie kliknelismy od 8s
        {
            //to probujemy wygenerowac asteroide przed ryjem
            //minx maxx miny maxy
            Vector4 spawn = new Vector4(player.transform.position.x, player.transform.position.x, player.transform.position.y, player.transform.position.y);
            float vx = player.GetComponent<Rigidbody>().velocity.x;
            float vy = player.GetComponent<Rigidbody>().velocity.y;
            float vc = player.GetComponent<Rigidbody>().velocity.magnitude;
            float c = screenBounds.magnitude + 10;

            float cos = Abs(vx) / vc;
            float sin = Abs(vy) / vc;

            if (vy > 0)
            {
                if (vx > 0) //pierwsza cw
                {
                    //okno 30x30
                    spawn += new Vector4(c * cos + 5, c * cos + 35, sin * c + 5, sin * c + 35);
                } else //druga
                {
                    //y dodatnie, x ujemne
                    spawn += new Vector4( (-c) * cos - 5, (-c) * cos - 35, sin * c + 5, sin * c + 35);
                }
            } else
            {
                if (vx > 0) //czwarta
                {
                    //y ujemne x dodatnie
                    spawn += new Vector4( c * cos + 5, c * cos + 35, sin * (-c) - 5, sin * (-c) - 35);

                } else //trzecia
                {
                    //y ujmne x ujemne
                    spawn += new Vector4( (-c) * cos - 5, (-c) * cos - 35, sin * (-c) - 5, sin * (-c) - 35);
                }
            }
            generateAsteroid(spawn.x, spawn.y, spawn.z, spawn.w);
        }
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
        Vector4 gora = new Vector4(transform.position.x - screenBounds.x * 7f, transform.position.x + screenBounds.x * 7f, transform.position.y + screenBounds.y * 4f, transform.position.y + screenBounds.y * 9);
        Vector4 dol = new Vector4(transform.position.x - screenBounds.x * 7f, transform.position.x + screenBounds.x * 7f, transform.position.y - screenBounds.y * 4f, transform.position.y - screenBounds.y * 9);
        Vector4 lewo = new Vector4(transform.position.x - screenBounds.x * 7f, transform.position.x - screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        Vector4 prawo = new Vector4(transform.position.x + screenBounds.x * 3f, transform.position.x + screenBounds.x * 7f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        int startLen = suns.Length;
        for (int i = 0; i < maxSuns - startLen; i++) //probuje stworzyc slonca tyle razy ile moge jeszcze utworzyc slonc
        {
            //losowo wybieram polowke w ktorej bede tworzyl obiekt
            float cwiatrka = Random.value;
            if (cwiatrka <= .25f) //na gorze
            {
                generateSun(gora.x, gora.y, gora.z, gora.w);

            }
            else if (cwiatrka <= .5f) //na dole
            {
                generateSun(dol.x, dol.y, dol.z, dol.w);

            }
            else if (cwiatrka <= .75f) // na lewo
            {
                generateSun(lewo.x, lewo.y, lewo.z, lewo.w);

            }
            else //na prawo
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
        for (int i = 0; i < maxAsteroids - startLen; i++) //probuje stworzyc asteroidy tyle razy ile moge jeszcze je utworzyc
        {
            //losowo wybieram polowke w ktorej bede tworzyl obiekt
            float cwiatrka = Random.value;
            if (cwiatrka <= .25f) //na gorze
            {
                generateAsteroid(gora.x, gora.y, gora.z, gora.w);

            }
            else if (cwiatrka <= .5f) //na dole
            {
                generateAsteroid(dol.x, dol.y, dol.z, dol.w);

            }
            else if (cwiatrka <= .75f) // na lewo
            {
                generateAsteroid(lewo.x, lewo.y, lewo.z, lewo.w);

            }
            else //na prawo
            {
                generateAsteroid(prawo.x, prawo.y, prawo.z, prawo.w);
            }
        }
    }

    private void spawnPlanets()
    {
        //jest 1.6f zeby nie tworzyl sie na widoku
        Vector4 gora = new Vector4(transform.position.x - screenBounds.x * 7f, transform.position.x + screenBounds.x * 7f, transform.position.y + screenBounds.y * 4f, transform.position.y + screenBounds.y * 9);
        Vector4 dol = new Vector4(transform.position.x - screenBounds.x * 7f, transform.position.x + screenBounds.x * 7f, transform.position.y - screenBounds.y * 4f, transform.position.y - screenBounds.y * 9);
        Vector4 lewo = new Vector4(transform.position.x - screenBounds.x * 7f, transform.position.x - screenBounds.x * 3f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        Vector4 prawo = new Vector4(transform.position.x + screenBounds.x * 3f, transform.position.x + screenBounds.x * 7f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y);
        int startLen = planets.Length;
        for (int i = 0; i < maxPlanets - startLen; i++) //probuje stworzyc planety tyle razy ile moge jeszcze utworzyc planety
        {
            //losowo wybieram polowke w ktorej bede tworzyl obiekt
            float cwiatrka = Random.value;
            if (cwiatrka <= .25f) //na gorze
            {
                generatePlanet(gora.x, gora.y, gora.z, gora.w);

            } else if (cwiatrka <= .5f) //na dole
            {
                generatePlanet(dol.x, dol.y, dol.z, dol.w);

            } else if (cwiatrka <= .75f) // na lewo
            {
                generatePlanet(lewo.x, lewo.y, lewo.z, lewo.w);

            } else //na prawo
            {
                generatePlanet(prawo.x, prawo.y, prawo.z, prawo.w);
            }
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
            planet.transform.position = generateObjectPosition(xrange, yrange, planet.transform.localScale.x, planets, 90);

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
            Vector3 position = generateObjectPosition(xrange, yrange, asteroid.transform.localScale.x, asteroids.Concat(planets).ToArray(), 30);
           
            //jesli nie udalo sie wygenerowac pozycji wczesniej to niszczymy obiekt
            if (asteroid.transform.position == Vector3.zero)
            {
                Destroy(asteroid);
            }

            asteroid.transform.position = position; //+ new Vector3(0, 0, -70);
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
                if (gObject != null)
                {
                    if (Vector2.Distance(new Vector2(gObject.transform.position.x, gObject.transform.position.y), position) - size - gObject.transform.localScale.x <= minDistance)
                    {
                        foundBad = true;
                        break;
                    }
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
