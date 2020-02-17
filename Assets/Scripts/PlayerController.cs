﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private Vector3 targetForce = Vector3.zero;
    private Vector3 targetForceSteer = Vector3.zero;
    private Vector3 planetForce = Vector3.zero;
    private GameObject attractedTo;
    private float strengthOfAttraction = 5F;
    public Rigidbody rb;

    // Start is called before the first frame update
    //UWAGA NA RAZIE ZAKOMENTOWALEM RZECZY ZWIAZANE Z PLANETAMI BO MUSZE ZROBIC GENERATOR PLANET
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       // attractedTo = GameObject.FindGameObjectWithTag("Planet");
        rb.velocity = new Vector3(0, 15, 0); //predkosc poczatkowa w gore
    }

    // Update is called once per frame
    void Update()
    {
        //rb.velocity = new Vector3(15, 15, 0); //predkosc poczatkowa w gore
        //obrot obiektu
        float coordinate = steer(rb.velocity.x, rb.velocity.y);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        if (rb.velocity.x > 0 && rb.velocity.y > 0)
        {
            rotation = Quaternion.Euler(0, 0, coordinate - 90f);
        } 
        else if (rb.velocity.x < 0 && rb.velocity.y > 0)
        {
            rotation = Quaternion.Euler(0, 0, 90f - coordinate);
        }
        else if (rb.velocity.x < 0 && rb.velocity.y < 0)
        {
            rotation = Quaternion.Euler(0, 0, -coordinate + 90f);
        }
        else if (rb.velocity.x > 0 && rb.velocity.y < 0)
        {
            rotation = Quaternion.Euler(0, 0, coordinate - 90f);
        }
            transform.rotation = rotation;


        //ograniczenie max predkosci
        //mniejsze ograniczenie na skrecanie
        if(targetForceSteer.x < 5 && targetForceSteer.y < 2 && targetForceSteer.x > -5 && targetForceSteer.y > -2)
        {
            if (Input.GetKey("right"))
            {
                targetForceSteer += new Vector3(5, 0, 0);
            }

            if (Input.GetKey("left"))
            {
                targetForceSteer += new Vector3(-5, 0, 0);
            }

            if (Input.GetKey("up"))
            {
                targetForceSteer += new Vector3(0, 1, 0); //DODAC OGRANICZENIE NA ILOSC PALIWA!
            }
        }

        /*
        //dodajemy sile od planety
        float dist = Vector3.Distance(transform.position, attractedTo.transform.position);
        if (dist < 8F)
        {
            planetForce = attractedTo.transform.position - transform.position;
            targetForce = planetForce * strengthOfAttraction / dist; //  SILA przyciagania jest wieksza wraz ze zmniejszeniem sie dystansu

        } else //jak jestesmy poza dzialaniem planety to wyciszamy sile planety
        {
            if (Vector3.Distance(targetForce, Vector3.zero) > 0)
            {
                targetForce -= targetForce * 0.02F;
            }
        }
        */

        rb.AddForce(targetForce); //stosujemy sile
        rb.AddRelativeForce(targetForceSteer); //SILA RELATYWNY KIERUNEK, teraz nie musze obliczac tych katow i wgl

        //wyciszamy ogolna predkosc i sile ze sterowania zawsze!
        if (Vector3.Distance(targetForceSteer, Vector3.zero) > 0)
        {
            targetForceSteer -= targetForceSteer * 0.02F;
        }
        if (Vector3.Distance(rb.velocity, Vector3.zero) > 0)
        {
            rb.velocity -= rb.velocity * 0.001F;
        }


    }

    private float steer(float x1, float y1) //kat, TO MUSI BYC Z VELOCITY WYLICZANE
    {
        if(x1 == 0 || y1 == 0)
        {
            return 0f;
        }
        double sin = y1 / Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2));
        double alfa = (Math.Asin(sin) * 180) / Math.PI;
        return (float)alfa;
    }

}
