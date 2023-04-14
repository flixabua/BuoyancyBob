using System;
using UnityEngine;
using System.IO.Ports;
using System.Numerics;
using TMPro;
using Vector3 = UnityEngine.Vector3;

public class InputManager : MonoBehaviour
{
    private float ControllerValue;
    public int AmountToMove;
    public GameObject air;
    public GameObject depth;
    private Rigidbody _rigidbody;
    private float lastData = 0f;
    private TMP_Text AirText;
    private TMP_Text DepthText;
    private TMP_Text JacketText;
    
    // important for force calculation
    private float currentAir = 0f;
    private float currentDepth = 10f;
    private float lastDepth = 10f;
    private float currentJacketAir = 4f;
    private float additionalVolume = 10f;
    private float DiverVolume = 65f;
    private float Mass = 80f;
    private float force = 0f;
    
    // Game relevant data
    public float AirInTank = 2400f;
    private float currentPressure;

    private SerialPort serial = new SerialPort("COM11", 9600);
    // Start is called before the first frame update
    void Start()
    {
        currentPressure = 1f + (currentDepth / 10f);
        _rigidbody = GetComponent<Rigidbody>();
        AirText = air.GetComponent<TMP_Text>();
        DepthText = depth.GetComponent<TMP_Text>();
        AirText.SetText("Current air in Lung: {0}", currentAir);
        DepthText.SetText("Current Depth: {0}", currentDepth);
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
            data = float.Parse(serial.ReadLine()) / 1000000;
            lastData = data;
        }
        catch (TimeoutException)
        {
            data = lastData;
        }
    
        currentAir += data;
        
        CalculateVolume();
        currentDepth = -_rigidbody.position.y + 10;
        AirText.SetText("Current air in Lung/Jacket/tank: {0}/{1}/{2}", currentAir, currentJacketAir, AirInTank);
        CalculateForce();
        DepthText.SetText("Current Depth: {0} force: {1}", currentDepth, force);
        
        // Debug.Log(data);
        // MoveObject(data);
        ConsumeAir(data);
        MoveObjectbyForce(force);


    }

    void MoveObject(float value)
    {
        currentDepth = -_rigidbody.position.y + 10;
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

    void MoveObjectbyForce(float force)
    {
        Vector3 appiedForce = new Vector3(0f, force, 0f);
        
        _rigidbody.AddForce(appiedForce);
    }

    void CalculateForce()
    {
        float fAuf = (currentAir + currentJacketAir + additionalVolume + DiverVolume);
        float fAb = Mass;
        force = (fAuf - fAb);
    }

    void CalculateVolume()
    {
        float new_pressure = 1f + (currentDepth / 10f);
        float old_pressure = 1f + (lastDepth / 10f);
        currentPressure = new_pressure;
        currentAir = (old_pressure * currentAir) / new_pressure;
        currentJacketAir = (old_pressure * currentJacketAir) / new_pressure;
        additionalVolume = (old_pressure * additionalVolume) / new_pressure;
        lastDepth = currentDepth;

    }

    void ConsumeAir(float data)
    {
        if (data > 0)
        {
            AirInTank -= (data / currentPressure);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Ouput the Collision to the console
        Debug.Log("Collision : " + collision.gameObject.name);
    }
    
}
