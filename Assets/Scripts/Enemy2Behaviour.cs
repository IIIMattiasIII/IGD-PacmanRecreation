using System.Collections.Generic;
using UnityEngine;

public class Enemy2Behaviour : EnemyLvl1Behaviour
{
    protected override void Pathfind(List<Vector3> directions) {
        Closest(directions, player.position);
    }
}
