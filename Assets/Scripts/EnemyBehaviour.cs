using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AKA: GhostController
/// </summary>
[RequireComponent(typeof(Enemy))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    public Enemy enemyManager { get; private set; }
    [SerializeField] protected Vector3 scatterCorner;
    [SerializeField] protected Vector3 target;
    [SerializeField] protected Transform player;
    private Coroutine homeSeq;

    // ===
    [Header("Path Target Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private Sprite debugSprite;
    [SerializeField] private Color debugCol = new(255,255,255);
    private GameObject debugNode;
    void TargetDebug(){
        if (debug && debugNode == null && enemyManager.state == Enemy.EnemyState.Chase) {
            debugNode = new GameObject();
            debugNode.name = gameObject.name + "_DebugNode";
            SpriteRenderer sr = debugNode.AddComponent<SpriteRenderer>();
            sr.sprite = debugSprite;
            sr.color = debugCol;
        } else if ((!debug || enemyManager.state != Enemy.EnemyState.Chase) && debugNode != null) {
            Destroy(debugNode);
            debugNode = null;
        }
        if (debugNode != null) debugNode.transform.position = target;
    }
    // ===

    void Awake() {
        enemyManager = GetComponent<Enemy>();
    }
    
    void Update() {
        TargetDebug();
    }

    public virtual void Reset() {
        enemyManager.state = Enemy.EnemyState.Home;
        homeSeq = null;
    }

    public void HomeSequence(float time) {
        if (homeSeq != null) { return; }
        enemyManager.Trigger("normal");
        if (enemyManager.levelManager.gameTime > 1) { time = 0; } // Home time shouldn't apply after death
        homeSeq = StartCoroutine(enemyManager.movement.HomeMovement(time, HomeExit));
    }

    protected virtual void HomeExit() {
        homeSeq = null;
        enemyManager.state = enemyManager.levelManager.isInnov ? enemyManager.levelManager.lastAttackState : Enemy.EnemyState.Chase;
        enemyManager.levelManager.CheckElroy();
    }

    public async void DeathSequence() {
        await enemyManager.movement.DeathMovement();
        if (enemyManager.levelManager == null) return;
        enemyManager.state = Enemy.EnemyState.Home;
        enemyManager.levelManager.RespawnMusicCheck();
    }

    protected bool IsBackstep(Vector3 direction) {
        return direction == -enemyManager.movement.currentDir;
    }

    public virtual void SwapAttackState(Enemy.EnemyState attackState) {
        if (enemyManager.isActive) {
            enemyManager.state = attackState;
            enemyManager.movement.Backstep();
        }
    }

    protected virtual void Scared(List<Vector3> directions) {
        RandomDirection(directions);
    }

    protected virtual void Scatter(List<Vector3> directions) {
        Closest(directions, scatterCorner);
    }
    
    protected virtual void Chase(List<Vector3> directions) {
        SetTarget();
        Closest(directions, target);
    }

    protected virtual void SetTarget() {}

    public void Pathfind() {
        List<Vector3> validDirs = enemyManager.levelManager.GetValidDirections(transform.position);
        if (enemyManager.isFrightened) {
            Scared(validDirs);
        } else if (enemyManager.state == Enemy.EnemyState.Chase) {
            Chase(validDirs);
        } else if (enemyManager.state == Enemy.EnemyState.Scatter) {
            Scatter(validDirs);
        }
    }
    
    protected List<Tuple<Vector3, float>> GetDirections(List<Vector3> validDirections, Vector3 _target) {
        List<Tuple<Vector3, float>> directions = new();
        foreach (Vector3 valDir in validDirections) {
            Vector3 newPos = transform.position + new Vector3(valDir.x, valDir.y, transform.position.z);
            float dist = Vector3.Distance(newPos, _target);
            directions.Add(new Tuple<Vector3, float>(valDir, dist));
        }
        return directions;
    }

    static int GetVectorPriority(Vector3 vec) { // Mirrors original game's priority
        if (vec == Vector3.up) return 0;
        if (vec == Vector3.left) return 1;
        if (vec == Vector3.down) return 2;
        if (vec == Vector3.right) return 3;
        Debug.LogWarning("Unexpected vector direction when prioritising");
        return 4; // This shouldn't ever occur
    }

    static int CompareDirections(Tuple<Vector3, float> t1, Tuple<Vector3, float> t2) {
        int floatComparison = t1.Item2.CompareTo(t2.Item2);
        if (floatComparison == 0) { return GetVectorPriority(t1.Item1).CompareTo(GetVectorPriority(t2.Item1)); }
        return floatComparison;
    }

    protected void Closest(List<Vector3> validDirections, Vector3 _target) {
        List<Tuple<Vector3, float>> directions = GetDirections(validDirections, _target);
        int idx = 0;
        directions.Sort(CompareDirections);
        if (directions.Count > 1 && IsBackstep(directions[idx].Item1)) {
            if (++idx >= directions.Count) { idx = 0; }
        }
        enemyManager.movement.SetMovement(directions[idx].Item1);
    }

    protected void RandomDirection(List<Vector3> directions) {
        int idx = UnityEngine.Random.Range(0, directions.Count);
        if (directions.Count > 1 && IsBackstep(directions[idx])) {
            if (++idx >= directions.Count) { idx = 0; }
        }
        enemyManager.movement.SetMovement(directions[idx]);
    }
}
