using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2.7f;
    public Vector3 direction { get; private set; } = Vector3.zero;
    public bool isMoving => Vector3.Magnitude(direction) > 0;
    [SerializeField] private Vector3 initDir; 
    [SerializeField] private Vector3 homePosition;
    [SerializeField] private Vector3 exitPosition = new(0, 2.5f, -4f);
    public Enemy enemyManager { get; private set; }
    private Tweener tweener;
    private Vector3 currentDir = Vector3.zero;
    private bool playerDead => enemyManager.levelManager.levelState == LevelManager.GameState.PlayerDead;
    Coroutine homeSeq;

    void Awake() {
        enemyManager = GetComponent<Enemy>();
    }

    void Start() {
        tweener = enemyManager.levelManager.tweener;
        Reset();
    }

    public void Reset() {
        transform.position = homePosition;
        enemyManager.state = Enemy.EnemyState.Home;
    }


    void Move(Vector3 dest) {
        if (dest == null) dest = transform.position + currentDir;
        float time = Vector3.Distance(transform.position, dest) / moveSpeed;
        tweener.AddTween(transform, transform.position, dest, time);
        direction = Vector3.Normalize(dest - transform.position);
    }

    void Update() {
        if (playerDead || enemyManager.state == Enemy.EnemyState.Dead || enemyManager.state == Enemy.EnemyState.Home) { return; }
        if (!tweener.TweenExists(transform)) {
            // todo - enemy movement and pathfinding
        }
        enemyManager.animator.SetFloat("moveX", currentDir.x);
        enemyManager.animator.SetFloat("moveY", currentDir.y);
    }

    public void HomeSequence(float time) {
        if (homeSeq != null) { return; }
        enemyManager.Trigger("normal");
        homeSeq = StartCoroutine(HomeMovement(time));
    }

    IEnumerator HomeMovement(float time) {
        while (time > 0) {
            // todo movement
            yield return null;
        }
        enemyManager.state = Enemy.EnemyState.Normal;
        currentDir = initDir;
        homeSeq = null;
    }

    public async void Death() {
        tweener.RemoveTween(transform);
        currentDir = Vector3.zero;
        Move(homePosition);
        enemyManager.animator.SetFloat("moveX", direction.x);
        enemyManager.animator.SetFloat("moveY", direction.y);
        // while (tweener.TweenExists(transform)) { await Task.Yield(); }
        await Task.Delay(3000); // I'm glad I read ahead to see this gets removed, cause otherwise this line just seems wrong.
        enemyManager.state = Enemy.EnemyState.Home;
    }
}
