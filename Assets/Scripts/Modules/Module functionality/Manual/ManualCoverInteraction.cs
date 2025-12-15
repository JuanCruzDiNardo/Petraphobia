using UnityEngine;

public class ManualCoverInteraction : MonoBehaviour
{
    [Header("Cover Rotation")]
    public Vector3 closedRotation = Vector3.zero;
    public Vector3 openHoverRotation = new Vector3(-15f, 0f, 0f);
    public float rotationSpeed = 6f;

    private bool isHovered = false;    

    void Start()
    {
        closedRotation = transform.localEulerAngles;
    }

    void Update()
    {
        Vector3 targetRotation = isHovered ? openHoverRotation : closedRotation;
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.Euler(targetRotation),
            Time.deltaTime * rotationSpeed
        );
    }

    // -------------------------------
    // MOUSE EVENTS
    // -------------------------------

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    void OnMouseDown()
    {
        ManualManager.Instance.OpenManual();
    }

}
