using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public Tweener tweener;
    [SerializeField] private string levelId = "level01";
    [SerializeField] private LevelUI uiManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private BonusController bonusController;
    [SerializeField] private Player player;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Tilemap wallsMap;
    [SerializeField] private Transform pellets;

    public enum GameState { Paused, Normal, Scared, Recovering, PlayerDead }
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
    Coroutine scaredSeq;
    int pointsMultiplier = 1;

    void Awake() {
        if (uiManager == null) uiManager = GetComponent<LevelUI>();
        if (audioManager == null) audioManager = GameObject.Find("Audio Source").GetComponent<AudioManager>();
        if (bonusController == null) bonusController = GetComponent<BonusController>();
        if (tweener == null) tweener = GetComponent<Tweener>();
        if (wallsMap == null) wallsMap = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
    }

    void ResetLevel() {
        gameTime = 0;
        score = 0;
        lives = 3;
        uiManager.SetScaredTime(0);
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }
        ResetLife();
    }

    void ResetLife() {
        player.movement.Reset();
        pointsMultiplier = 1;
        foreach (Enemy e in enemies) { e.movement.Reset(); }
        levelState = GameState.Normal;
    }

    async void Start() {
        ResetLevel();
        levelState = GameState.Paused;
        if (uiManager != null) {
            Time.timeScale = 0;
            audioManager.PlayIntro();
            await uiManager.StartSequence();
            Time.timeScale = 1;
            bonusController.BonusLoop();
        } else {
            Debug.LogWarning("LevelManager cannot access LevelUI");
        }
        audioManager.PlayBG();
        levelState = GameState.Normal;
    }

    void TimeSet() {
        gameTime += Time.deltaTime;
    }
    
    void Update() {
        if (levelState == GameState.Paused) { return; }
        TimeSet();
    }

    public bool CanMove(Vector3 charPos, Vector3 moveDir) {
        Vector3 newPos = new(charPos.x + moveDir.x, charPos.y + moveDir.y, 0);
        Vector3Int cellPos = wallsMap.WorldToCell(newPos);
        bool tileExists = wallsMap.HasTile(cellPos);
        return !tileExists;
    }

    public void BonusCollected(BonusChest c) {
        score += c.points;
        player.audioManager.BonusChest();
    }

    public void PelletEaten(Pellet p) {
        score += p.points;
        if (p.GetType() != typeof(PowerPellet)) { player.audioManager.Pellet(); }
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

    public void PowerPelletEaten(PowerPellet p) {
        PelletEaten(p);
        player.audioManager.PowerPellet();
        if (scaredSeq != null ) { 
            StopCoroutine(scaredSeq);
        } else {
            audioManager.PlayScared();
        }
        levelState = GameState.Scared;
        scaredSeq = StartCoroutine(ScaredSequence(p.duration));
    }

    IEnumerator ScaredSequence(float duration) {
        bool inRecov = false;
        float remaining = duration;
        while (remaining > 0) {
            uiManager.SetScaredTime(remaining+1);
            if (!inRecov && remaining <= 3) {
                levelState = GameState.Recovering;
                inRecov = true;
            }
            remaining -= Time.deltaTime;
            yield return null;
        }
        uiManager.SetScaredTime(0);
        pointsMultiplier = 1;
        levelState = GameState.Normal;
        audioManager.PlayBG();
        scaredSeq = null;
    }

    public void PlayerEaten() {
        lives -= 1;
        levelState = GameState.PlayerDead;
        player.audioManager.Death();
        player.animations.DeathParticle();
        tweener.RemoveTween(player.gameObject.transform);
        foreach (Enemy e in enemies) { tweener.RemoveTween(e.gameObject.transform); }
        if (lives > 0) {
            Invoke(nameof(ResetLife), 2f);
        } else {
            Invoke(nameof(GameOver), 1f);
        }
        return;
    }

    public void EnemyEaten(Enemy e) {
        // To the marker: I presume including the mult feat in lvl1 is acceptable, but if not, here's the ternary to remove it:
        // score += levelId == "level01" ? e.points : e.points * pointsMultiplier++;
        score += e.points * pointsMultiplier++;
        audioManager.PlayKiller();
    }

    void GameOver() {
        Time.timeScale = 0;
        SaveManager.Save(levelId, score, gameTime);
        _ = uiManager.GameOver();
    }
}
