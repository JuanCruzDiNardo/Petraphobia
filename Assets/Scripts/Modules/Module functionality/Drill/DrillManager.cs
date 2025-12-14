using UnityEngine;
using System;

public static class DrillManager
{
    // ============================
    // CONFIGURACIÓN
    // ============================
    public static float MaxSpeed = 60f; // km/h
    public static float CurrentSpeed { get; private set; } = 0f;

    // Distancia
    public static float TotalDistance { get; private set; }
    public static float CurrentDistance { get; private set; }

    private static bool initialized = false;

    // ============================
    // RIESGO
    // ============================
    public enum DrillRisk { Safe, Unstable, Critical }
    public static DrillRisk CurrentRisk { get; private set; } = DrillRisk.Safe;

    // ============================
    // EVENTOS
    // ============================
    public static event Action<float> OnSpeedChanged;
    public static event Action<float> OnProgressChanged; // 0–1
    public static event Action<DrillRisk> OnRiskChanged;
    public static event Action OnDrillStopped;

    // ============================
    // INIT
    // ============================
    public static void Initialize()
    {
        TotalDistance = UnityEngine.Random.Range(800f, 1200f); // unidades narrativas
        CurrentDistance = 0f;
        CurrentSpeed = 0f;
        CurrentRisk = DrillRisk.Safe;

        initialized = true;

        OnSpeedChanged?.Invoke(0f);
        OnProgressChanged?.Invoke(0f);
        OnRiskChanged?.Invoke(CurrentRisk);
    }

    // ============================
    // INPUT DESDE PALANCA
    // ============================
    public static void UpdateDrillControl(float normalizedValue)
    {
        if (!initialized)
            Initialize();

        // 🔴 SIN ENERGÍA → IGNORAR INPUT
        if (!EnergyManager.IsOperational)
        {
            ForceStop();
            return;
        }

        CurrentSpeed = (normalizedValue / 100f) * MaxSpeed;
        OnSpeedChanged?.Invoke(CurrentSpeed);

        DrillRisk newRisk = EvaluateRisk(normalizedValue);
        if (newRisk != CurrentRisk)
        {
            CurrentRisk = newRisk;
            OnRiskChanged?.Invoke(CurrentRisk);
        }
    }

    // ============================
    // UPDATE TEMPORAL (llamado desde runner)
    // ============================
    public static void Tick(float deltaTime)
    {
        if (!initialized) return;

        // 🔴 SIN ENERGÍA → NO AVANZA
        if (!EnergyManager.IsOperational || CurrentSpeed <= 0f)
            return;

        // Conversión narrativa (no física)
        float advance = CurrentSpeed * deltaTime * 0.1f;
        CurrentDistance += advance;

        float progress = Mathf.Clamp01(CurrentDistance / TotalDistance);
        OnProgressChanged?.Invoke(progress);
    }

    // ============================
    // FORZAR PARADA
    // ============================
    public static void ForceStop()
    {
        if (CurrentSpeed == 0f) return;

        CurrentSpeed = 0f;
        OnSpeedChanged?.Invoke(0f);
        OnDrillStopped?.Invoke();
    }

    // ============================
    private static DrillRisk EvaluateRisk(float value)
    {
        if (value < 30f) return DrillRisk.Safe;
        if (value < 70f) return DrillRisk.Unstable;
        return DrillRisk.Critical;
    }
}
