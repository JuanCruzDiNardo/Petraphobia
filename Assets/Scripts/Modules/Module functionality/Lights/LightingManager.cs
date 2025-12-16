using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [Header("Luz Principal")]
    [SerializeField] private Light mainLight;

    [Header("Luces de Emergencia")]
    [SerializeField] private Light[] emergencyLights;

    private bool energyOnline = true;

    void Start()
    {
        ApplyLightingState();
        EnergyManager.OnOverload += OverloadLights;
        EnergyManager.OnRestored += RestoreLights;
    }

    // 🔴 Llamar cuando ocurre una sobrecarga
    public void OverloadLights()
    {
        energyOnline = false;
        ApplyLightingState();
    }

    // 🟢 Llamar cuando se restaura la energía
    public void RestoreLights()
    {
        energyOnline = true;
        ApplyLightingState();
    }

    private void ApplyLightingState()
    {
        if (mainLight != null)
            mainLight.enabled = energyOnline;

        foreach (Light light in emergencyLights)
        {
            if (light != null)
                light.enabled = !energyOnline;
        }
    }

}
