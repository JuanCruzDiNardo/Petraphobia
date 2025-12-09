using UnityEngine;
using System;

public static class DrillManager
{
    // ============================
    // CONFIGURACIÓN GLOBAL
    // ============================
    public static float MaxSpeed = 60f; // km/h representativos
    public static float CurrentSpeed { get; private set; } = 0f;

    // Zonas de riesgo
    public enum DrillRisk
    {
        Safe,
        Unstable,
        Critical
    }

    public static DrillRisk CurrentRisk { get; private set; } = DrillRisk.Safe;

    // ============================
    // EVENTOS
    // ============================
    public static event Action<float> OnSpeedChanged;      // km/h reales
    public static event Action<DrillRisk> OnRiskChanged;   // safe/unstable/critical


    // ============================
    // MÉTODO PRINCIPAL:
    // RECIBE EL VALOR 0–100 DEL MÓDULO
    // ============================
    public static void UpdateDrillControl(float normalizedValue)
    {
        // Velocidad real del taladro
        CurrentSpeed = (normalizedValue / 100f) * MaxSpeed;

        OnSpeedChanged?.Invoke(CurrentSpeed);

        // Evaluar riesgo según tramos
        DrillRisk newRisk = EvaluateRisk(normalizedValue);

        if (newRisk != CurrentRisk)
        {
            CurrentRisk = newRisk;
            OnRiskChanged?.Invoke(CurrentRisk);
        }
    }


    private static DrillRisk EvaluateRisk(float value)
    {
        if (value < 30f) return DrillRisk.Safe;
        if (value < 70f) return DrillRisk.Unstable;
        return DrillRisk.Critical;
    }
}
