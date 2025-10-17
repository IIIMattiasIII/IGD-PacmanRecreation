using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimations : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private ParticleSystem particles;
    private bool isDead => !playerMovement.playerManager.isAlive;

    void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    void ToggleParticles(bool state) {
        ParticleSystem.EmissionModule particleEmission = particles.emission;
        particleEmission.enabled = state;
    }

    void Update() {
        playerMovement.playerManager.animator.SetBool("dead", isDead);
        if (!isDead && playerMovement.isMoving && !particles.emission.enabled) {
            ToggleParticles(true);
        } else if ((isDead || !playerMovement.isMoving) && particles.emission.enabled) {
            ToggleParticles(false);
        }
    }
}
