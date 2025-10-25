using System.Collections.Generic;
using UnityEngine;

public class Enemy3Behaviour : EnemyLvl1Behaviour
{
    protected override void Chase(List<Vector3> directions) {
        RandomDirection(directions);
    }
}
