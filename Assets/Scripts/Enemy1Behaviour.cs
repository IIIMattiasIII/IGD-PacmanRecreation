using System.Collections.Generic;
using UnityEngine;

public class Enemy1Behaviour : EnemyLvl1Behaviour
{
    protected override void Chase(List<Vector3> directions) {
        Furthest(directions, player.position);
    }
}
