using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AKA: GhostController
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2.7f;
    public Vector3 direction { get; private set; } = Vector3.zero;
    public Vector3 homePosition;
    public bool isMoving => Vector3.Magnitude(direction) > 0;
    [SerializeField] private Vector3 initDir; 
    [SerializeField] private Vector3 exitPosition = new(0, 2.5f, -4f);
    public Enemy enemyManager { get; private set; }
    private Tweener tweener;
    public Vector3 currentDir { get; private set; } = Vector3.zero;
    public Vector3 nextDir { get; private set; } = Vector3.zero;
    private bool playerDead => enemyManager.levelManager.levelState == LevelManager.GameState.PlayerDead;

    void Awake() {
        enemyManager = GetComponent<Enemy>();
    }

    void Start() {
        tweener = enemyManager.levelManager.tweener;
        Reset();
    }

    public void Reset() {
        transform.position = homePosition;
    }

    public void SetMovement(Vector3 dir) {
        nextDir = dir;
    }

    public void Move(Vector3? dest = null) {
        if (dest == null) dest = transform.position + currentDir;
        float time = Vector3.Distance(transform.position, (Vector3)dest) / moveSpeed;
        tweener.AddTween(transform, transform.position, (Vector3)dest, time);
        direction = Vector3.Normalize((Vector3)dest - transform.position);
    }

    void Update() {
        if (playerDead || enemyManager.isInactive) { return; }
        if (!tweener.TweenExists(transform)) {
            if (enemyManager.levelManager.CanMove(transform.position, nextDir)) {
                currentDir = nextDir;
                Move();
            } else if (enemyManager.levelManager.CanMove(transform.position, currentDir)) {
                Move();
            }
        }
        enemyManager.animator.SetFloat("moveX", currentDir.x);
        enemyManager.animator.SetFloat("moveY", currentDir.y);
    }
    
    public IEnumerator HomeMovement(float time, Action Callback) {
        if (currentDir != Vector3.up || currentDir != Vector3.down) {
            if (transform.position.y < 0) { currentDir = Vector3.up; }
            else { currentDir = Vector3.down; }
        }
        while (time > 0) {
            time -= Time.deltaTime;
            if (tweener.TweenExists(transform)) { yield return null; continue; }
            if (!enemyManager.levelManager.CanMove(transform.position, currentDir)) {
                currentDir *= -1;                    
            }
            enemyManager.movement.Move();
        }
        while (tweener.TweenExists(transform)) { yield return null; }
        Vector3 horizPos = exitPosition;
        horizPos.y = transform.position.y;
        enemyManager.movement.Move(horizPos);
        while (tweener.TweenExists(transform)) { yield return null; }
        enemyManager.movement.Move(exitPosition);
        while (tweener.TweenExists(transform)) { yield return null; }
        enemyManager.state = Enemy.EnemyState.Normal;
        nextDir = initDir;
        if (nextDir == Vector3.zero) {
            List<Vector3> dirs = enemyManager.levelManager.GetValidDirections(transform.position);
            nextDir = dirs[UnityEngine.Random.Range(0, dirs.Count)];
        }
        Callback();
    }

    public async Task DeathMovement() {
        tweener.RemoveTween(transform);
        Move(homePosition);
        enemyManager.animator.SetFloat("moveX", direction.x);
        enemyManager.animator.SetFloat("moveY", direction.y);
        while (tweener.TweenExists(transform)) { await Task.Yield(); }
        return;
    }
}
