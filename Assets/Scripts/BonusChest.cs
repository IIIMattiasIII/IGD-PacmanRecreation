using System.Threading.Tasks;
using UnityEngine;

public class BonusChest : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.2f;
    public TaskCompletionSource<bool> life = new();
    public Vector3 dest;
    private Tweener tweener;

    void Start() {
        tweener = GameObject.Find("LevelManager").GetComponent<Tweener>();
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
        throw new System.NotImplementedException("Bonus collisions not yet implemented");
        // if (other.gameObject.name == "Player") {
        //     life.SetResult(true);
        //     Destroy(gameObject);
        // }
    }
}
