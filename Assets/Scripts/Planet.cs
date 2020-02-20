using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planet : MonoBehaviour
{
    private Vector2 screenBounds;
    private Vector2 widthBounds;
    private Vector2 heightBounds;

    // Start is called before the first frame update
    void Start()
    {
        //wylicza szerokość i wysokość kamery
        screenBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
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

    //kolizja
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}