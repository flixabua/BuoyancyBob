using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using TMPro;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class MenuManager : MonoBehaviour
{
    public ParticleSystem bubbles;
    public Animator transition;
    public float transitiontime = 1f;

    private SerialPort serial = new SerialPort("COM11", 9600);
    private float lastData = 0f;
    private float currentAir = 0f;

    private Rigidbody _rigidbody;
    //sound data
    private bool changed = false;
    public AudioSource inhale_sound;
    public AudioClip[] inhale_sounds;
    public AudioSource exhale_sound;
    public AudioClip[] exhale_sounds;
    private Random rng = new Random();
    public TMP_Text Score;

    // Start is called before the first frame update
    void Start()
    {

        _rigidbody = GetComponent<Rigidbody>();
        if (Score != null)
        {
            Score.SetText("{0} m", ScoreController.control.getScore());
        }
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
            if ((lastData <= 0 && data > 0) || (lastData > 0 && data <= 0))
                changed = true;
            lastData = data;
        }
        catch (TimeoutException)
        {
            data = lastData;
        }

        currentAir += data;
        
        
        if (changed)
        {
            
            if (data > 0)
            {
                //breathing in
                bubbles.enableEmission = false;
                inhale_sound.clip = inhale_sounds[rng.Next(inhale_sounds.Length)];
                inhale_sound.Play();
            }
            else
            {
                //breathing out
                bubbles.enableEmission = true;
                exhale_sound.clip = exhale_sounds[rng.Next(exhale_sounds.Length)];
                exhale_sound.Play();
            }
            changed = false;
        }
        
        MoveObject(data);

        if (transform.position.y > 20)
        {
            //exit game
            Debug.Log("Closing game");
            QuitGame();
        }

        if (transform.position.y < -25)
        {
            //start level
            LoadNextLevel();
        }
        
    }

    public void LoadNextLevel()
    {
        serial.Close();
        StartCoroutine(LoadLevel(1));
    }
    public void QuitGame()
    {
        serial.Close();
        StartCoroutine(LoadLevel(99));
    }

    IEnumerator LoadLevel(int levelindex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitiontime);
        if (levelindex == 99)
            Application.Quit();
        SceneManager.LoadScene(levelindex);
    }
    
    void MoveObject(float value)
    {
        transform.position += 30*Vector3.up*value;
    }
    

}
