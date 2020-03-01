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
        //wielkosc i kolor sparkles
        sparks = transform.GetChild(0).GetComponent<ParticleSystem>();
        float sSize = transform.localScale.x * 0.1f - 0.2f;
        sparks.transform.localScale = new Vector3(sSize, sSize, sSize); //odpowiedni rozmiar
        int index = Random.Range(0, materials.Length);
        sparks.GetComponent<ParticleSystemRenderer>().trailMaterial = materials[index]; //losowy kolor
        //wielkosc ksiezyca od 0.2 do 0.4
        float moonSize = Random.Range(0.2f, 0.4f);
        moon = transform.GetChild(2).gameObject;
        moon.transform.localScale = new Vector3(moonSize, moonSize, moonSize);
    }

    // Update is called once per frame
    void Update()
    {
        widthBounds = new Vector2(Camera.main.transform.position.x - screenBounds.x * 3f, Camera.main.transform.position.x + screenBounds.x * 3f);
        heightBounds = new Vector2(Camera.main.transform.position.y - screenBounds.y * 4f, Camera.main.transform.position.y + screenBounds.y * 4f);
        //*2 zeby bylo opoznienie, 1.5 bo szerokosc jest wieksza niz 

        if (transform.position.x < widthBounds.x || transform.position.x > widthBounds.y || transform.position.y < heightBounds.x || transform.position.y > heightBounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}