using System.Collections.Generic;
using UnityEngine;

public class Enemy3Behaviour : EnemyLvl1Behaviour
{
    protected override void Pathfind(List<Vector3> directions) {
        RandomDirection(directions);
    }
}
