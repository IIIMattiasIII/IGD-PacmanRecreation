using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector3 direction { get; private set; }
    public Tweener tweener;
    private Animator animator;
    private Vector2[] positions = {
        new(-12.5f, 12.5f),
        new(-12.5f, 8.5f),
        new(-7.5f, 8.5f),
        new(-7.5f, 12.5f),
    };
    private int positionIdx;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (tweener == null)
            tweener = GameObject.Find("GameManager").GetComponent<Tweener>();
    }
    void Start()
    {
        Reset();
    }

    void Reset()
    {
        transform.position = new Vector3(positions[0].x, positions[0].y, transform.position.z);
        positionIdx = 0;
    }

    void Update()
    {
        if (tweener == null) return;
        if (!tweener.TweenExists(transform))
        {
            Vector3 dest = new(positions[positionIdx].x, positions[positionIdx].y, transform.position.z);
            positionIdx = ++positionIdx >= positions.Length ? 0 : positionIdx;
            float time = Vector3.Distance(transform.position, dest) / moveSpeed;
            tweener.AddTween(transform, transform.position, dest, time);
            direction = Vector3.Normalize(dest - transform.position);
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }
    }
}
