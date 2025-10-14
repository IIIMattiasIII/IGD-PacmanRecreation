using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// AKA: PacStudentController
/// </summary>
[RequireComponent(typeof(PlayerManager))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector3 direction { get; private set; } = Vector3.zero;
    public bool isMoving => Vector3.Magnitude(direction) > 0;
    [SerializeField] private Vector3 initDir; 
    public PlayerManager playerManager { get; private set; }
    private Vector3 initPosition;
    private Vector3 currentInput = Vector3.zero;
    private Vector3 lastInput = Vector3.zero;

    void Awake() {
        playerManager = GetComponent<PlayerManager>();
        initPosition = transform.position;
    }

    void Start() {
        Reset();
    }

    void Reset() {
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

    bool CanMove(Vector3 playerPos, Vector3 moveDir) {
        Vector3 newPos = new(playerPos.x + moveDir.x, playerPos.y + moveDir.y, 0);
        Vector3Int cellPos = playerManager.wallsMap.WorldToCell(newPos);
        bool tileExists = playerManager.wallsMap.HasTile(cellPos);
        return !tileExists;
    }

    void Move() {
        Vector3 dest = transform.position + currentInput;
        float time = Vector3.Distance(transform.position, dest) / moveSpeed;
        playerManager.tweener.AddTween(transform, transform.position, dest, time);
        direction = Vector3.Normalize(dest - transform.position);
    }

    void Update() {
        GetInput();
        if (!playerManager.tweener.TweenExists(transform))
        {
            if (CanMove(transform.position, lastInput)) {
                currentInput = lastInput;
                Move();
            } else if (CanMove(transform.position, currentInput)) {
                Move();
            } else {
                direction = Vector3.zero;
            }
            playerManager.animator.SetFloat("moveX", currentInput.x);
            playerManager.animator.SetFloat("moveY", currentInput.y);
        }
    }
}
