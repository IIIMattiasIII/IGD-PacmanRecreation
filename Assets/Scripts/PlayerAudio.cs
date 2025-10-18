using System.Collections;
using System.Threading.Tasks;
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
    [SerializeField] AudioClip pellet;
    [SerializeField] AudioClip powerPellet;
    private Coroutine pelletCollect;

    void Awake() {
        source = GetComponent<AudioSource>();
        Transform feet = transform.Find("Feet");
        if (feet != null)
            stepSources = feet.GetComponents<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if (stepSources.Length > 0 && footsteps == null && playerMovement.direction.sqrMagnitude > 0 && pelletCollect == null) {
            stepIdx = 0;
            footsteps = StartCoroutine(PlayFootsteps());
        }
    }

    IEnumerator PlayFootsteps() {
        while (playerMovement.direction.sqrMagnitude > 0 && playerMovement.playerManager.isAlive  && pelletCollect == null)
        {
            stepSources[stepIdx++].Play();
            stepIdx = stepIdx >= stepSources.Length ? 0 : stepIdx;
            yield return new WaitForSeconds(0.3f);
        }
        footsteps = null;
    }

    public void Death() {
        source.clip = death;
        source.volume = 0.9f;
        source.Play();
    }

    public void WallHit() {
        source.clip = wallHit;
        source.volume = 1;
        source.Play();
    }

    public void Pellet() {
        if (pelletCollect != null) { StopCoroutine(pelletCollect); }
        pelletCollect = StartCoroutine(PelletAudio(pellet, 0.15f));
    }

    public void PowerPellet() {
        if (pelletCollect != null) { StopCoroutine(pelletCollect); }
        pelletCollect = StartCoroutine(PelletAudio(powerPellet));
    }

    public void BonusChest() {
        PowerPellet(); // Currently using same audio clip. Function defined in the event that is changed in the future.
    }

    IEnumerator PelletAudio(AudioClip clip, float vol = 1) {
        source.clip = clip;
        source.volume = vol;
        source.Play();
        yield return new WaitForSeconds(0.4f);
        pelletCollect = null;
    }
}
