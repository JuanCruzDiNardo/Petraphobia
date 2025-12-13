using UnityEngine;
using System;

public static class EnergyManager
{
    // ================================
    //  VARIABLES ESTÁTICAS
    // ================================
    private static float _currentEnergy = 100f;
    public static float CurrentEnergy => _currentEnergy;

    public static float MaxEnergy = 100f;

    public static bool IsOperational = true;
    public static bool IsOverloaded = false;

    // ================================
    //  EVENTOS ESTÁTICOS
    // ================================
    public static event Action<float> OnEnergyChanged;
    public static event Action OnOverload;
    public static event Action OnRestored;


    // =========================================================
    // MÉTODO CENTRAL: MODIFICAR ENERGÍA
    // =========================================================
    private static bool TryModifyEnergy(float delta)
    {
        if (!IsOperational)
        {
            Debug.LogWarning("Energy system is not operational.");
            return false;
        }

        float newValue = _currentEnergy + delta;

        if (newValue < 0f)
        {
            Debug.Log("Not enough energy to perform this operation.");
            return false;
        }

        // clamp al máximo
        if (newValue > MaxEnergy)
            newValue = MaxEnergy;

        _currentEnergy = newValue;

        OnEnergyChanged?.Invoke(_currentEnergy);

        return true;
    }


    // =========================================================
    // SOLICITAR ENERGÍA (encender módulo)
    // =========================================================
    public static bool RequestEnergy(float amount)
    {
        amount = -Mathf.Abs(amount); // consumo → negativo

        return TryModifyEnergy(amount);
    }


    // =========================================================
    // LIBERAR ENERGÍA (apagar módulo)
    // =========================================================
    public static bool ReleaseEnergy(float amount)
    {
        amount = Mathf.Abs(amount); // liberar → positivo

        return TryModifyEnergy(amount);
    }


    // =========================================================
    // FORZAR SOBRECARGA
    // =========================================================
    public static void ForceOverload()
    {
        if (IsOverloaded)
            return;

        Debug.Log("⚠ OVERLOAD DETECTED!");
        ConsoleTextPrinter.Log("OVERLOAD DETECTED!");

        IsOperational = false;
        IsOverloaded = true;

        OnOverload?.Invoke();
    }


    // =========================================================
    // RESTAURAR SISTEMA
    // =========================================================
    public static void RestoreEnergy()
    {
        Debug.Log("Energy system restored.");

        IsOperational = true;
        IsOverloaded = false;

        _currentEnergy = MaxEnergy;

        OnEnergyChanged?.Invoke(_currentEnergy);
        OnRestored?.Invoke();
    }

}
