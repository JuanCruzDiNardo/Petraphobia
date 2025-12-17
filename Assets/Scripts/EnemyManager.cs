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
    public static float BaseAggressiveness = 0.01f;
    public static float CurrentAggressiveness = 0.1f;
    public static float AggressivenessIncreaseRate = 0.003f;
    public static float MaxAggressiveness = 0.7f;

    // ---------------------------------------------------------
    // TIMERS
    // ---------------------------------------------------------
    public static float CheckInterval = 5f;
    private static float checkTimer;

    public static float AttackTimeLimit = 20f;
    private static float attackTimer;

    public static bool DefenseActivated = false;

    // ---------------------------------------------------------
    // PROBABILIDADES
    // ---------------------------------------------------------
    public static float StalkingChanceMultiplier = 1.1f;
    public static float AttackChanceMultiplier = 1.4f;

    // ---------------------------------------------------------
    // EVENTOS
    // ---------------------------------------------------------
    public static event Action OnWandering;
    public static event Action OnStalking;
    public static event Action OnAttack;
    public static event Action OnPlayerDeath;
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
            //Debug.Log("State Check");
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
            ConsoleTextPrinter.Log("CRATURA OSTIL IDENTIFICADA EN LAS PROXIMIDADES....");
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
            ConsoleTextPrinter.Log("ACCION OSTIL INMINENTE DETECTADA... ACTIVE LA DESCARGA PARA DEFENDERSE...");
            StartAttack();
        }
        else if (r > 0.8f)
        {
            Debug.Log("return to wandering State");
            ConsoleTextPrinter.Log("CREATURA OSTIL ELUDIDA EXITOSAMENTE... RETOME LAS LABORES ABITUALES");
            OnWandering?.Invoke();
            CurrentState = EnemyState.Wandering;
        }
    }

    // ---------------------------------------------------------
    // ATAQUE
    // ---------------------------------------------------------
    public static void StartAttack()
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
            ConsoleTextPrinter.Instance.Clear();
            ConsoleTextPrinter.Log("AMENAZA REPELIDA CON EXITO... SOBRECARGA DE ENERGIA...");
            //ConsoleTextPrinter.Log("Amenaza repelida con exito");
            CurrentState = EnemyState.Wandering;
            CurrentAggressiveness *= 0.2f;
            OnAttackDefended?.Invoke();
            return;
        }

        // Jugador no reaccionó
        if (attackTimer >= AttackTimeLimit)
        {
            Time.timeScale = 0f;
            Debug.Log("Defence failure");
            CurrentState = EnemyState.Wandering;
            OnPlayerDeath?.Invoke();
        }

        Debug.Log("atacking in " + attackTimer.ToString() + "s / " + AttackTimeLimit + "s.");
    }

    // ---------------------------------------------------------
    // LLAMADO EXTERNO DEL SISTEMA DE DEFENSA
    // ---------------------------------------------------------
    public static void ActivateDefense()
    {
        if (CurrentState == EnemyState.Attacking)
            DefenseActivated = true;
        else
            EnergyManager.ForceOverload();

    }
}
