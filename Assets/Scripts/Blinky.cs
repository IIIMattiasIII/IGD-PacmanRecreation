using UnityEngine;

public class Blinky : EnemyBehaviour
{
    public override void Reset() {
        base.Reset();
        enemyManager.state = Enemy.EnemyState.Normal;
    }

    protected override void SetTarget() {
        target = player.position;
    }
}
