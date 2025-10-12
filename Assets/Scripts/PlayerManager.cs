using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
public class PlayerManager : MonoBehaviour
{
    public Tweener tweener { get; private set; }
    public Animator animator { get; private set; }
    public Tilemap wallsMap { get; private set; }

    void Awake() {
        animator = GetComponent<Animator>();
        tweener = GameObject.Find("LevelManager").GetComponent<Tweener>();
        wallsMap = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
    }
}
