using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))  // O = Overload
            EnergyManager.ForceOverload();

        if (Input.GetKeyDown(KeyCode.R))  // R = Restore
            EnergyManager.RestoreEnergy();

        if (Input.GetKeyDown(KeyCode.X))  // R = Restore
            EnemyManager.ActivateDefense();

        EnemyManager.Update();
    }
}
