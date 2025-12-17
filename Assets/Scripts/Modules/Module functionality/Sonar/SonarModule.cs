using UnityEngine;
using System;
using System.Collections;

public class SonarModule : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light sonarLight;

    [Header("Blink Timings")]
    [SerializeField] private float stalkingBlinkInterval = 1.5f;
    [SerializeField] private float attackingBlinkInterval = 0.4f;

    [Header("Light Intensity")]
    [SerializeField] private float pulseIntensity = 4f;
    [SerializeField] private float baseIntensity = 0f;

    private Coroutine blinkRoutine;

    [Header("Energy Switch")]
    public EnergySwitchController linkedSwitch;

    // Evento público para audio u otros sistemas
    public static event Action OnSonarPing;

    private void Awake()
    {
        if (sonarLight != null)
            sonarLight.intensity = baseIntensity;
    }

    private void Update()
    {
        if (linkedSwitch != null && !linkedSwitch.isOn)
            StopSonar();
        else
            CheckState();
    }

    private void CheckState()
    {
        if(blinkRoutine == null)
        {
            if(EnemyManager.CurrentState == EnemyManager.EnemyState.Stalking) 
                HandleStalking();
            else if(EnemyManager.CurrentState == EnemyManager.EnemyState.Attacking)
                HandleAttack();
        }
    }

    private void OnEnable()
    {
        EnemyManager.OnStalking += HandleStalking;
        EnemyManager.OnAttack += HandleAttack;
        EnemyManager.OnWandering += StopSonar;
        EnemyManager.OnAttackDefended += StopSonar;
        EnemyManager.OnPlayerDeath += StopSonar;
    }

    private void OnDisable()
    {
        EnemyManager.OnStalking -= HandleStalking;
        EnemyManager.OnAttack -= HandleAttack;
        EnemyManager.OnAttackDefended -= StopSonar;
        EnemyManager.OnPlayerDeath -= StopSonar;
    }

    // =====================================================
    // EVENT HANDLERS
    // =====================================================

    private void HandleStalking()
    {
        StartBlink(stalkingBlinkInterval);
    }

    private void HandleAttack()
    {
        StartBlink(attackingBlinkInterval);
    }

    private void StopSonar()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (sonarLight != null)
            sonarLight.intensity = baseIntensity;
    }

    // =====================================================
    // BLINK CONTROL
    // =====================================================

    private void StartBlink(float interval)
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkRoutine(interval));
    }

    IEnumerator BlinkRoutine(float interval)
    {
        while (true)
        {
            // ON
            if (sonarLight != null)
                sonarLight.intensity = pulseIntensity;

            OnSonarPing?.Invoke(); // 🔊 evento para audio

            yield return new WaitForSeconds(0.1f);

            // OFF
            if (sonarLight != null)
                sonarLight.intensity = baseIntensity;

            yield return new WaitForSeconds(interval);
        }
    }
}
