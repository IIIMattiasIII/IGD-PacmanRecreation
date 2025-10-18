using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AKA: GhostController
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyBehaviour : MonoBehaviour
{
    public Enemy enemyManager { get; private set; }
    [SerializeField] private Transform target;
    private Coroutine homeSeq;

    void Awake() {
        enemyManager = GetComponent<Enemy>();
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

    protected virtual void Scared(List<Vector3> directions) {
        int idx = Random.Range(0, directions.Count);
        if (directions.Count > 1 && directions[idx] == -enemyManager.movement.currentDir) {
            if (++idx >= directions.Count) { idx = 0; }
        }
        enemyManager.movement.SetMovement(directions[idx]);
    }
    
    // public abstract void Pathfind(List<Vector3> directions);

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.TryGetComponent(out Node node)) { return; }
        Scared(node.validDirs);
        // if (enemyManager.isFrightened) {
        //     Scared(node.validDirs);
        // } else if (enemyManager.isActive) {
        //     Pathfind(node.validDirs);
        // }
    }
}
