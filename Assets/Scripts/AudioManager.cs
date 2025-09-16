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

    void Start()
    {
        StartCoroutine(PlayRequired());
    }

    IEnumerator PlayRequired()
    {
        audioSource.clip = intro;
        audioSource.volume = 0.6f;
        audioSource.Play();
        yield return new WaitForSeconds(intro.length);
        audioSource.clip = normalState;
        audioSource.volume = 1f;
        audioSource.Play();
        audioSource.loop = true;
    }
}
