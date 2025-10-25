using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy4Behaviour : EnemyLvl1Behaviour
{
    [Header("Enemy 4 Vars")]
    [SerializeField] private Transform[] edgeTargets;
    private bool targetReached = false;

    public override void Reset() {
        base.Reset();
        EdgeTargetReset();
    }

    protected override void HomeExit() {
        base.HomeExit();
        EdgeTargetReset();
    }

    // Prefer further tunnel to ensure it is consistently entered. If closer tunnel is used when targeting is done near 
    // corners, can get caught in endless loop around one of the wall blocks.
    // Tunnels are used as preferred edge-finding location as they ensure consistency in direction; if a corner or similar
    // point were used, there are two possible entry directions, causing the 'hug left wall' approach to cause an 
    // anti-clockwise movement around an inner wall rather than the clockwise movement around the outer walls/edge.
    public void EdgeTargetReset() {
        targetReached = false;
        List<Transform> prefEdgeTarget = edgeTargets.ToList();
        prefEdgeTarget.Sort(
            (t1, t2) => Vector2.Distance(transform.position, t2.position).CompareTo(Vector2.Distance(transform.position, t1.position))
        );
        target = prefEdgeTarget[0].position;
    }

    protected override void Chase(List<Vector3> directions) {
        if (!targetReached) { GetToEdge(directions); }
        else { FollowEdge(directions); }
    }

    private void GetToEdge(List<Vector3> directions) {
        if (Vector2.Distance(transform.position, target) <= 1.5f) { targetReached = true; }
        Closest(directions, target);
    }

    private void FollowEdge(List<Vector3> directions) {
        Vector3 dir = enemyManager.movement.currentDir;
        dir = TurnLeft(dir);
        while (!directions.Contains(dir)) { dir = TurnRight(dir); }
        enemyManager.movement.SetMovement(dir);
    }

    public Vector3 TurnLeft(Vector3 dir) {
        return new Vector3(-dir.y, dir.x, 0);
    }

    public Vector3 TurnRight(Vector3 dir) {
        return new Vector3(dir.y, -dir.x, 0);
    }
}
