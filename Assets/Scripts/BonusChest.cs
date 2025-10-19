using System.Threading.Tasks;
using UnityEngine;

public class BonusChest : MonoBehaviour
{
    public int points = 100;
    [SerializeField] private float moveSpeed = 2.2f;
    public TaskCompletionSource<bool> life = new();
    public Vector3 dest;
    public Tweener tweener;
    public LevelManager levelManager;

    void Start() {
        if (tweener == null) {
            Debug.LogWarning("Bouns Chest failed to access tweener");
            Destroy(gameObject);
            return;
        }
        dest = new(-transform.position.x, -transform.position.y, transform.position.z);
        float time = Vector3.Distance(transform.position, dest) / moveSpeed;
        tweener.AddTween(transform, transform.position, dest, time);
    }

    void Update() {
        if (tweener.TweenExists(transform)) { return; }
        life.SetResult(false);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            tweener.RemoveTween(transform);
            levelManager.BonusCollected(this);
            life.SetResult(true);
            Destroy(gameObject);
        }
    }
}
