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
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private AudioManager audioManager;

    void Awake() {
        if (levelManager == null) levelManager = GetComponent<LevelManager>();
        if (audioManager == null) audioManager = GameObject.Find("Audio Source").GetComponent<AudioManager>();
    }

    public async Task StartSequence() {
        overlay.gameObject.SetActive(true);
        audioManager.PlayIntro();
        await SetCentreText("3", .5f, 500);
        await SetCentreText("2", .5f, 500);
        await SetCentreText("1", .5f, 500);
        await SetCentreText("GO!", .5f, 500);
        overlay.gameObject.SetActive(false);
        audioManager.PlayBG();
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

    public void SetScore(int sc) {
        string padded = sc.ToString("D6");
        score.text = padded;
    }
}
