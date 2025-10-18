using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerAnimations animations;
    public PlayerAudio audioManager;
    public LevelManager levelManager { get; private set; }
    public Animator animator { get; private set; }
    public bool isAlive => levelManager.levelState != LevelManager.GameState.PlayerDead;

    void Awake() {
        animator = GetComponent<Animator>();
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }
}
