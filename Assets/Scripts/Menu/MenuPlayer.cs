using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class MenuPlayer : MonoBehaviour
{
    private float score = 0;

    void Update()
    {
        Vector3 deviceAcc = Input.acceleration;
        deviceAcc.Normalize();
        deviceAcc = new Vector3(deviceAcc.x, deviceAcc.y);
        transform.position += deviceAcc * 20 * Time.deltaTime;
        score = Vector3.Distance(Vector3.zero, transform.position) * 0.4f;
    }

    public float getScore()
    {
        return score;
    }
}