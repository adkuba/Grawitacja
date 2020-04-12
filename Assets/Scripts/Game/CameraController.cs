using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject planetPrefab;
    public GameObject asteroidPrefab;
    public GameObject sunPrefab;

    private Vector2 screenBounds;
    private float timeFromLastMovement;
    private GameObject player;
    private Rigidbody playerRigidbody;
    private GameObject objectsGenerator;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        objectsGenerator = GameObject.FindGameObjectWithTag("ObjectsGenerator");
        playerRigidbody = player.GetComponent<Rigidbody>();
        timeFromLastMovement = Time.time;
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
    }


    void Update()
    {
        //camera size in player controller!
        transform.position = player.transform.position + new Vector3(0, 0, -90);

        float score = player.GetComponent<PlayerController>().getScore();
        if (score > 2000)
        {
            score = 2000;
        }

        if (Input.touchCount > 0)
        {
            timeFromLastMovement = Time.time;
        }
        //not clicking + difficulty scaling
        float timeObs = score * -0.00225f + 5;
        if (Time.time - timeFromLastMovement > timeObs && playerRigidbody.velocity.magnitude > 1 && player.GetComponent<PlayerController>().getAttractedTo() == -1)
        {
            Vector3 speed = playerRigidbody.velocity;
            speed.Normalize();
            speed *= screenBounds.magnitude + 10;
            speed += player.transform.position;
            float offset = 1;
            objectsGenerator.GetComponent<ObjectsGenerator>().generateAsteroid(speed.x - offset, speed.x + offset, speed.y - offset, speed.y + offset);
        }
    }
}
