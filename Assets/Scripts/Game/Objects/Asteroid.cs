using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 screenBounds;
    private Vector2 widthBounds;
    private Vector2 heightBounds;
    private Vector3 lastPlayerPosition;
    private Vector3 playerPosition;
    private GameObject playerGO;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerPosition = playerGO.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)), 45 * Time.deltaTime);
        Vector3 myPosition = transform.position;
        lastPlayerPosition = playerPosition;
        playerPosition = playerGO.transform.position;
        Vector3 difference = lastPlayerPosition - playerPosition;
        difference.z = 0;
        transform.position = myPosition + difference * 0.3f;

        //wartosci dobrane tak aby obiekt byl usuwany jak wyjdzie poza generator
        widthBounds = new Vector2(Camera.main.transform.position.x - screenBounds.x * 4, Camera.main.transform.position.x + screenBounds.x * 4);
        heightBounds = new Vector2(Camera.main.transform.position.y - screenBounds.y * 5, Camera.main.transform.position.y + screenBounds.y * 5); 

        if (transform.position.x < widthBounds.x || transform.position.x > widthBounds.y || transform.position.y < heightBounds.x || transform.position.y > heightBounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}
