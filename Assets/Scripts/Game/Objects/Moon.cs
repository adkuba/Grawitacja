using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    private Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 2) == 0) //wartosci 0 albo 1
        {
            rotation = new Vector3(0, 0, 1);
        }
        else
        {
            rotation = new Vector3(0, 0, -1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.parent.position, rotation, 30 * Time.deltaTime);
    }
}
