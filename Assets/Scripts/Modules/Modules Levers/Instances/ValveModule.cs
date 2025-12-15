using UnityEngine;

public class ValveModule : MonoBehaviour
{
    [Header("Valve Rotation Limits (degrees)")]
    public float minAngle = 0f;
    public float maxAngle = 1080f;

    [Header("Mouse rotation control")]
    public float mouseSensitivity = 0.3f;
    public bool invertMouse = false;

    [Header("Auto return")]
    public bool autoReturn = false;
    public float returnSpeed = 180f;

    [Header("Gauge Indicator")]
    public Transform gaugeNeedle;
    public float gaugeMinZ = -90f;
    public float gaugeMaxZ = 90f;

    [Header("State")]
    [Range(0f, 100f)]
    public float value01 = 80f; // 🔹 valor inicial deseado

    [Header("Energy Switch")]
    public EnergySwitchController linkedSwitch;

    private bool isDragging = false;
    private Vector3 lastMousePos;
    private float currentAngle;

    // ---------------------------------------------------------
    void Start()
    {
        // 🔹 Inicializar en 80%
        currentAngle = Mathf.Lerp(minAngle, maxAngle, value01 / 100f);

        ApplyAngle(currentAngle);
        UpdateValueFromAngle();
        UpdateGauge();
    }

    // ---------------------------------------------------------
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

    // ---------------------------------------------------------
    void Update()
    {
        if (isDragging)
        {
            if (linkedSwitch != null && !linkedSwitch.isOn)
            {
                isDragging = false;
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

    // ---------------------------------------------------------
    void HandleMouseRotation()
    {
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

    // ---------------------------------------------------------
    void ApplyAngle(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
    }

    void UpdateValueFromAngle()
    {
        value01 = Mathf.InverseLerp(minAngle, maxAngle, currentAngle) * 100f;
    }

    void UpdateGauge()
    {
        if (gaugeNeedle == null) return;

        float t = value01 / 100f;
        float angle = Mathf.Lerp(gaugeMinZ, gaugeMaxZ, t);
        gaugeNeedle.localRotation = Quaternion.Euler(0f, angle, 0f);
    }
}
