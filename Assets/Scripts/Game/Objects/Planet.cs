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


    void Start()
    {
        //camera height and width
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        player = GameObject.FindGameObjectWithTag("Player");

        //fuel
        float rand = Random.value;
        fuel = false;
        if (rand > .66f)
        {
            fuel = true;
        }
        
        //additional effects
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

        //sparkles size and colour
        sparks = transform.GetChild(0).GetComponent<ParticleSystem>();
        float sSize = transform.localScale.x * 0.14f - 0.2f;
        sparks.transform.localScale = new Vector3(sSize, sSize, sSize);
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

        //moon size
        float moonSize = Random.Range(0.2f, 0.4f);
        moon = transform.GetChild(2).gameObject;
        moon.transform.localScale = new Vector3(moonSize, moonSize, moonSize);

        if (!moonK)
        {
            Destroy(transform.GetChild(2).gameObject);
        }
        if (!sparksK)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }


    void Update()
    {
        //deleting
        widthBounds = new Vector2(Camera.main.transform.position.x - screenBounds.x * 5, Camera.main.transform.position.x + screenBounds.x * 5);
        heightBounds = new Vector2(Camera.main.transform.position.y - screenBounds.y * 7, Camera.main.transform.position.y + screenBounds.y * 7);

        if (transform.position.x < widthBounds.x || transform.position.x > widthBounds.y || transform.position.y < heightBounds.x || transform.position.y > heightBounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}