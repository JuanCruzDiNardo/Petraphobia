using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Update()
    {
        EnemyManager.Update();

        if (ConsoleInput.active == true) return;

        if (Input.GetKeyDown(KeyCode.O))  // O = Overload
            EnergyManager.ForceOverload();

        if (Input.GetKeyDown(KeyCode.R))  // R = Restore
            EnergyManager.RestoreEnergy();

        if (Input.GetKeyDown(KeyCode.X))  // R = Restore
            EnemyManager.ActivateDefense();        
    }
}
