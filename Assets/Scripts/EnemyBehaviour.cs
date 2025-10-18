using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyBehaviour : MonoBehaviour
{
    public Enemy enemyManager { get; private set; }
    private Coroutine homeSeq;

    void Awake() {
        enemyManager = GetComponent<Enemy>();
    }
    
    public void HomeSequence(float time) {
        if (homeSeq != null) { return; }
        enemyManager.Trigger("normal");
        homeSeq = StartCoroutine(enemyManager.movement.HomeMovement(time));
    }

    public async void DeathSequence() {
        await enemyManager.movement.DeathMovement();
        enemyManager.state = Enemy.EnemyState.Home;
        enemyManager.levelManager.RespawnMusicCheck();
    }
}
