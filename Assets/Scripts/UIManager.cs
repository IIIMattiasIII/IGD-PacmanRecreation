using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Canvas titleCanvas;
    private float borderInset;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(border.transform.parent.gameObject);
        borderInset = border.rectTransform.offsetMin.y;
        border.rectTransform.offsetMin = new(-25, -25);
        border.rectTransform.offsetMax = new(25, 25);
    }

    void Start() {
        _ = BorderIn(3);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    async Task BorderIn(float duration) {
        await BorderTween(border.rectTransform, borderInset, duration);
    }

    async Task BorderOut(float duration) {
        await BorderTween(border.rectTransform, -25, duration);
    }

    async Task BorderTween(RectTransform rect, float newOffset, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            float lOffset = Mathf.Lerp(rect.offsetMin.y, newOffset, elapsed/duration);
            rect.offsetMax = new(-lOffset, -lOffset);
            rect.offsetMin = new(lOffset, lOffset);
            elapsed += Time.deltaTime;
            await Task.Yield();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 1) {
            GameObject qb = GameObject.FindWithTag("LevelExitButton");
            if (qb != null) qb.GetComponent<Button>().onClick.AddListener(LoadMenu);
        }
    }

    public static async Task LoadSceneAsTask(int sceneIdx) {
        TaskCompletionSource<bool> tcs = new();
        AsyncOperation aOp = SceneManager.LoadSceneAsync(sceneIdx);
        aOp.completed += (op) => { tcs.SetResult(true); };
        await tcs.Task;
    }

    async void LoadMenu() {
        await LoadSceneAsTask(0);
        Destroy(gameObject);
    }

    public async void LoadLevel(int sceneIdx) {
        titleCanvas.enabled = false;
        loadingText.enabled = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        List<Task> tasks = new()
        {
          BorderOut(5),
          LoadSceneAsTask(sceneIdx)
        };
        await Task.WhenAll(tasks);
        loadingText.enabled = false;
        Destroy(border.transform.parent.gameObject);
    }
}
