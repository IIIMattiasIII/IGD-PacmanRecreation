using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LevelManager))]
public class LevelUI : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Image overlay;
    [SerializeField] private TextMeshProUGUI centreText;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI scaredTimer;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Transform lives;

    void Awake() {
        if (levelManager == null) levelManager = GetComponent<LevelManager>();
    }

    public async Task StartSequence() {
        await Task.Delay(100); // Slight delay to add visual clarity (mask unity leading) to start sequence
        overlay.gameObject.SetActive(true);
        await SetCentreText("3", .5f, 500);
        await SetCentreText("2", .5f, 500);
        await SetCentreText("1", .5f, 500);
        await SetCentreText("GO!", .5f, 500);
        overlay.gameObject.SetActive(false);
        return;
    }

    async Task SetCentreText(string text, float duration, int hold) {
        float initSize = centreText.fontSize;
        centreText.text = text;
        centreText.fontSize *= 0.75f;
        float elapsed = 0f;
        while (elapsed < duration) {
            float fSize = Mathf.Lerp(centreText.fontSize, initSize, elapsed/duration);
            centreText.fontSize = fSize;
            elapsed += Time.unscaledDeltaTime;
            await Task.Yield();
        }
        await Task.Delay(hold);
        return;
    }

    public void SetTime(float t) {
        TimeSpan ts = TimeSpan.FromSeconds(t);
        string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        timer.text = formattedTime;
    }

    public void SetScaredTime(float t) {
        scaredTimer.transform.parent.gameObject.SetActive(t > 0);
        TimeSpan ts = TimeSpan.FromSeconds(t);
        string formattedTime = string.Format("-{0:00}", ts.Seconds);
        scaredTimer.text = formattedTime;
    }

    public void SetScore(int sc) {
        string padded = sc.ToString("D6");
        score.text = padded;
    }

    public void SetLives(int l) {
        for (int i = 0; i < lives.childCount; i++) {
            lives.GetChild(i).gameObject.SetActive(l-- > 0);
        }
    }

    public async Task GameOver() {
        overlay.gameObject.SetActive(true);
        await SetCentreText("Game Over", .5f, 2500);
        StartUIManager menu = FindFirstObjectByType<StartUIManager>();
        if (menu == null) { // if scene loaded manually in editor
            UnityEditor.EditorApplication.isPlaying = false;
        } else {
            menu.LoadMenu();
            Time.timeScale = 1;
        }
    }
}
