using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudio : MonoBehaviour
{
    private AudioSource[] audioSources;
    private PlayerMovement playerMovement;
    private int stepIdx = 0;
    private Coroutine footsteps;

    void Awake()
    {
        // audioSources = GetComponentsInChildren<AudioSource>();
        Transform feet = transform.Find("Feet");
        if (feet != null)
            audioSources = feet.GetComponents<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (audioSources.Length > 0 && footsteps == null && playerMovement.direction.sqrMagnitude > 0)
        {
            stepIdx = 0;
            footsteps = StartCoroutine(PlayFootsteps());
        }
    }

    IEnumerator PlayFootsteps()
    {
        while (playerMovement.direction.sqrMagnitude > 0)
        {
            audioSources[stepIdx++].Play();
            stepIdx = stepIdx >= audioSources.Length ? 0 : stepIdx;
            yield return new WaitForSeconds(0.3f);
        }
        footsteps = null;
    }
}
