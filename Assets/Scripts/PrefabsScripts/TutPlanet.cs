using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutPlanet : MonoBehaviour
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
        fuel = true;

        //sparkles size and colour
        sparks = transform.GetChild(0).GetComponent<ParticleSystem>();
        float sSize = transform.localScale.x * 0.14f - 0.2f;
        sparks.transform.localScale = new Vector3(sSize, sSize, sSize);

        score = player.GetComponent<PlayerTutorial>().getScore();
        
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

        Destroy(transform.GetChild(2).gameObject);
        
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