using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("right"))
        {
            transform.Rotate(new Vector3(0, -1, 0) * 70 * Time.deltaTime);
        }
        if (Input.GetKey("left"))
        {
            transform.Rotate(new Vector3(0, 1, 0) * 70 * Time.deltaTime);
        }
    }
}
