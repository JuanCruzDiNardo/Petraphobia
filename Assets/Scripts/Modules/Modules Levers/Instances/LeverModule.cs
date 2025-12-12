using UnityEngine;

public class LeverModule : BaseModule
{
    [Header("Configuración de Palanca")]
    public bool moveOnX = true;
    public float minPosition = -0.18f;
    public float maxPosition = 0.18f;

    private float currentOffset = 0f;

    private bool isEnabled = true;

    // Referencia al controlador del minijuego
    private LeverRestoreController restoreController;

    protected override void Start()
    {
        base.Start();
        currentOffset = 0f;

        restoreController = FindAnyObjectByType<LeverRestoreController>();
    }

    // -------------------------------------------------------------------------

    public void EnableLever(bool value)
    {
        isEnabled = value;
    }

    // -------------------------------------------------------------------------

    protected override void ProcessDrag()
    {
        if (!isEnabled) return;

        float mouseInput = Input.GetAxis(moveOnX ? "Mouse X" : "Mouse Y");

        currentOffset += mouseInput * sensitivity * Time.deltaTime;

        currentOffset = Mathf.Clamp(currentOffset, minPosition, maxPosition);

        moduleValue = Mathf.InverseLerp(minPosition, maxPosition, currentOffset) * 100f;

        ApplyLeverPosition();

        // Si llegó a 100 → registrar activación
        if (moduleValue >= 99.9f)
        {
            isEnabled = false; // bloquear hasta que vuelva a cero
            restoreController.RegisterActivation();
        }
    }

    protected override void ProcessAutoReturn()
    {
        // La palanca debe volver siempre, incluso si está deshabilitada
        currentOffset = Mathf.Lerp(currentOffset, minPosition, Time.deltaTime * returnSpeed);

        moduleValue = Mathf.InverseLerp(minPosition, maxPosition, currentOffset) * 100f;

        ApplyLeverPosition();

        // Si ya volvió a 0 → reactivar si corresponde
        if (!isEnabled && moduleValue <= 1f)
        {
            isEnabled = true;
        }
    }

    private void ApplyLeverPosition()
    {
        Vector3 newPosition = initialPosition;

        newPosition.y = initialPosition.x + currentOffset;

        transform.localPosition = newPosition;
    }
}
