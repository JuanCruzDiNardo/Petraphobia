using System;
using UnityEngine;

public class SwitchModule : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float minAngle = 0f;
    public float maxAngle = 30f;
    public float rotateSpeed = 8f;

    [Header("State")]
    public bool isOn = false;

    private float currentAngle;
    private float targetAngle;

    [Header("Energy Logic")]
    public EnergySwitchController logicController;

    [Header("LED")]
    public Renderer ledRenderer;
    public Material ledOnMat;      // Verde
    public Material ledOffMat;     // Rojo
    public Material ledOverloadMat; // Negro

    public static event Action OnClick;

    private bool isInOverloadMode = false;


    // ======================================================
    //                   INITIALIZATION
    // ======================================================
    private void Start()
    {
        currentAngle = isOn ? maxAngle : minAngle;
        targetAngle = currentAngle;
        transform.localRotation = Quaternion.Euler(currentAngle, 90f, -90f);

        UpdateLED();
    }

    private void OnEnable()
    {
        EnergyManager.OnOverload += EnterOverloadMode;
        EnergyManager.OnRestored += ExitOverloadMode;
    }

    private void OnDisable()
    {
        EnergyManager.OnOverload -= EnterOverloadMode;
        EnergyManager.OnRestored -= ExitOverloadMode;
    }


    // ======================================================
    //              INPUT MANUAL (CLICK)
    // ======================================================
    private void OnMouseDown()
    {
        if (isInOverloadMode || InputBlocker.IsBlocked)
            return;  // no se puede interactuar en sobrecarga

        OnClick?.Invoke();
        ManualToggle();
    }

    public void ManualToggle()
    {
        bool requestedState = !isOn;

        logicController.SetStateFromExternal(requestedState);
        // El controller decidirá si aprueba o no el cambio.
    }


    // ======================================================
    //      MÉTODO QUE LLAMA EnergySwitchController
    // ======================================================
    public void SetStateExternally(bool state)
    {
        isOn = state;
        targetAngle = isOn ? maxAngle : minAngle;
        UpdateLED();
    }


    // ======================================================
    //              VISUAL SMOOTH ROTATION
    // ======================================================
    private void Update()
    {
        if (Mathf.Abs(currentAngle - targetAngle) > 0.01f)
        {
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotateSpeed);
            transform.localRotation = Quaternion.Euler(currentAngle, 90f, -90f);
        }
    }


    // ======================================================
    //                LED STATE LOGIC
    // ======================================================
    private void UpdateLED()
    {
        if (ledRenderer == null)
            return;

        if (isInOverloadMode)
        {
            ledRenderer.material = ledOverloadMat;
            return;
        }

        ledRenderer.material = isOn ? ledOnMat : ledOffMat;
    }


    // ======================================================
    //        OVERLOAD ENTER / EXIT HANDLING
    // ======================================================
    private void EnterOverloadMode()
    {
        isInOverloadMode = true;
        ledRenderer.material = ledOverloadMat;
    }

    private void ExitOverloadMode()
    {
        isInOverloadMode = false;
        UpdateLED(); // recupera rojo si está apagado, verde si está encendido
    }
}
