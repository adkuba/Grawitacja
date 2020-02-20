using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLightController : MonoBehaviour
{
    private Light s_light;

    // Start is called before the first frame update
    void Start()
    {
        s_light = GetComponent<Light>();
        s_light.range = transform.parent.localScale.x * 8.6f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
