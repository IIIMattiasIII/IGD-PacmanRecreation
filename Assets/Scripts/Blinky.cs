using UnityEngine;

public class Blinky : EnemyBehaviour
{
    bool elroy = false;
    public override void Reset() {
        base.Reset();
        enemyManager.state = enemyManager.levelManager.lastAttackState;
        enemyManager.baseSpeedModifier = .9f;
        elroy = false;
    }

    protected override void SetTarget() {
        target = player.position;
    }

    public void CruiseElroy() {
        if (enemyManager.state == Enemy.EnemyState.Scatter) {
            enemyManager.state = Enemy.EnemyState.Chase;
        }
        enemyManager.baseSpeedModifier = 1f;
        elroy = true;
        enemyManager.Trigger("normal");
    }

    public override void SwapAttackState(Enemy.EnemyState attackState) {
        if (elroy) { return; }
        base.SwapAttackState(attackState);
    }
}
