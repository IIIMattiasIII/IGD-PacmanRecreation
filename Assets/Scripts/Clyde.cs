using UnityEngine;

public class Clyde : EnemyBehaviour
{
    protected override void SetTarget() {
        if (Vector2.Distance(transform.position, player.position) < 8) {
            target = scatterCorner;
        } else {
            target = player.position;
        }
    }
}
