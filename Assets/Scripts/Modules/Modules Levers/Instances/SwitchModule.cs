using UnityEngine;

public class OnOffModule : MonoBehaviour
{

    [Header("Rotation Settings")]
    public float minAngle = 0f;
    public float maxAngle = 30f;
    public float rotateSpeed = 8f; // lo lento o rápido que hace la animación

    [Header("State")]
    public bool isOn = false;

    private float currentAngle;
    private float targetAngle;

    private void Start()
    {
        // Estado inicial
        currentAngle = isOn ? maxAngle : minAngle;
        targetAngle = currentAngle;
        
        transform.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);

        //Debug.Log("[OnOffModule] Start — Estado inicial: " + (isOn ? "ON" : "OFF"));
    }

    private void Update()
    {
        // Interpolar suavemente
        if (Mathf.Abs(currentAngle - targetAngle) > 0.01f)
        {
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotateSpeed);
            
            transform.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
        }
    }

    private void OnMouseDown()
    {
        //Debug.Log("[OnOffModule] Click detectado → Toggle()");
        Toggle();
    }

    public void Toggle()
    {
        isOn = !isOn;

        targetAngle = isOn ? maxAngle : minAngle;

        //Debug.Log("[OnOffModule] Nuevo estado = " + (isOn ? "ON" : "OFF"));
    }
}
