using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    private AudioSource[] stepSources;
    private PlayerMovement playerMovement;
    private int stepIdx = 0;
    private Coroutine footsteps;
    private AudioSource source;
    [SerializeField] AudioClip wallHit;
    [SerializeField] AudioClip death;

    void Awake() {
        source = GetComponent<AudioSource>();
        Transform feet = transform.Find("Feet");
        if (feet != null)
            stepSources = feet.GetComponents<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if (stepSources.Length > 0 && footsteps == null && playerMovement.direction.sqrMagnitude > 0) {
            stepIdx = 0;
            footsteps = StartCoroutine(PlayFootsteps());
        }
    }

    IEnumerator PlayFootsteps() {
        while (playerMovement.direction.sqrMagnitude > 0 && playerMovement.playerManager.isAlive)
        {
            stepSources[stepIdx++].Play();
            stepIdx = stepIdx >= stepSources.Length ? 0 : stepIdx;
            yield return new WaitForSeconds(0.3f);
        }
        footsteps = null;
    }

    public void Death() {
        source.clip = death;
        source.Play();
    }

    public void WallHit() {
        
    }
}
