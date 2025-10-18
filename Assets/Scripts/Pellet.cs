using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10;

    protected virtual void Eat() {
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().PelletEaten(this);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            gameObject.SetActive(false);
            Eat();
        }
    }
}
