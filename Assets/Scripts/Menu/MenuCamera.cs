using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    private GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 0, -90);
    }

}
