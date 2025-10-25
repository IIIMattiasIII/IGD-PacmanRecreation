using UnityEngine;

public class Pinky : EnemyBehaviour
{
    [Header("Pinky Vars")]
    [SerializeField] private PlayerMovement playerMovement;

    protected override void SetTarget() {
        Vector3 facing = playerMovement.GetCurrentFacing();
        target = player.position + 4*facing;
        if (facing == Vector3.up) {
            // This maintains the hex overflow bug from the original game. Not a necessary addition, but seemed a faithful inclusion.
            target.x -= 4;
        }
    }
}
