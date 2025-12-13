using UnityEngine;

public class EnergySwitchController : MonoBehaviour
{
    // ======================================================
    //        CONFIGURACIÓN GENERAL DEL MÓDULO
    // ======================================================
    public enum ModuleType
    {
        None,
        Motor,
        Defensas,
        Presion,
        Sonar,
        Computadora,
    }

    [Header("Module Settings")]
    public ModuleType moduleType = ModuleType.None;
    public float energyConsumption = 20f;

    [Header("State")]
    public bool isOn = false;

    // ======================================================
    //               REFERENCIAS A OBJETOS
    // ======================================================

    [Header("Controller Reference")]
    public SwitchModule switchModule;  // animación física


    // ======================================================
    //     SUBSCRIPCIÓN A EVENTOS DEL ENERGYMANAGER
    // ======================================================
    private void OnEnable()
    {
        EnergyManager.OnOverload += HandleOverload;
        EnergyManager.OnRestored += HandleRestore;
    }

    private void OnDisable()
    {
        EnergyManager.OnOverload -= HandleOverload;
        EnergyManager.OnRestored -= HandleRestore;
    }

    // ======================================================
    //   MÉTODO PRINCIPAL QUE LLAMA EL SWITCH FÍSICO
    // ======================================================
    public void SetStateFromExternal(bool requestedState)
    {
        if (requestedState)
            TryTurnOn();
        else
            TurnOff();
    }


    // ======================================================
    //                  ENCENDER LÓGICAMENTE
    // ======================================================
    private void TryTurnOn()
    {
        if (!EnergyManager.IsOperational)
        {
            Debug.LogWarning($"[{moduleType}] No se puede encender: sistema en sobrecarga.");
            ForceSync(false);
            return;
        }

        bool ok = EnergyManager.RequestEnergy(energyConsumption);

        if (ok)
        {
            isOn = true;
            Debug.Log($"[{moduleType}] ENCENDIDO.");
            ForceSync(true);
        }
        else
        {
            Debug.Log($"[{moduleType}] Energía insuficiente.");
            ForceSync(false);
        }
    }


    // ======================================================
    //               APAGAR LÓGICO DEL MÓDULO
    // ======================================================
    private void TurnOff()
    {
        if (isOn && !EnergyManager.IsOverloaded)
            EnergyManager.ReleaseEnergy(energyConsumption);

        isOn = false;

        Debug.Log($"[{moduleType}] APAGADO.");
        ForceSync(false);
    }


    // ======================================================
    //           FORZAR SINCRONIZACIÓN VISUAL
    // ======================================================
    private void ForceSync(bool state)
    {
        if (switchModule != null && switchModule.isOn != state)
        {
            switchModule.SetStateExternally(state);
        }
    }

    // ======================================================
    //          EVENTOS DEL SISTEMA DE ENERGÍA
    // ======================================================
    private void HandleOverload()
    {
        if (isOn)
            TurnOff();
    }

    private void HandleRestore()
    {
        // Cuando se restaura, no se enciende automáticamente
        ForceSync(false);
    }
}
