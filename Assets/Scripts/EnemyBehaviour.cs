using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AKA: GhostController
/// </summary>
[RequireComponent(typeof(Enemy))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    public Enemy enemyManager { get; private set; }
    [SerializeField] protected Transform target;
    [SerializeField] protected Transform player;
    private Coroutine homeSeq;

    void Awake() {
        enemyManager = GetComponent<Enemy>();
    }

    public virtual void Reset() {
        enemyManager.state = Enemy.EnemyState.Home;
        homeSeq = null;
    }
    
    public void HomeSequence(float time) {
        if (homeSeq != null) { return; }
        enemyManager.Trigger("normal");
        homeSeq = StartCoroutine(enemyManager.movement.HomeMovement(time, ()=>homeSeq = null));
        
    }

    public async void DeathSequence() {
        await enemyManager.movement.DeathMovement();
        enemyManager.state = Enemy.EnemyState.Home;
        enemyManager.levelManager.RespawnMusicCheck();
    }

    protected bool IsBackstep(Vector3 direction) {
        return direction == -enemyManager.movement.currentDir;
    }

    protected virtual void Scared(List<Vector3> directions) {
        RandomDirection(directions);
    }
    
    protected abstract void Pathfind(List<Vector3> directions);

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.TryGetComponent(out Node node)) { return; }
        if (enemyManager.isFrightened) {
            Scared(node.validDirs);
        } else if (enemyManager.state == Enemy.EnemyState.Normal) {
            Pathfind(node.validDirs);
        }
    }

    protected void RandomDirection(List<Vector3> directions) {
        int idx = Random.Range(0, directions.Count);
        if (directions.Count > 1 && IsBackstep(directions[idx])) {
            if (++idx >= directions.Count) { idx = 0; }
        }
        enemyManager.movement.SetMovement(directions[idx]);
    }
}
