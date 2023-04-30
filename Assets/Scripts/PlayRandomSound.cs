using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PlayRandomSound : MonoBehaviour
{

    public AudioSource _as;

    public AudioClip[] audioClipArray;

    private void Awake()
    {
        _as = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

}
