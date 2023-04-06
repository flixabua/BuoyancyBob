using System;
using UnityEngine;
using System.IO.Ports;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class InputManager : MonoBehaviour
{
    private float ControllerValue;
    public int AmountToMove;
    private Rigidbody _rigidbody;
    private float lastData = 0f;

    private SerialPort serial = new SerialPort("COM11", 9600);
    // Start is called before the first frame update
    void Start()
    {
        
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!serial.IsOpen)
        {
            serial.Open();
            serial.ReadTimeout = 10;
        }

        float data;
        try
        {
            data = float.Parse(serial.ReadLine()) / 1000;
            lastData = data;
        }
        catch (TimeoutException)
        {
            data = lastData;
        }
        Debug.Log(data);
        MoveObject(data);
        
        
    }

    void MoveObject(float value)
    {
        Debug.Log(value);
        if (value > 0)
        {
            value = value/12f;
            _rigidbody.velocity = new Vector3(0.0f, value, 0.0f) * AmountToMove; 
            // transform.position += Vector3.up*value;
        }else if (value < 0)
        {
            value = value/10f;
            _rigidbody.velocity = new Vector3(0.0f, value, 0.0f) * AmountToMove; 
            // transform.position += Vector3.up*value;
        }
    }
}
