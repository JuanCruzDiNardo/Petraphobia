using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    [Header("UI")]
    public Slider energySlider;

    private void OnEnable()
    {
        // Suscribirse a los eventos del EnergyManager
        EnergyManager.OnEnergyChanged += UpdateBar;
        EnergyManager.OnRestored += RestoreBar;
    }

    private void OnDisable()
    {
        // Desuscripción para evitar errores
        EnergyManager.OnEnergyChanged -= UpdateBar;
        EnergyManager.OnRestored -= RestoreBar;
    }

    private void Start()
    {
        // Configurar slider inicial
        if (energySlider != null)
        {
            energySlider.minValue = 0f;
            energySlider.maxValue = EnergyManager.MaxEnergy;
            energySlider.value = EnergyManager.CurrentEnergy;
        }
    }

    private void UpdateBar(float currentEnergy)
    {
        if (energySlider != null)
            energySlider.value = currentEnergy;
    }

    private void RestoreBar()
    {
        if (energySlider != null)
            energySlider.value = EnergyManager.MaxEnergy;
    }
}
