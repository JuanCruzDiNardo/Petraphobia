using UnityEngine;

public class LeverRestoreController : MonoBehaviour
{
    [Header("Referencia a la palanca")]
    public LeverModule lever;

    [Header("Valores de restauración")]
    public int requiredActivations;      // entre 2 y 7
    private int currentActivations = 0;

    [Header("Estado")]
    public bool restorationActive = false;

    private void Start()
    {
        // Suscripción a eventos del manager
        EnergyManager.OnOverload += StartRestoration;
        EnergyManager.OnRestored += ResetRestoration;
    }

    private void OnDestroy()
    {
        EnergyManager.OnOverload -= StartRestoration;
        EnergyManager.OnRestored -= ResetRestoration;
    }

    // ------------------------------------------------------------------------------------

    private void StartRestoration()
    {
        restorationActive = true;
        currentActivations = 0;

        // generar número entre 2 y 5
        requiredActivations = Random.Range(2, 6);

        Debug.Log($"🔧 Se requieren {requiredActivations} activaciones de palanca para restaurar energía.");
        ConsoleTextPrinter.Log($"Se requieren {requiredActivations} activaciones de palanca para restaurar energia.");
        lever.EnableLever(true);
    }

    // ------------------------------------------------------------------------------------

    // Lo llama LeverModule cuando llega a valor = 100
    public void RegisterActivation()
    {
        if (!restorationActive) return;

        currentActivations++;

        Debug.Log($"⚡ Activación {currentActivations}/{requiredActivations}");
        ConsoleTextPrinter.Log($"Activacion {currentActivations}/{requiredActivations}");
        if (currentActivations >= requiredActivations)
        {
            Debug.Log("⚡ Energía restaurada.");
            ConsoleTextPrinter.Instance.Clear();
            ConsoleTextPrinter.Log("Energia restaurada.");
            EnergyManager.RestoreEnergy();
        }
        else
        {
            // se desactiva hasta que vuelva a cero
            lever.EnableLever(false);
        }
    }

    // ------------------------------------------------------------------------------------

    private void ResetRestoration()
    {
        restorationActive = false;
        requiredActivations = 0;
        currentActivations = 0;

        lever.EnableLever(false);
    }
}
