using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip normalState;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayIntro() {
        audioSource.clip = intro;
        audioSource.volume = 0.6f;
        audioSource.loop = false;
        audioSource.Play();        
    }

    public void PlayBG() {
        audioSource.clip = normalState;
        audioSource.volume = 1f;
        audioSource.Play();
        audioSource.loop = true;
    }
}
