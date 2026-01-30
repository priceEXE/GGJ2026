using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class BGAudio : MonoBehaviour
{
    public List<AudioClip> bgs;
    
    private AudioSource au;

    private void Start()
    {
        au = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (au.isPlaying) return;
        
        au.clip = bgs[Random.Range(0, bgs.Count)];
        au.Play();
    }
}
