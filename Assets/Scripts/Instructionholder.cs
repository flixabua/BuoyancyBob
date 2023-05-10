using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructionholder : MonoBehaviour
{
    public float speed = 1f;
    public float range = 3f;

    private float startPos;
    private bool down = true;

    // Update is called once per frame
    private void Start()
    {
        startPos = transform.position.y;
    }

    void Update()
    {
        if (transform.position.y < startPos - range)
        {
            down = false;
        }
        if (transform.position.y > startPos + range)
        {
            down = true;
        }
        
        if (down)
            MoveObject(-speed);
        else
            MoveObject(speed);
        
    }
    
    void MoveObject(float value)
    {
        transform.position += 30*Vector3.up*value;
    }
}
