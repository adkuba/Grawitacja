using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private Vector2 screenBounds;
    private Vector2 widthBounds;
    private Vector2 heightBounds;
    public bool fuel;
    private ParticleSystem sparks;
    private GameObject moon;
    private GameObject player;
    public Material[] materials;
    private float score;

    // Start is called before the first frame update
    void Start()
    {
        //wylicza szerokość i wysokość kamery
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        player = GameObject.FindGameObjectWithTag("Player");

        //czy mam paliwo?
        float rand = Random.value;
        fuel = false;
        if (rand > .66f)
        {
            fuel = true;
        }
        
        //musze teraz okreslic co bedzie dodatkowo przy planecie
        rand = Random.value;
        bool sparksK = false;
        bool moonK = false;
        bool find = false;
        if (rand <= .4f && !find)
        {
            moonK = true;
            find = true;
        }
        if (rand <= .8f && !find)
        {
            sparksK = true;
            find = true;
        }
        if (rand <= .9 && !find)
        {
            sparksK = true;
            moonK = true;
        }
        //wielkosc i kolor sparkles
        //musi byc get child zeby znalazlo obiekt pod odpowiednia planeta
        sparks = transform.GetChild(0).GetComponent<ParticleSystem>();
        float sSize = transform.localScale.x * 0.14f - 0.2f;
        //odpowiedni rozmiar
        sparks.transform.localScale = new Vector3(sSize, sSize, sSize);

        //kolor zalezny od odleglosci
        score = player.GetComponent<PlayerController>().getScore();
        int index = 0;
        if (score < 400)
        {
            index = 0;
        }
        else if (score < 800)
        {
            index = 1;
        }
        else if (score < 1200)
        {
            index = 2;
        }
        else if (score < 1600)
        {
            index = 3;

        }
        else
        {
            index = 4;
        }
        sparks.GetComponent<ParticleSystemRenderer>().trailMaterial = materials[index];
        //wielkosc ksiezyca od 0.2 do 0.4
        float moonSize = Random.Range(0.2f, 0.4f);
        moon = transform.GetChild(2).gameObject;
        moon.transform.localScale = new Vector3(moonSize, moonSize, moonSize);
        //usuwanie
        if (!moonK)
        {
            Destroy(transform.GetChild(2).gameObject);
        }
        if (!sparksK)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //usuwa sie poza generatorem
        widthBounds = new Vector2(Camera.main.transform.position.x - screenBounds.x * 5, Camera.main.transform.position.x + screenBounds.x * 5);
        heightBounds = new Vector2(Camera.main.transform.position.y - screenBounds.y * 7, Camera.main.transform.position.y + screenBounds.y * 7);

        if (transform.position.x < widthBounds.x || transform.position.x > widthBounds.y || transform.position.y < heightBounds.x || transform.position.y > heightBounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}