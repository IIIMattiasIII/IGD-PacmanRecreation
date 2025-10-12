using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
/**
*   Support class to allow basic animating of canvas components using sprites, to save recreating the animations and animators, 
*   as the ones exported from Aseprite animate the sprite and not the image.
*/
public class CanvasAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float delay = 100;
    private Image image;
    private int frameIdx = 0;

    void Awake() {
        image = GetComponent<Image>();
    }

    void Start()
    {
        if (sprites.Length <= 1) {
            Debug.LogWarning("No sprites set in CanvasAnimator");
            return;
        }
        StartCoroutine(Animate());
    }

    IEnumerator Animate() {
        while (true) {
            image.sprite = sprites[frameIdx++];
            if (frameIdx >= sprites.Length) { frameIdx = 0; }
            yield return new WaitForSeconds(delay/1000);
        }
    }
}
