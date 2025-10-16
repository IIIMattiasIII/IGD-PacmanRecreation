using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelUI uiManager;
    [SerializeField] private BonusController bonusController;

    public bool startDone { get; private set; } = false;

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

    void Awake() {
        if (uiManager == null) uiManager = GetComponent<LevelUI>();
        if (bonusController == null) bonusController = GetComponent<BonusController>();
    }

    async void Start() {
        if (uiManager != null) {
            Time.timeScale = 0;
            await uiManager.StartSequence();
            Time.timeScale = 1;
            bonusController.BonusLoop();
        } else {
            Debug.LogWarning("LevelManager cannot access LevelUI");
        }
        startDone = true;
    }

    void TimeSet() {
        gameTime += Time.deltaTime;
    }
    
    void Update() {
        if (!startDone) { return; }
        TimeSet();
    }

    public void AddPoints(int p) {
        score += p;     
    }
}
