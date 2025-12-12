using UnityEngine;
using System;

public static class EnemyManager
{
    // ---------------------------------------------------------
    // ESTADOS
    // ---------------------------------------------------------
    public enum EnemyState
    {
        Wandering,
        Stalking,
        Attacking
    }

    public static EnemyState CurrentState { get; private set; } = EnemyState.Wandering;

    // ---------------------------------------------------------
    // AGRESIVIDAD
    // ---------------------------------------------------------
    public static float BaseAggressiveness = 0.1f;
    public static float CurrentAggressiveness = 0.1f;
    public static float AggressivenessIncreaseRate = 0.01f;
    public static float MaxAggressiveness = 1f;

    // ---------------------------------------------------------
    // TIMERS
    // ---------------------------------------------------------
    public static float CheckInterval = 3f;
    private static float checkTimer;

    public static float AttackTimeLimit = 20f;
    private static float attackTimer;

    public static bool DefenseActivated = false;

    // ---------------------------------------------------------
    // PROBABILIDADES
    // ---------------------------------------------------------
    public static float StalkingChanceMultiplier = 1.5f;
    public static float AttackChanceMultiplier = 2f;

    // ---------------------------------------------------------
    // EVENTOS
    // ---------------------------------------------------------
    public static event Action OnStalking;
    public static event Action OnAttack;
    public static event Action OnAttackFailed;
    public static event Action OnAttackDefended;   

    // ---------------------------------------------------------
    // UPDATE (DEBE SER LLAMADO DESDE UN MONOBEHAVIOUR)
    // ---------------------------------------------------------
    public static void Update()
    {
        // Incremento progresivo de agresividad
        CurrentAggressiveness = Mathf.Clamp(
            CurrentAggressiveness + AggressivenessIncreaseRate * Time.deltaTime,
            BaseAggressiveness,
            MaxAggressiveness
        );

        checkTimer += Time.deltaTime;

        if (checkTimer >= CheckInterval)
        {
            Debug.Log("State Check");
            checkTimer = 0;
            HandleStates();
        }

        if (CurrentState == EnemyState.Attacking)
        {
            HandleAttack();
        }
    }

    // ---------------------------------------------------------
    // ESTADO GENERAL
    // ---------------------------------------------------------
    private static void HandleStates()
    {
        float speedFactor = Mathf.Clamp01(DrillManager.CurrentSpeed / DrillManager.MaxSpeed);

        switch (CurrentState)
        {
            case EnemyState.Wandering:
                TryStartStalking(speedFactor);
                break;

            case EnemyState.Stalking:
                TryAttackOrReturnToWandering(speedFactor);
                break;

            case EnemyState.Attacking:
                break;
        }
    }

    // ---------------------------------------------------------
    // TRANSICIONES
    // ---------------------------------------------------------
    private static void TryStartStalking(float speedFactor)
    {
        float chance = CurrentAggressiveness * speedFactor * StalkingChanceMultiplier;

        if (UnityEngine.Random.value < chance)
        {
            CurrentState = EnemyState.Stalking;
            Debug.Log("Change to Stalking State");
            OnStalking?.Invoke();
        }
    }

    private static void TryAttackOrReturnToWandering(float speedFactor)
    {
        float attackChance = CurrentAggressiveness * speedFactor * AttackChanceMultiplier;
        float r = UnityEngine.Random.value;

        if (r < attackChance)
        {
            Debug.Log("Change to Attack State");
            StartAttack();
        }
        else if (r > 0.9f)
        {
            Debug.Log("return to wandering State");
            CurrentState = EnemyState.Wandering;
        }
    }

    // ---------------------------------------------------------
    // ATAQUE
    // ---------------------------------------------------------
    private static void StartAttack()
    {
        CurrentState = EnemyState.Attacking;
        attackTimer = 0f;
        DefenseActivated = false;

        OnAttack?.Invoke();
    }

    private static void HandleAttack()
    {
        attackTimer += Time.deltaTime;

        // Defensa activada
        if (DefenseActivated)
        {
            Debug.Log("Defence succesful");
            CurrentState = EnemyState.Wandering;
            CurrentAggressiveness *= 0.5f;
            OnAttackDefended?.Invoke();
            return;
        }

        // Jugador no reaccionó
        if (attackTimer >= AttackTimeLimit)
        {
            Debug.Log("Defence failure");
            OnAttackFailed?.Invoke();
        }
    }

    // ---------------------------------------------------------
    // LLAMADO EXTERNO DEL SISTEMA DE DEFENSA
    // ---------------------------------------------------------
    public static void ActivateDefense()
    {
        DefenseActivated = true;
    }
}
