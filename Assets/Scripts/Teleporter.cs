using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform exit;
    [SerializeField] private Tweener tweener;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            tweener.RemoveTween(other.transform);
            Vector3 pos = exit.position;
            pos.z = other.transform.position.z;
            other.transform.position = pos;
        } else if (other.CompareTag("Enemy")) {
            // Add enemy logic (invert their direction)
        }
    }
}
