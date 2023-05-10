using System;
using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System.Numerics;
using TMPro;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;
using Random = System.Random;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private float ControllerValue;
    public int AmountToMove;
    private Rigidbody2D _rigidbody;
    private float lastData = 0f;
    /*
    private TMP_Text AirText;
    private TMP_Text DepthText;
    private TMP_Text JacketText;
    */
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
    private float AirStart;
    private float currentPressure;
    public ParticleSystem bubbles;
    
    //sound data
    private bool changed = false;
    public AudioSource inhale_sound;
    public AudioClip[] inhale_sounds;
    public AudioSource exhale_sound;
    public AudioClip[] exhale_sounds;
    private Random rng = new Random();

    private SerialPort serial = new SerialPort("COM11", 9600);
    
    //gameplay relevant data
    public TMP_Text Score;
    public Slider oxygenMeter;
    private int score = 0;
    private float timer;
    public Animator transition;
    public float transitiontime = 1f;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        ScoreController.control.resetScore();
        currentPressure = 1f + (currentDepth / 10f);
        _rigidbody = GetComponent<Rigidbody2D>();
        AirStart = AirInTank;
        Score.SetText("{0} m", score);
        /*
        AirText = air.GetComponent<TMP_Text>();
        DepthText = depth.GetComponent<TMP_Text>();
        AirText.SetText("Current air in Lung: {0}", currentAir);
        DepthText.SetText("Current Depth: {0}", currentDepth);
        */
    }

    // Update is called once per frame
    void Update()
    {   
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            score += 1;
            Score.SetText("{0} m", score);
            timer = 0f;
            ScoreController.control.addScore(1);
        }
        
        if (!serial.IsOpen)
        {
            serial.Open();
            serial.ReadTimeout = 10;
        }
    
        float data;
        try
        {
            data = float.Parse(serial.ReadLine()) / 1000000;
            if ((lastData <= 0 && data > 0) || (lastData > 0 && data <= 0))
                changed = true;
            lastData = data;
        }
        catch (TimeoutException)
        {
            data = lastData;
        }
        
        currentAir += data;
        
        // CalculateVolume();
        currentDepth = -_rigidbody.position.y + 10;
        // AirText.SetText("Current air in Lung/Jacket/tank: {0}/{1}/{2}", currentAir, currentJacketAir, AirInTank);
        // CalculateForce();
        // DepthText.SetText("Current Depth: {0} force: {1}", currentDepth, force);
        
 
        if (changed)
        {
            
            if (data > 0)
            {
                //breathing in
                bubbles.enableEmission = false;
                inhale_sound.clip = inhale_sounds[rng.Next(inhale_sounds.Length)];
                inhale_sound.Play();
                Debug.Log("inhaling");
            }
            else
            {
                //breathing out
                bubbles.enableEmission = true;
                exhale_sound.clip = exhale_sounds[rng.Next(exhale_sounds.Length)];
                exhale_sound.Play();
                Debug.Log("exhaling");
            }
            changed = false;
        }
        // Debug.Log(data);
        MoveObject(data);
        ConsumeAir(data);
        // MoveObjectbyForce(force);

    }

    void MoveObject(float value)
    {
        currentDepth = -_rigidbody.position.y + 10;
        value *= 300;
        if (value > 0)
        {
            value = value/12f;
            // _rigidbody.velocity = new Vector3(0.0f, value, 0.0f) * AmountToMove;
            transform.position += Vector3.up*value;
        }else if (value < 0)
        {
            value = value/10f;
            // _rigidbody.velocity = new Vector3(0.0f, value, 0.0f) * AmountToMove;
            transform.position += Vector3.up*value;
        }

        if (transform.position.y < -30)
            transform.position = new Vector3(transform.position.x, -30f, -90f);
        
        if (transform.position.y > 30)
            transform.position = new Vector3(transform.position.x, 30f, -90f);
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
            data *= 50;
            AirInTank -= data;
            // AirInTank -= (data / currentPressure);
            updateOxygenMeter();
            if (AirInTank < 50)
            {
                LoadNextLevel();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Ouput the Collision to the console
        Debug.Log("Collision : " + collision.gameObject.name);
        ConsumeAir(5);
        
    }

    void updateOxygenMeter()
    {
        oxygenMeter.value = AirInTank/AirStart;
    }
    
    public void LoadNextLevel()
    {
        
        StartCoroutine(LoadLevel(2));
    }

    IEnumerator LoadLevel(int levelindex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitiontime);
        if (levelindex == 99)
            Application.Quit();
        
        serial.Close();
        SceneManager.LoadScene(levelindex);
    }
    
}
