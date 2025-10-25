using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy4Behaviour : EnemyLvl1Behaviour
{
    [Header("Enemy 4 Vars")]
    [Tooltip("Tunnel entry cross intersection coords")]
    [SerializeField] private Vector2[] edgeTargets;
    private bool targetReached = false;

    public override void Reset() {
        base.Reset();
        EdgeTargetReset();
    }

    protected override void HomeExit() {
        base.HomeExit();
        EdgeTargetReset();
    }

    /*  Tunnels are used as preferred edge-finding locations as they ensure consistency in direction; if a corner or similar
        point were used, there are two possible entry directions, causing the 'hug left wall' approach to cause an 
        anti-clockwise movement around an inner wall rather than the clockwise movement around the outer walls/edge.
        This can have the effect of the enemy having to briefly travel not along an edge or counter-clockwise to reach the 
        'edge starting point', but it ensures the edge-following is consistent.
        The specifications don't state whether this is acceptable, but it also doesn't specify how to account for the scared 
        state movements either - and the version in the demo video also doesn't seem to account for this, so I cannot use 
        that as a reference either. I presume this is an acceptable solution (and it is more elegant than harcoding the full 
        path/coordinates/turns and just travelling back to that path/the previous location when the scared sequence ends).
    */
    public void EdgeTargetReset() {
        targetReached = false;
        target = edgeTargets.Aggregate((currentMin, next) =>
            Vector2.Distance(transform.position, next) < Vector2.Distance(transform.position, currentMin) ? next : currentMin
        );
        Debug.Log(target.x);
    }

    protected override void Chase(List<Vector3> directions) {
        if (!targetReached) { GetToEdge(directions); }
        else { FollowEdge(directions); }
    }

    private void GetToEdge(List<Vector3> directions) {
        if (Vector2.Distance(transform.position, target) <= 1.5f) {
            if (edgeTargets.Contains(target)) { // Is at tunnel entry
                target.x = 11 * Math.Sign(target.x); // Shift target so that tunnel is entered
            } else {
                targetReached = true; // Is in tunnel, so can now shift to following the edge wall
            }
        }
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
