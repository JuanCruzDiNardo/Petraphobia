using UnityEngine;
using UnityEngine.UI;

public class CustomMouseCursor : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private Canvas canvas;

    [Header("Tamaños")]
    [SerializeField] private float normalSize = 10f;
    [SerializeField] private float hoverSize = 25f;

    [Header("Transparencia")]
    [SerializeField] private float normalAlpha = 1f;
    [SerializeField] private float hoverAlpha = 0.5f;

    [Header("Suavizado")]
    [SerializeField] private float transitionSpeed = 10f;

    [Header("Interacción")]
    [SerializeField] private LayerMask interactableLayer;

    private RectTransform rectTransform;
    private Camera mainCamera;
    private bool isActive = false;

    void Awake()
    {
        rectTransform = cursorImage.rectTransform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isActive) return;

        UpdatePosition();
        UpdateState();
    }

    public void SetActiveCursor(bool active)
    {
        isActive = active;
        cursorImage.gameObject.SetActive(active);

        if (active)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void UpdatePosition()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out mousePos
        );

        rectTransform.localPosition = mousePos;
    }

    void UpdateState()
    {
        bool isHoveringInteractable = CheckInteractable();

        float targetSize = isHoveringInteractable ? hoverSize : normalSize;
        float targetAlpha = isHoveringInteractable ? hoverAlpha : normalAlpha;

        float newSize = Mathf.Lerp(
            rectTransform.sizeDelta.x,
            targetSize,
            Time.deltaTime * transitionSpeed
        );

        rectTransform.sizeDelta = new Vector2(newSize, newSize);

        Color color = cursorImage.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * transitionSpeed);
        cursorImage.color = color;
    }

    bool CheckInteractable()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, 100f, interactableLayer);
    }
}
