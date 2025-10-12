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
        DontDestroyOnLoad(border.transform.parent);
        borderInset = border.rectTransform.offsetMax.y;
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
        await BorderTween(border.rectTransform, -50, duration);
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

    public async Task LoadSceneAsTask(int sceneIdx) {
        TaskCompletionSource<bool> tcs = new();
        AsyncOperation aOp = SceneManager.LoadSceneAsync(sceneIdx);
        aOp.completed += (op) => { tcs.SetResult(true); };
        await tcs.Task;
    }

    public async void LoadLevel(int sceneIdx) {
        titleCanvas.enabled = false;
        loadingText.enabled = true;
        List<Task> tasks = new()
        {
          BorderOut(5),
          LoadSceneAsTask(sceneIdx)
        };
        await Task.WhenAll(tasks);
    }
}
