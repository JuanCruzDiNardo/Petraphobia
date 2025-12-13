using Unity.VisualScripting;
using UnityEngine;

public class ValveModule : MonoBehaviour
{
    [Header("Valve Rotation Limits (degrees)")]
    public float minAngle = 0f;           // Ej: 0 grados
    public float maxAngle = 1080f;        // Ej: 3 vueltas completas = 1080 grados

    [Header("Mouse rotation control")]
    public float mouseSensitivity = 0.3f; // Ajusta la fuerza del movimiento del mouse
    public bool invertMouse = false;      // Invierte el sentido

    [Header("Auto return")]
    public bool autoReturn = false;
    public float returnSpeed = 180f;      // Grados por segundo

    [Header("Gauge Indicator")]
    public Transform gaugeNeedle;
    public float gaugeMinZ = -90f;
    public float gaugeMaxZ = 90f;

    [Header("State")]
    [Range(0f, 100f)]
    public float value01 = 0f;            // Se genera automáticamente por el ángulo

    private bool isDragging = false;
    private Vector3 lastMousePos;

    [Header("Energy Switch")]
    public EnergySwitchController linkedSwitch;


    // Ángulo REAL actual de la válvula (no el acumulado local de Unity)
    private float currentAngle;

    void Start()
    {
        // Leer rotación inicial real en Z
        currentAngle = NormalizeAngle(transform.localEulerAngles.z);
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
        ApplyAngle(currentAngle);
        UpdateValueFromAngle();
        UpdateGauge();
    }

    void OnMouseDown()
    {
        if (linkedSwitch != null && !linkedSwitch.isOn)
            return;
        
        isDragging = true;
        lastMousePos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            if (linkedSwitch != null && !linkedSwitch.isOn)
            {
                isDragging = false; // corta interacción
            }
            else
            {
                HandleMouseRotation();
            }
        }
        else if (autoReturn)
        {
            currentAngle = Mathf.MoveTowards(
                currentAngle,
                minAngle,
                returnSpeed * Time.deltaTime
            );

            ApplyAngle(currentAngle);
            UpdateValueFromAngle();
            UpdateGauge();
        }
    }


    void HandleMouseRotation()
    {
        if (linkedSwitch != null && !linkedSwitch.isOn)
            return;

        Vector3 mousePos = Input.mousePosition;
        float delta = (mousePos.x - lastMousePos.x) - (mousePos.y - lastMousePos.y);

        if (invertMouse) delta = -delta;

        currentAngle += delta * mouseSensitivity;
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        ApplyAngle(currentAngle);
        UpdateValueFromAngle();
        UpdateGauge();

        lastMousePos = mousePos;
    }


    // Aplica rotación física al objeto
    void ApplyAngle(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
    }

    // Convierte ángulo (minAngle–maxAngle) → valor 0–100
    void UpdateValueFromAngle()
    {
        value01 = Mathf.InverseLerp(minAngle, maxAngle, currentAngle) * 100f;
    }

    // Aplica rotación al indicador en base a value01
    void UpdateGauge()
    {
        if (gaugeNeedle == null) return;

        float t = value01 / 100f;
        float angle = Mathf.Lerp(gaugeMinZ, gaugeMaxZ, t);
        gaugeNeedle.localRotation = Quaternion.Euler(0f, angle, 0f);
    }

    // Normalizar ángulo evita que Unity devuelva 0–360 siempre
    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
