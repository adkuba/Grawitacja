using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planetPrefab;
    private Vector2 screenBounds;
    private GameObject[] planets;
    public int maxPlanets = 5;
    public float respawn = 3f;

    // Start is called before the first frame update
    void Start()
    {
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        planets = GameObject.FindGameObjectsWithTag("Planet");
        StartCoroutine(planetWave());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0, -10);
        planets = GameObject.FindGameObjectsWithTag("Planet"); //wszystkie planety
    }

    private void spawnPlanets()
    {
        for(int i=0; i<2; i++) //gora i dol
        {
            //jest 1.2f zeby nie tworzyl sie na widoku
            if (checkPlanetsNumber())
            {
                GameObject a = Instantiate(planetPrefab) as GameObject;
                a.transform.localScale = generatePlanetSize();
                a.transform.position = generatePlanetPosition(transform.position.x - screenBounds.x * 1.5f, transform.position.x + screenBounds.x * 1.5f, transform.position.y + screenBounds.y * 1.2f, transform.position.y + screenBounds.y * 2, a.transform.localScale.x);
                if (a.transform.position == Vector3.zero)
                {
                    Destroy(a);
                }
                planets = GameObject.FindGameObjectsWithTag("Planet"); //POTENCJALNY ZAMULACZ, ale tak jest najprosciej bo jakby planeta moze powstac szybciej niz zostanie odswiezona tablica w Update() i pozniej pojawia sie za duzo planet i w zlej odleglosci
            }
            if (checkPlanetsNumber())
            {
                GameObject b = Instantiate(planetPrefab) as GameObject;
                b.transform.localScale = generatePlanetSize();
                b.transform.position = generatePlanetPosition(transform.position.x - screenBounds.x * 1.5f, transform.position.x + screenBounds.x * 1.5f, transform.position.y - screenBounds.y * 1.2f, transform.position.y - screenBounds.y * 2, b.transform.localScale.x);
                if (b.transform.position == Vector3.zero)
                {
                    Destroy(b);
                }
                planets = GameObject.FindGameObjectsWithTag("Planet");
            }
        }
        for (int i=0; i<1; i++) //prawo lewo
        {
            //jest 1.2f zeby nie tworzyl sie na widoku
            if (checkPlanetsNumber())
            {
                GameObject a = Instantiate(planetPrefab) as GameObject;
                a.transform.localScale = generatePlanetSize();
                a.transform.position = generatePlanetPosition(transform.position.x - screenBounds.x * 1.5f, transform.position.x - screenBounds.x * 1.2f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y, a.transform.localScale.x);
                if (a.transform.position == Vector3.zero)
                {
                    Destroy(a);
                }
                planets = GameObject.FindGameObjectsWithTag("Planet");
            }
            if (checkPlanetsNumber())
            {
                GameObject b = Instantiate(planetPrefab) as GameObject;
                b.transform.localScale = generatePlanetSize();
                b.transform.position = generatePlanetPosition(transform.position.x + screenBounds.x * 1.2f, transform.position.x + screenBounds.x * 1.5f, transform.position.y - screenBounds.y, transform.position.y + screenBounds.y, b.transform.localScale.x);
                if (b.transform.position == Vector3.zero)
                {
                    Destroy(b);
                }
                planets = GameObject.FindGameObjectsWithTag("Planet");
            }
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

    private bool checkPlanetsNumber()
    {
        if (planets.Length < maxPlanets)
        {
            return true;
        }
        return false;
    }

    private bool checkPlanetDistance(Vector2 planetToCheck, float size)
    {
        foreach (GameObject planet in planets)
        {
            //14 to odleglosc taka ze dwie planety musza byc oddalone od siebie o 7 (o jedna planete)
            if (Vector2.Distance(new Vector2(planet.transform.position.x, planet.transform.position.y), planetToCheck) - size - planet.transform.localScale.x  <= 21f)
            {
                return true; //zwraca true zeby do while sie kontynuowal bo trzeba jeszcze raz wygenerowac wspolrzedne
            }
        }
        return false; //false zeby wyjsc z while jesli jest ok
    }

    private Vector2 generatePlanetPosition(float minx, float maxx, float miny, float maxy, float size)
    {
        int steps = 0;
        Vector2 position;
        do
        {
            position = new Vector2(Random.Range(minx, maxx), Random.Range(miny, maxy));
            steps++;
        } while (checkPlanetDistance(position, size) && steps < 10); //max 10 prob znalezienia pozycji

        if (steps == 10) //jesli nie udalo sie znalezc, tbh moze byc sytuacja ze uda sie znalzc akurat w 10 kroku ale jebac
        {
            position = Vector2.zero;
        }

        return position;
    }

    //generowanie wielkosci z roznym prawdopodobienstwem
    private Vector3 generatePlanetSize()
    {
        float rand = Random.value;
        float size;
        if (rand <= .8f)
        {
            size = Random.Range(6f, 8f);
            return new Vector3(size, size, size);
        }
        if (rand <= .95f)
        {
            size = Random.Range(8f, 10f);
            return new Vector3(size, size, size);
        }
        size = Random.Range(10f, 15f);
        return new Vector3(size, size, size);
    }
}
