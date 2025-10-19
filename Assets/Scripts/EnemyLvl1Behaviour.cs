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

    private List<Tuple<Vector3, float>> GetDirections(List<Vector3> validDirections, Vector3 _target) {
        List<Tuple<Vector3, float>> directions = new();
        foreach (Vector3 valDir in validDirections) {
            Vector3 newPos = transform.position + new Vector3(valDir.x, valDir.y, transform.position.z);
            float dist = Vector3.Distance(newPos, _target);
            directions.Add(new Tuple<Vector3, float>(valDir, dist));
        }
        return directions;
    }

    protected void Closest(List<Vector3> validDirections, Vector3 _target) {
        List<Tuple<Vector3, float>> directions = GetDirections(validDirections, _target);
        int idx = 0;
        directions.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
        if (directions.Count > 1 && IsBackstep(directions[idx].Item1)) {
            if (++idx >= directions.Count) { idx = 0; }
        }
        enemyManager.movement.SetMovement(directions[idx].Item1);
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
