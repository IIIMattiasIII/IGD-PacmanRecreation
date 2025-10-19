using System.Collections.Generic;
using UnityEngine;

public class Enemy4Behaviour : EnemyLvl1Behaviour
{
    public bool targetReached = false;

    public override void Reset() {
        base.Reset();
        targetReached = false;
    }

    protected override void Pathfind(List<Vector3> directions) {
        if (!targetReached) { GetToEdge(directions); }
        else { FollowEdge(directions); }
    }

    private void GetToEdge(List<Vector3> directions) {
        Closest(directions, target.position);
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
