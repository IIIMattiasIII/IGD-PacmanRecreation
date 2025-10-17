using System.Threading.Tasks;
using UnityEngine;

public class BonusChest : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private int points = 100;
    public TaskCompletionSource<bool> life = new();
    public Vector3 dest;
    private Tweener tweener;
    private LevelManager levelManager;

    void Start() {
        GameObject g = GameObject.FindWithTag("LevelManager");
        tweener = g.GetComponent<Tweener>();
        levelManager = g.GetComponent<LevelManager>();
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
        if (other.gameObject.name == "Player") {
            tweener.RemoveTween(transform);
            levelManager.AddPoints(points);
            life.SetResult(true);
            Destroy(gameObject);
        }
    }
}
