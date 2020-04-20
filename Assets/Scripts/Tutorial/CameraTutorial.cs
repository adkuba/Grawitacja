using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTutorial : MonoBehaviour
{
    private Vector2 screenBounds;
    private GameObject player;
    private Rigidbody playerRigidbody;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
    }


    void Update()
    {
        //camera size in player controller!
        transform.position = player.transform.position + new Vector3(0, 0, -90);

        float score = player.GetComponent<PlayerTutorial>().getScore();
        if (score > 2000)
        {
            score = 2000;
        }
    }
}
