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

    protected override void Start()
    {
        base.Start();
        currentRotation = 0f;

        //Debug.Log($"[LeverRotary] START → Min:{minRotation} Max:{maxRotation} Eje: X:{rotateOnX} Y:{rotateOnY} Z:{rotateOnZ}");
    }

    protected override void ProcessDrag()
    {
        float mouseInput = Input.GetAxis("Mouse Y");

        //Debug.Log($"[LeverRotary] DRAG → mouseInput:{mouseInput:F4}, currentRotation:{currentRotation:F2}");

        currentRotation += mouseInput * sensitivity * Time.deltaTime;
        currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);

        //Debug.Log($"[LeverRotary] After Clamp → rotation:{currentRotation:F2}");

        moduleValue = Mathf.InverseLerp(minRotation, maxRotation, currentRotation) * 100f;

        ApplyRotation();
    }

    protected override void ProcessAutoReturn()
    {
        //Debug.Log($"[LeverRotary] AUTO-RETURN → current:{currentRotation:F2}");

        currentRotation = Mathf.Lerp(currentRotation, 0f, Time.deltaTime * returnSpeed);

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        Vector3 localEuler = transform.localEulerAngles;

        if (rotateOnX) localEuler.x = currentRotation;
        if (rotateOnY) localEuler.y = currentRotation;
        if (rotateOnZ) localEuler.z = currentRotation;

        transform.localEulerAngles = localEuler;

        //Debug.Log($"[LeverRotary] APPLY ROTATION → {localEuler}");
    }
}
