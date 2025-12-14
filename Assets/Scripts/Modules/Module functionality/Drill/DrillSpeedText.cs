using UnityEngine;
using TMPro;

public class DrillSpeedTextUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI speedText;

    void OnEnable()
    {
        DrillManager.OnSpeedChanged += UpdateSpeedText;
    }

    void OnDisable()
    {
        DrillManager.OnSpeedChanged -= UpdateSpeedText;
    }

    void Start()
    {
        // Inicializar en 0
        UpdateSpeedText(0f);
    }

    void UpdateSpeedText(float speed)
    {
        speedText.text = $"{Mathf.RoundToInt(speed)} km/h";
    }
}
