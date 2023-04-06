using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 0.04f;

    public int edgeLeft = -115;
    public int edgeRight = 18;
    public int edgeBottom = -30;
    public int edgeTop = 34;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < edgeLeft)
        {
            Vector3 newPos = new Vector3(edgeRight, Random.Range(edgeBottom, edgeTop), transform.position.z);
            transform.position = newPos;
        }

        transform.Translate(Vector3.left * speed);
    }
}
