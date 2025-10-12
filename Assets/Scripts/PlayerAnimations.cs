using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimations : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private ParticleSystem particles;

    void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    void ToggleParticles(bool state) {
        ParticleSystem.EmissionModule particleEmission = particles.emission;
        particleEmission.enabled = state;
    }

    void Update() {
        if (playerMovement.isMoving && !particles.emission.enabled) {
            ToggleParticles(true);
        } else if (!playerMovement.isMoving && particles.emission.enabled) {
            ToggleParticles(false);
        }
    }
}
