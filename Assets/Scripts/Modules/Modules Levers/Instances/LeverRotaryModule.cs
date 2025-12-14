using Unity.VisualScripting;
using UnityEngine;

public class LeverRotaryModule : BaseModule
{
    [Header("Configuración de Rotación")]
    public float minRotation = -65f;
    public float maxRotation = 65f;
    public bool rotateOnX = false;
    public bool rotateOnY = true;
    public bool rotateOnZ = false;

    private float currentRotation;

    [Header("Dependencias")]
    public SwitchModule linkedSwitch;   // ← NUEVO: switch que habilita/deshabilita la palanca

    protected override void Start()
    {
        base.Start();
        currentRotation = 0f;
    }

    protected override void Update()
    {
        base.Update();

        // Cuando el switch está apagado, siempre vuelve a 0
        if (linkedSwitch != null && !linkedSwitch.isOn)
        {
            ForceNeutralPosition();
            return;
        }
    }

    protected override void ProcessDrag()
    {
        // ------------------------------------------------------
        // 🚫 BLOQUEO POR SWITCH
        // ------------------------------------------------------
        if (linkedSwitch != null && !linkedSwitch.isOn)
        {
            ForceNeutralPosition();
            return;
        }
        // ------------------------------------------------------

        float mouseInput = Input.GetAxis("Mouse Y");

        currentRotation += mouseInput * sensitivity * Time.deltaTime;
        currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);

        moduleValue = Mathf.InverseLerp(minRotation, maxRotation, currentRotation) * 100f;

        DrillManager.UpdateDrillControl(moduleValue);

        ApplyRotation();
    }

    protected override void ProcessAutoReturn()
    {        

        // Si está encendido → funciona normal
        currentRotation = Mathf.Lerp(currentRotation, 0f, Time.deltaTime * returnSpeed);

        DrillManager.UpdateDrillControl(moduleValue);

        ApplyRotation();
    }

    // ======================================================================================

    private void ForceNeutralPosition()
    {
        DrillManager.ForceStop();

        // Forzar rotación a 0
        currentRotation = Mathf.Lerp(currentRotation, minRotation, Time.deltaTime * returnSpeed * 2f);

        // Forzar valor a 0
        moduleValue = 0f;

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        Vector3 localEuler = transform.localEulerAngles;

        if (rotateOnX) localEuler.x = currentRotation;
        if (rotateOnY) localEuler.y = currentRotation;
        if (rotateOnZ) localEuler.z = currentRotation;

        transform.localEulerAngles = localEuler;
    }
}
