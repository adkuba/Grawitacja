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
    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        //wylicza szerokość i wysokość kamery
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        
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
        int index = Random.Range(0, materials.Length);
        //losowy kolor
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
        widthBounds = new Vector2(Camera.main.transform.position.x - screenBounds.x * 7f, Camera.main.transform.position.x + screenBounds.x * 7f);
        heightBounds = new Vector2(Camera.main.transform.position.y - screenBounds.y * 9f, Camera.main.transform.position.y + screenBounds.y * 9f);

        if (transform.position.x < widthBounds.x || transform.position.x > widthBounds.y || transform.position.y < heightBounds.x || transform.position.y > heightBounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}