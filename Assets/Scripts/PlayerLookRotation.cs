using UnityEngine;

public class PlayerLookRotator : MonoBehaviour
{
    [Header("Rotación")]
    public float rotationSpeed = 5f;   // Velocidad de transición suave
    private bool isRotating = false;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        HandleInput();
        SmoothRotate();
    }

    private void HandleInput()
    {
        if(ConsoleInput.active == true) return;

        if (isRotating) return;

        if (Input.GetKeyDown(KeyCode.D))
            RotateStep(90f);

        if (Input.GetKeyDown(KeyCode.A))
            RotateStep(-90f);
    }

    private void RotateStep(float angle)
    {
        targetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + angle,
            transform.eulerAngles.z
        );

        isRotating = true;
    }

    private void SmoothRotate()
    {
        if (!isRotating) return;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );

        // Cuando llega suficientemente cerca al objetivo, fijamos la rotación
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
        {
            transform.rotation = targetRotation;
            isRotating = false;
        }
    }
}
