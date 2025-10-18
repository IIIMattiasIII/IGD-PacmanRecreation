using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip normalState;
    public AudioClip scaredState;
    public AudioClip killerState;
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

    public void PlayScared() {
        audioSource.clip = scaredState;
        audioSource.volume = 1f;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void PlayKiller() {
        audioSource.clip = killerState;
        audioSource.volume = 1f;
        audioSource.Play();
        audioSource.loop = true;
    }
}
