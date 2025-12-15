using UnityEngine;
using UnityEngine.UI;

public class ManualManager : MonoBehaviour
{
    /* ===============================
       SINGLETON
       =============================== */
    public static ManualManager Instance { get; private set; }

    /* ===============================
       UI
       =============================== */
    [Header("Manual Canvas")]
    [SerializeField] private GameObject manualCanvas;
    [SerializeField] private Image mainImage;

    [Header("Tutorial Images")]
    [SerializeField] private Sprite[] tutorialImages;

    [Header("Navigation Buttons")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Header("Close Buttons")]
    [SerializeField] private Button[] closeButtons;

    /* ===============================
       STATE
       =============================== */
    private int currentIndex = 0;
    public bool IsOpen { get; private set; } = false;

    /* ===============================
       UNITY
       =============================== */
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        manualCanvas.SetActive(false);

        prevButton.onClick.AddListener(Previous);
        nextButton.onClick.AddListener(Next);

        foreach (var btn in closeButtons)
            btn.onClick.AddListener(CloseManual);

        UpdateImage();
    }

    /* ===============================
       PUBLIC API
       =============================== */

    public void OpenManual()
    {
        if (IsOpen) return;

        IsOpen = true;
        manualCanvas.SetActive(true);        
        UpdateImage();

        BlockGameplayInput(true);
    }

    public void CloseManual()
    {
        if (!IsOpen) return;

        IsOpen = false;
        manualCanvas.SetActive(false);

        BlockGameplayInput(false);
    }

    /* ===============================
       NAVIGATION
       =============================== */

    void Next()
    {
        if (currentIndex < tutorialImages.Length - 1)
        {
            currentIndex++;
            UpdateImage();
        }
    }

    void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateImage();
        }
    }

    void UpdateImage()
    {
        if (tutorialImages.Length == 0) return;

        mainImage.sprite = tutorialImages[currentIndex];

        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < tutorialImages.Length - 1;
    }

    /* ===============================
       INPUT & CAMERA LOCK
       =============================== */

    void BlockGameplayInput(bool block)
    {
        // 🔒 BLOQUEAR INPUT GENERAL
        InputBlocker.SetBlocked(block);

        // 🔒 BLOQUEAR CÁMARA
        //CameraController.Instance?.SetRotationEnabled(!block);

        // 🔓 Mostrar cursor
        //Cursor.visible = block;
        //Cursor.lockState = block ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
