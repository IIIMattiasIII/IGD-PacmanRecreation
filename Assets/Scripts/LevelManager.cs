using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Tweener tweener;
    [SerializeField] private string levelId = "level01";
    [SerializeField] private LevelUI uiManager;
    [SerializeField] private BonusController bonusController;
    [SerializeField] private Player player;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform pellets;

    public enum GameState { Paused, Normal, Scared, Dead }
    public GameState levelState = GameState.Paused;
    private float _timer = 0;
    public float gameTime {
        get { return _timer; }
        private set {
            _timer = value;
            uiManager.SetTime(value);
        }
    }
    private int _score = 0;
    public int score {
        get { return _score; }
        private set {
            _score = value;
            uiManager.SetScore(value);
        }
    }
    private int _lives = 3;
    public int lives {
        get { return _lives; }
        private set {
            _lives = value;
            uiManager.SetLives(value);
        }
    }

    void Awake() {
        if (uiManager == null) uiManager = GetComponent<LevelUI>();
        if (bonusController == null) bonusController = GetComponent<BonusController>();
        if (tweener == null) tweener = GetComponent<Tweener>();
    }

    void ResetLevel() {
        gameTime = 0;
        score = 0;
        lives = 3;
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }
        ResetLife();
    }

    void ResetLife() {
        player.movement.Reset();
        // enemy reset
        levelState = GameState.Normal;
    }

    async void Start() {
        ResetLevel();
        levelState = GameState.Paused;
        if (uiManager != null) {
            Time.timeScale = 0;
            await uiManager.StartSequence();
            Time.timeScale = 1;
            bonusController.BonusLoop();
        } else {
            Debug.LogWarning("LevelManager cannot access LevelUI");
        }
        levelState = GameState.Normal;
    }

    void TimeSet() {
        gameTime += Time.deltaTime;
    }
    
    void Update() {
        if (levelState == GameState.Paused) { return; }
        TimeSet();
    }

    public void AddPoints(int p) {
        score += p;     
    }

    public void PelletEaten() {
        // to be implemented
        if (!HasPellets()) {
            GameOver();
        }
    }

    bool HasPellets() {
        foreach (Transform pellet in pellets) {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }
        return false;
    }

    public void PlayerEaten() {
        lives -= 1;
        levelState = GameState.Dead;
        player.audioManager.Death();
        tweener.RemoveTween(player.gameObject.transform);
        foreach (GameObject e in enemies) { tweener.RemoveTween(e.transform); }
        if (lives > 0) {
            Invoke(nameof(ResetLife), 2f);
        } else {
            Invoke(nameof(GameOver), 1f);
        }
        return;
    }

    void GameOver() {
        Time.timeScale = 0;
        SaveManager.Save(levelId, score, gameTime);
        _ = uiManager.GameOver();
    }
}
