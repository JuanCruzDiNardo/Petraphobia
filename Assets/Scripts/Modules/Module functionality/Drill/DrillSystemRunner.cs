using UnityEngine;

public class DrillSystemRunner : MonoBehaviour
{
    void Start()
    {
        DrillManager.Initialize();

        // Energía → Taladro
        EnergyManager.OnOverload += DrillManager.ForceStop;

        // Energía restaurada → taladro en 0
        EnergyManager.OnRestored += DrillManager.ForceStop;

        // Enemigo → Energía
        EnemyManager.OnAttackDefended += EnergyManager.ForceOverload;
    }

    void Update()
    {
        DrillManager.Tick(Time.deltaTime);
    }
}
