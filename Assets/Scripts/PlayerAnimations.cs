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
    private Animator animator;

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

    void Start() { 
        animator = playerMovement.playerManager.animator;
    }

    void ToggleParticles(ParticleSystem particles, bool state) {
        ParticleSystem.EmissionModule particleEmission = particles.emission;
        particleEmission.enabled = state;
    }

    void Update() {
        animator.SetBool("dead", isDead);      
        if (playerMovement.isMoving && !stepParticles.emission.enabled) {
            ToggleParticles(stepParticles, true);
            animator.speed = 1f;
        } else if (!playerMovement.isMoving) {
            if (stepParticles.emission.enabled) { ToggleParticles(stepParticles, false); }
            animator.speed = isDead ? 1f : 0f;
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
