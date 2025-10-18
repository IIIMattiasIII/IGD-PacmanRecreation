using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyBehaviour behaviour;
    public LevelManager levelManager { get; private set; }
    public Animator animator { get; private set; }
    public int points = 300;
    public enum EnemyState { Home, Normal, Scared, Res, Dead }
    public EnemyState state = EnemyState.Home;
    public bool isActive => state == EnemyState.Normal || state == EnemyState.Home;
    public bool isFrightened => state == EnemyState.Scared || state == EnemyState.Res;
    public bool isInactive => state == EnemyState.Home || state == EnemyState.Dead;
    [SerializeField] float homeTime = 0;

    void Awake() {
        animator = GetComponent<Animator>();
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }

    public void Trigger(string triggerName = null, EnemyState? state = null) {
        foreach (AnimatorControllerParameter parameter in animator.parameters) {
            if (parameter.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(parameter.name);
            }
        }
        if (triggerName != null) { animator.SetTrigger(triggerName); }
        if (state != null) { this.state = (EnemyState)state; }
        if (this.state == EnemyState.Home || this.state == EnemyState.Normal) {
            movement.moveSpeed = .9f*levelManager.baseMoveSpeed;
        } else {
            movement.moveSpeed = .45f*levelManager.baseMoveSpeed;
        }
    }

    void CheckState() {
        if (state == EnemyState.Dead) { return; }
        else if (state == EnemyState.Home) { behaviour.HomeSequence(homeTime); }
        else if (levelManager.levelState == LevelManager.GameState.Normal && state != EnemyState.Normal) {
            Trigger("normal", EnemyState.Normal);
        } else if (levelManager.levelState == LevelManager.GameState.Scared && state != EnemyState.Scared) {
            Trigger("scared", EnemyState.Scared);
        } else if (levelManager.levelState == LevelManager.GameState.Recovering && state != EnemyState.Res) {
            Trigger("res", EnemyState.Res);
        }
    }

    void Update() {
        CheckState();
    }
  
  
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (state == EnemyState.Normal) {
                levelManager.PlayerEaten();
            } else if (isFrightened) {
                levelManager.EnemyEaten(this);
                Trigger("dead", EnemyState.Dead);
                behaviour.DeathSequence();
            }
        }
    }
}
