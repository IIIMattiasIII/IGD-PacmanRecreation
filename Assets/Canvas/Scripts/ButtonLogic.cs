using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ButtonLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject hoverPanel;
    [Header("Button Scaling")]
    [SerializeField] private float hoverScale = 1.01f;
    [SerializeField] private float clickScale = 0.98f;
    private bool hasPanel => hoverPanel != null;
    private bool isHovered = false;
    private bool isClicked = false;
    private Vector3 originalScale;
    private Button button;
    private ColorBlock originalColours;

    void Start() {
        if (hasPanel) {
            hoverPanel.SetActive(false);
        }
        originalScale = transform.localScale;
        button = gameObject.GetComponent<Button>();
        originalColours = button.colors;
    }

    void OpenHover() {
        if (hasPanel && !hoverPanel.activeSelf) {
            hoverPanel.SetActive(true);
        }
    }

    void CloseHover() {
        if (hasPanel && hoverPanel.activeSelf) {
            hoverPanel.SetActive(false);
        }
        isClicked = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!isClicked) {
            isHovered = true;
            OpenHover();
        }
        transform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
        if (!isClicked) {
            CloseHover();
            transform.localScale = originalScale;
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (hasPanel) {
            isClicked = !isClicked;
            if (isClicked) {
                OpenHover();
                ColorBlock c = button.colors;
                c.selectedColor = c.pressedColor;
                button.colors = c;
            } else if (!isClicked && !isHovered) {
                CloseHover();
                button.colors = originalColours;
            }
        }
        transform.localScale = originalScale * clickScale;
    }

    public void OnPointerUp(PointerEventData eventData) {
        transform.localScale = isHovered ? originalScale * hoverScale : originalScale;
    }
}
