using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip normalState;
    public AudioClip scaredState;
    public AudioClip killerState;
    private AudioSource audioSource;
    [SerializeField] private float pauseVolMult = 0.3f;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPause() {
        audioSource.volume *= pauseVolMult;
    }

    public void OnPlay() {
        audioSource.volume *= 1/pauseVolMult;
    }

    public void PlayIntro() {
        audioSource.clip = intro;
        audioSource.volume = 0.6f;
        audioSource.loop = false;
        audioSource.Play();        
    }

    public void PlayBG() {
        if (audioSource.clip == normalState) { return; }
        float time = 0;
        if (audioSource.loop) {
            time = audioSource.time % normalState.length;
        }
        audioSource.clip = normalState;
        audioSource.volume = 1f;
        audioSource.time = time;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void PlayScared() {
        if (audioSource.clip == scaredState) { return; }
        float time = 0;
        if (audioSource.loop) {
            time = audioSource.time % normalState.length;
        }
        audioSource.clip = scaredState;
        audioSource.volume = 1f;
        audioSource.time = time;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void PlayKiller() {
        if (audioSource.clip == killerState) { return; }
        float time = 0;
        if (audioSource.loop) {
            time = audioSource.time % normalState.length;
        }
        audioSource.clip = killerState;
        audioSource.volume = 1f;
        audioSource.time = time;
        audioSource.Play();
        audioSource.loop = true;
    }
}
