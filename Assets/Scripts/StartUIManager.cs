using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Canvas titleCanvas;
    private float borderInset;

    void Awake() {
        borderInset = border.rectTransform.offsetMin.y;
    }

    async void Start() {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(border.transform.parent.gameObject);
        border.rectTransform.offsetMin = new(-25, -25);
        border.rectTransform.offsetMax = new(25, 25);
        // Border lerp breaks if called immediately - only with _unscaled_ time, I'd guess due to engine loading time being considered part of frame?
        await Awaitable.WaitForSecondsAsync(0.1f);
        _ = BorderIn(.75f);
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
        float initOffset = rect.offsetMin.y;
        while (elapsed < duration) {
            float lOffset = Mathf.Lerp(initOffset, newOffset, elapsed/duration);
            rect.offsetMax = new(-lOffset, -lOffset);
            rect.offsetMin = new(lOffset, lOffset);
            elapsed += Time.unscaledDeltaTime;
            await Task.Yield();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        GameObject qb = GameObject.FindWithTag("LevelExitButton");
        if (qb != null) qb.GetComponent<Button>().onClick.AddListener(LoadMenu);
    }

    public static async Task LoadSceneAsTask(int sceneIdx) {
        TaskCompletionSource<bool> tcs = new();
        AsyncOperation aOp = SceneManager.LoadSceneAsync(sceneIdx);
        aOp.completed += (op) => { tcs.SetResult(true); };
        await tcs.Task;
    }

    public async void LoadMenu() {
        await LoadSceneAsTask(0);
        if (border != null) Destroy(border.transform.parent.gameObject);
        Destroy(gameObject);
    }

    public async void LoadLevel(int sceneIdx) {
        titleCanvas.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Task bt = BorderOut(1.5f);
        await LoadSceneAsTask(sceneIdx);
        loadingText.gameObject.SetActive(false);
        LevelManager lm = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        if (lm.isInnov) {
            lm.uiManager.BorderIn = () => _ = BorderIn(.75f);
            lm.uiManager.BorderOut = () => _ = BorderOut(.75f);
        } else {
            await bt;
            Destroy(border.transform.parent.gameObject);
        }
    }
}
