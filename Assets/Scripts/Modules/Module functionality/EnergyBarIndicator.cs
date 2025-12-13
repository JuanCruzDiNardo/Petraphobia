using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnergyBarIndicator : MonoBehaviour
{
    [Header("Bloques de energía (ordenados de abajo hacia arriba)")]
    public List<Image> energyBlocks = new List<Image>();

    [Header("Parpadeo")]
    public float blinkInterval = 0.15f;
    public int blinkCount = 3;

    [Header("Restauración")]
    public float restoreDelay = 0.08f;

    private int currentActiveBlocks;
    private Coroutine energyRoutine;

    private void OnEnable()
    {
        EnergyManager.OnEnergyChanged += OnEnergyChanged;
        EnergyManager.OnOverload += HandleOverload;
        EnergyManager.OnRestored += HandleRestore;
    }

    private void OnDisable()
    {
        EnergyManager.OnEnergyChanged -= OnEnergyChanged;
        EnergyManager.OnOverload -= HandleOverload;
        EnergyManager.OnRestored -= HandleRestore;
    }

    private void Start()
    {
        currentActiveBlocks = energyBlocks.Count;
        SyncVisualsInstant();
    }

    // ---------------------------------------------------------
    // EVENTO CENTRAL
    // ---------------------------------------------------------
    private void OnEnergyChanged(float energyValue)
    {
        int targetBlocks = Mathf.Clamp(
            Mathf.CeilToInt(energyValue / 10f),
            0,
            energyBlocks.Count
        );

        if (energyRoutine != null)
            StopCoroutine(energyRoutine);

        if (targetBlocks < currentActiveBlocks)
            energyRoutine = StartCoroutine(SequentialShutdown(targetBlocks));
        else if (targetBlocks > currentActiveBlocks)
            energyRoutine = StartCoroutine(SequentialRestore(targetBlocks));
    }

    // ---------------------------------------------------------
    // APAGADO SECUENCIAL
    // ---------------------------------------------------------
    private IEnumerator SequentialShutdown(int targetBlocks)
    {
        for (int i = currentActiveBlocks - 1; i >= targetBlocks; i--)
        {
            yield return StartCoroutine(BlinkAndDisable(i));
            currentActiveBlocks--;
        }
    }

    private IEnumerator BlinkAndDisable(int index)
    {
        Image block = energyBlocks[index];

        for (int i = 0; i < blinkCount; i++)
        {
            block.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            block.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        block.enabled = false;
    }

    // ---------------------------------------------------------
    // RESTAURACIÓN SECUENCIAL
    // ---------------------------------------------------------
    private IEnumerator SequentialRestore(int targetBlocks)
    {
        for (int i = currentActiveBlocks; i < targetBlocks; i++)
        {
            energyBlocks[i].enabled = true;
            currentActiveBlocks++;
            yield return new WaitForSeconds(restoreDelay);
        }
    }

    // ---------------------------------------------------------
    // SOBRECARGA / RESTAURACIÓN TOTAL
    // ---------------------------------------------------------
    private void HandleOverload()
    {
        if (energyRoutine != null)
            StopCoroutine(energyRoutine);

        for (int i = 0; i < energyBlocks.Count; i++)
            energyBlocks[i].enabled = false;

        currentActiveBlocks = 0;
    }

    private void HandleRestore()
    {
        if (energyRoutine != null)
            StopCoroutine(energyRoutine);

        energyRoutine = StartCoroutine(SequentialRestore(energyBlocks.Count));
    }

    // ---------------------------------------------------------
    // SYNC INICIAL
    // ---------------------------------------------------------
    private void SyncVisualsInstant()
    {
        for (int i = 0; i < energyBlocks.Count; i++)
            energyBlocks[i].enabled = true;
    }
}
