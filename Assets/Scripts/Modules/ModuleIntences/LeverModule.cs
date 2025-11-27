using UnityEngine;

public class LeverModule : BaseModule
{
    [Header("Configuración de Palanca")]
    public bool moveOnX = true;           // Si es false, se mueve sobre Y
    public float minPosition = -0.18f;
    public float maxPosition = 0.18f;

    private float currentOffset = 0f;

    protected override void Start()
    {
        base.Start();
        currentOffset = 0f;
    }

    protected override void ProcessDrag()
    {
        // Detectar eje de movimiento del mouse
        float mouseInput = Input.GetAxis(moveOnX ? "Mouse X" : "Mouse Y");

        // Ajustar desplazamiento
        currentOffset += mouseInput * sensitivity * Time.deltaTime;

        // Limitar
        currentOffset = Mathf.Clamp(currentOffset, minPosition, maxPosition);

        // Convertir a 0–100
        moduleValue = Mathf.InverseLerp(minPosition, maxPosition, currentOffset) * 100f;

        // Aplicar movimiento
        ApplyLeverPosition();
    }

    protected override void ProcessAutoReturn()
    {
        // Volver suavemente al centro
        currentOffset = Mathf.Lerp(currentOffset, 0f, Time.deltaTime * returnSpeed);

        // Recalcular valor normalizado
        moduleValue = Mathf.InverseLerp(minPosition, maxPosition, currentOffset) * 100f;

        // Aplicar movimiento
        ApplyLeverPosition();
    }

    private void ApplyLeverPosition()
    {
        Vector3 newPosition = initialPosition;

        if (moveOnX)
            newPosition.x = initialPosition.x + currentOffset;
        else
            newPosition.y = initialPosition.y + currentOffset;

        transform.localPosition = newPosition;
    }
}
