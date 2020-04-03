using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    private Vector3 rotation;


    void Start()
    {
        //values 0 or 1!
        if (Random.Range(0, 2) == 0)
        {
            rotation = new Vector3(0, 0, 1);
        }
        else
        {
            rotation = new Vector3(0, 0, -1);
        }
    }


    void Update()
    {
        transform.RotateAround(transform.parent.position, rotation, 30 * Time.deltaTime);
    }
}
