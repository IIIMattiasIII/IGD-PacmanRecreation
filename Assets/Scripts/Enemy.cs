using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public EnemyMovement movement;
    public LevelManager levelManager { get; private set; }
    public Animator animator { get; private set; }
    public int points = 300;
    public enum EnemyState { Home, Normal, Scared, Res, Dead }
    public EnemyState state = EnemyState.Home;
    [SerializeField] float homeTime = 0;

    void Awake() {
        animator = GetComponent<Animator>();
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }

    public void Trigger(string triggerName = null) {
        foreach (AnimatorControllerParameter parameter in animator.parameters) {
            if (parameter.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(parameter.name);
            }
        }
        if (triggerName != null) {
            animator.SetTrigger(triggerName);   
        }
    }

    void CheckState() {
        if (state == EnemyState.Dead) { return; }
        else if (state == EnemyState.Home) { movement.HomeSequence(homeTime); }
        else if (levelManager.levelState == LevelManager.GameState.Normal && state != EnemyState.Normal) {
            state = EnemyState.Normal;
            Trigger("normal");
        } else if (levelManager.levelState == LevelManager.GameState.Scared && state != EnemyState.Scared) {
            state = EnemyState.Scared;
            Trigger("scared");
        } else if (levelManager.levelState == LevelManager.GameState.Recovering && state != EnemyState.Res) {
            state = EnemyState.Res;
            Trigger("res");
        }
    }

    void Update() {
        CheckState();
    }
  
  
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (state == EnemyState.Normal) {
                levelManager.PlayerEaten();
            } else if (state == EnemyState.Scared || state == EnemyState.Res) {
                levelManager.EnemyEaten(this);
                state = EnemyState.Dead;
                Trigger("dead");
                movement.Death();
            }
        }
    }
}
