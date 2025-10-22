using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Middle-abstract class to override default scared behaviour and define other needed pathing functions
/// </summary>
public abstract class EnemyLvl1Behaviour : EnemyBehaviour
{
    protected override void Scared(List<Vector3> directions) {
        Furthest(directions, player.position);
    }

    protected void Furthest(List<Vector3> validDirections, Vector3 _target) {
        List<Tuple<Vector3, float>> directions = GetDirections(validDirections, _target);
        int idx = 0;
        directions.Sort((t1, t2) => t2.Item2.CompareTo(t1.Item2));
        if (directions.Count > 1 && IsBackstep(directions[idx].Item1)) {
            if (++idx >= directions.Count) { idx = 0; }
        }
        enemyManager.movement.SetMovement(directions[idx].Item1);
    }
}
