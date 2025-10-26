using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public Tweener tweener;
    public float baseMoveSpeed = 3;
    public Vector2 levelSize = new(28,29);
    [SerializeField] private string levelId = "level01";
    public LevelUI uiManager;
    public AudioManager audioManager;
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
    private Coroutine scaredSeq;
    private int pointsMultiplier = 0;
    public bool isInnov => levelId == "level02";
    public LevelAttackStateTimer attackTimer;
    public Enemy.EnemyState lastAttackState = Enemy.EnemyState.Scatter;

    void Awake() {
        if (uiManager == null) uiManager = GetComponent<LevelUI>();
        if (audioManager == null) audioManager = GameObject.Find("Audio Source").GetComponent<AudioManager>();
        if (bonusController == null) bonusController = GetComponent<BonusController>();
        if (tweener == null) tweener = GetComponent<Tweener>();
        if (wallsMap == null) wallsMap = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();

        if (isInnov) {
            // Timings match original game's first level, but stretched out to account for this games slower move speed
            List<LevelAttackStateTimer.TimerEvent> timerEvents = new() {
              new LevelAttackStateTimer.TimerEvent { triggerTime = 9f, state = Enemy.EnemyState.Chase },
              new LevelAttackStateTimer.TimerEvent { triggerTime = 35f, state = Enemy.EnemyState.Scatter },
              new LevelAttackStateTimer.TimerEvent { triggerTime = 44f, state = Enemy.EnemyState.Chase },
              new LevelAttackStateTimer.TimerEvent { triggerTime = 70f, state = Enemy.EnemyState.Scatter },
              new LevelAttackStateTimer.TimerEvent { triggerTime = 76.5f, state = Enemy.EnemyState.Chase },
              new LevelAttackStateTimer.TimerEvent { triggerTime = 102.5f, state = Enemy.EnemyState.Scatter },
              new LevelAttackStateTimer.TimerEvent { triggerTime = 109f, state = Enemy.EnemyState.Chase }
            };
            attackTimer = new LevelAttackStateTimer(timerEvents, enemies, s => lastAttackState = s);
        }
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
        pointsMultiplier = 0;
        foreach (Enemy e in enemies) {
            e.behaviour.Reset();
            e.movement.Reset();
        }
        levelState = GameState.Normal;
        attackTimer?.Reset();
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
        attackTimer?.Start();
    }

    void TimeSet() {
        gameTime += Time.deltaTime;
    }
    
    void Update() {
        if (levelState == GameState.Paused) { return; }
        TimeSet();
        attackTimer?.Update();
    }

    public bool CanMove(Vector3 charPos, Vector3 moveDir) {
        Vector3 newPos = new(charPos.x + moveDir.x, charPos.y + moveDir.y, 0);
        Vector3Int cellPos = wallsMap.WorldToCell(newPos);
        bool tileExists = wallsMap.HasTile(cellPos);
        return !tileExists;
    }

    public List<Vector3> GetValidDirections(Vector3 pos) {
        List<Vector3> ret = new();
        if (CanMove(pos, Vector3.up)) { ret.Add(Vector3.up); }
        if (CanMove(pos, Vector3.left)) { ret.Add(Vector3.left); }
        if (CanMove(pos, Vector3.down)) { ret.Add(Vector3.down); }
        if (CanMove(pos, Vector3.right)) { ret.Add(Vector3.right); }
        return ret;
    }
    
    public static float HalfUnit(float value) {
        float sign = Math.Sign(value);
        float absValue = Math.Abs(value);
        float fractionalPart = absValue - (float)Math.Truncate(absValue);

        if (Math.Abs(fractionalPart - 0.5f) < float.Epsilon) { return value; }
        else { return ((float)Math.Truncate(absValue) + 0.5f) * sign; }
    }

    public static Vector3 GridAlign(Vector3 pos, Vector3 moveDir) {
        if (moveDir == Vector3.up || moveDir == Vector3.down) pos.y = HalfUnit(pos.y);
        else if (moveDir == Vector3.left || moveDir == Vector3.right) pos.x = HalfUnit(pos.x);
        return pos;
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
        CheckElroy();
    }

    bool HasPellets() {
        foreach (Transform pellet in pellets) {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }
        return false;
    }

    public void CheckElroy() {
        if (isInnov && RemainingPelletCount() <= 20 && enemies.All(e => e.isActive)) {
            foreach (Enemy e in enemies) { if (e.TryGetComponent(out Blinky b)) { b.CruiseElroy(); } }
        }
    }

    int RemainingPelletCount() {
        int ret = 0;
        foreach (Transform pellet in pellets) {
            if (pellet.gameObject.activeSelf) { ret++; }
        }
        return ret;
    }

    public void PowerPelletEaten(PowerPellet p) {
        PelletEaten(p);
        player.audioManager.PowerPellet();
        if (scaredSeq != null ) { 
            StopCoroutine(scaredSeq);
        } else {
            audioManager.PlayScared();
            if (isInnov) {
                foreach (Enemy e in enemies) { e.movement.Backstep(); }
                attackTimer.Stop();
            }
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
        pointsMultiplier = 0;
        levelState = GameState.Normal;
        RespawnMusicCheck();
        foreach (Enemy e in enemies) { if (e.TryGetComponent(out Enemy4Behaviour eb)) { eb.EdgeTargetReset(); }}
        attackTimer?.Start();
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
        score += isInnov ? e.points * (int)Math.Pow(2, pointsMultiplier++) : e.points;
        audioManager.PlayKiller();
    }

    public void RespawnMusicCheck() {
        if (enemies.All(e => e.state != Enemy.EnemyState.Dead)) {
            if (levelState == GameState.Normal) {
                audioManager.PlayBG();
            } else if (levelState == GameState.Scared) {
                audioManager.PlayScared();
            }
        }
    }

    void GameOver() {
        Time.timeScale = 0;
        SaveManager.Save(levelId, score, gameTime);
        _ = uiManager.GameOver();
    }
}
