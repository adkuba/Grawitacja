using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    private Vector2 screenBounds;
    private Vector2 widthBounds;
    private Vector2 heightBounds;
    private Vector3 lastPlayerPosition;
    private Vector3 playerPosition;
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 myPosition = transform.position;
        lastPlayerPosition = playerPosition;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 difference = lastPlayerPosition - playerPosition;
        difference.z = 0;
        transform.position = myPosition - difference * 0.5f;

        widthBounds = new Vector2(Camera.main.transform.position.x - screenBounds.x * 6f, Camera.main.transform.position.x + screenBounds.x * 6f);
        heightBounds = new Vector2(Camera.main.transform.position.y - screenBounds.y * 8f, Camera.main.transform.position.y + screenBounds.y * 8f);
        //*2 zeby bylo opoznienie, 1.5 bo szerokosc jest wieksza niz 

        if (transform.position.x < widthBounds.x || transform.position.x > widthBounds.y || transform.position.y < heightBounds.x || transform.position.y > heightBounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}
