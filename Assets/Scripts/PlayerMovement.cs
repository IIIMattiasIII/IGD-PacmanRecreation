using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// AKA: PacStudentController
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector3 direction { get; private set; } = Vector3.zero;
    public bool isMoving => Vector3.Magnitude(direction) > 0;
    [SerializeField] private Vector3 initDir; 
    [SerializeField] private Vector3 initPosition;
    public Player playerManager { get; private set; }
    private Tweener tweener;
    private Vector3 currentInput = Vector3.zero;
    private Vector3 lastInput = Vector3.zero;

    void Awake() {
        playerManager = GetComponent<Player>();
    }

    void Start() {
        tweener = playerManager.levelManager.tweener;
        Reset();
    }

    public void Reset() {
        transform.position = initPosition;
        currentInput = initDir;
        lastInput = initDir;
    }

    void GetInput() {
        if (Input.GetKeyDown(KeyCode.W)) {
            lastInput = Vector3.up;
        } else if (Input.GetKeyDown(KeyCode.S)) {
            lastInput = Vector3.down;
        } else if (Input.GetKeyDown(KeyCode.A)) {
            lastInput = Vector3.left;
        } else if (Input.GetKeyDown(KeyCode.D)) {
            lastInput = Vector3.right;
        }
    }

    void Move() {
        Vector3 dest = transform.position + currentInput;
        float time = Vector3.Distance(transform.position, dest) / moveSpeed;
        tweener.AddTween(transform, transform.position, dest, time);
        direction = Vector3.Normalize(dest - transform.position);
    }

    void Update() {
        GetInput();
        if (lastInput == Vector3.zero) { return; }
        if (!tweener.TweenExists(transform) && playerManager.isAlive) {
            if (playerManager.levelManager.CanMove(transform.position, lastInput)) {
                currentInput = lastInput;
                Move();
            } else if (playerManager.levelManager.CanMove(transform.position, currentInput)) {
                Move();
            } else if (direction != Vector3.zero) {
                playerManager.audioManager.WallHit();
                playerManager.animations.WallParticle(currentInput);
                direction = Vector3.zero;
            }
            playerManager.animator.SetFloat("moveX", currentInput.x);
            playerManager.animator.SetFloat("moveY", currentInput.y);
        }
    }
}
