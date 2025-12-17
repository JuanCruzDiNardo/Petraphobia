using UnityEngine;
using System.Collections;

public class DefenseButton : MonoBehaviour
{
    [Header("Button Movement")]
    [SerializeField] private float pressDepth = 0.02f;   // cuánto se hunde
    [SerializeField] private float pressSpeed = 10f;
    [SerializeField] private float returnSpeed = 6f;
   
    private Vector3 initialLocalPos;
    private bool pressed = false;

    [Header("Energy Switch")]
    public EnergySwitchController linkedSwitch;

    private void Start()
    {
        initialLocalPos = transform.localPosition;
        EnergyManager.OnRestored += ResetButton;
    }

    private void OnMouseDown()
    {
        if (pressed || !linkedSwitch.isOn)
            return;

        pressed = true;

        // Activar defensa
        EnemyManager.ActivateDefense();

        // Animación
        StopAllCoroutines();
        StartCoroutine(PressRoutine());
    }

    IEnumerator PressRoutine()
    {
        Vector3 pressedPos = initialLocalPos + Vector3.right * pressDepth;

        // Mover hacia atrás
        while (Vector3.Distance(transform.localPosition, pressedPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                pressedPos,
                Time.deltaTime * pressSpeed
            );
            yield return null;
        }

        // Pequeña pausa
        yield return new WaitForSeconds(0.30f);

        // Volver a la posición original
        while (Vector3.Distance(transform.localPosition, initialLocalPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                initialLocalPos,
                Time.deltaTime * returnSpeed
            );
            yield return null;
        }
    }

    // --------------------------------------------------
    // OPCIONAL: resetear si querés permitir reutilizarlo
    // --------------------------------------------------
    public void ResetButton()
    {
        pressed = false;
        transform.localPosition = initialLocalPos;
    }
}
