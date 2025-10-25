using UnityEngine;

public class Inky : EnemyBehaviour
{
    [Header("Inky Vars")]
    [SerializeField] private Transform blinky;
    [SerializeField] private PlayerMovement playerMovement;


    protected override void SetTarget() {
        Vector3 facing = playerMovement.GetCurrentFacing();
        Vector3 initTarget = player.position + 2*facing;
        if (facing == Vector3.up) {
            // This maintains the hex overflow bug from the original game. Not a necessary addition, but seemed a faithful inclusion.
            initTarget.x -= 2;
        }
        Vector3 blinkyVec = initTarget - blinky.position;
        target = initTarget + blinkyVec;
    }
}
