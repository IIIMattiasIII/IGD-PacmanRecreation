using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimations : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] private ParticleSystem stepParticles;
    [SerializeField] private ParticleSystem wallParticles;
    [SerializeField] private ParticleSystem deathParticles;
    private bool isDead => !playerMovement.playerManager.isAlive;

    void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        if (stepParticles == null || wallParticles == null) {
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in particles) {
                if (p.name == "WalkParticles") { stepParticles = p; }
                else if (p.name == "WallHitParticle") { wallParticles = p; }
                else if (p.name == "DeathParticle") { deathParticles = p; }
            }
        }
    }

    void ToggleParticles(ParticleSystem particles, bool state) {
        ParticleSystem.EmissionModule particleEmission = particles.emission;
        particleEmission.enabled = state;
    }

    void Update() {
        playerMovement.playerManager.animator.SetBool("dead", isDead);
        if (!isDead && playerMovement.isMoving && !stepParticles.emission.enabled) {
            ToggleParticles(stepParticles, true);
            playerMovement.playerManager.animator.speed = 1f;
        } else if ((isDead || !playerMovement.isMoving) && stepParticles.emission.enabled) {
            ToggleParticles(stepParticles, false);
            if (!isDead) { playerMovement.playerManager.animator.speed = 0f; }
        }
    }

    public void WallParticle(Vector3 dir) {
        ParticleSystem.ShapeModule particleShape = wallParticles.shape;
        particleShape.rotation = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        wallParticles.Emit(1);
    }

    public void DeathParticle() {
        deathParticles.Emit(1);
    }
}
