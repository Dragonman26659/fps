using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggermusic : MonoBehaviour
{
    public AudioSource MusicTrackToPlay;
    public AudioSource MusicTrackToStop;


    void OnTriggerEnter()
    {
        MusicTrackToPlay.Play();
        MusicTrackToStop.Stop();
    }
}
