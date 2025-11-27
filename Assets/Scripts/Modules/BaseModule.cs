using UnityEngine;

public abstract class BaseModule : MonoBehaviour
{
    [Header("Interacción")]
    public float sensitivity = 1f;          // Sensibilidad general para el control
    public bool autoReturn = false;         // Si el módulo vuelve a una posición al soltar
    public float returnSpeed = 2f;          // Velocidad del retorno automático

    [Header("Estado")]
    [Range(0, 100)] public float moduleValue = 0f;   // Valor normalizado 0–100
    protected bool isDragging = false;               // Si está siendo arrastrado
    protected Vector3 initialPosition;               // Posición inicial (si aplica)
    protected Quaternion initialRotation;            // Rotación inicial (si aplica)

    [Header("Indicador opcional")]
    public Transform indicatorTransform;             // Ej: aguja, LED, etc.
    public float indicatorMin = 0f;                  // Ángulo o valor mínimo del indicador
    public float indicatorMax = 100f;                // Ángulo o valor máximo del indicador

    protected virtual void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    protected virtual void OnMouseDown()
    {
        isDragging = true;
        OnInteractionStart();
    }

    protected virtual void OnMouseUp()
    {
        isDragging = false;
        OnInteractionEnd();
    }

    protected virtual void Update()
    {
        // Lógica común
        HandleInput();
        ApplyIndicator();
    }

    /// <summary>
    /// Maneja la entrada básica y delega comportamiento al control específico.
    /// </summary>
    protected virtual void HandleInput()
    {
        if (isDragging)
        {
            ProcessDrag();
        }
        else if (autoReturn)
        {
            ProcessAutoReturn();
        }
    }

    /// <summary>
    /// Función que debe implementar cada control derivado.
    /// </summary>
    protected abstract void ProcessDrag();

    /// <summary>
    /// Retorno automático genérico, cada módulo puede sobreescribirlo.
    /// </summary>
    protected virtual void ProcessAutoReturn()
    {
        // Por defecto no hace nada. Los módulos concretos deciden cómo volver.
    }

    /// <summary>
    /// Aplica el valor del módulo al indicador visual.
    /// </summary>
    protected virtual void ApplyIndicator()
    {
        if (indicatorTransform == null) return;

        float t = moduleValue / 100f;
        float indicatorValue = Mathf.Lerp(indicatorMin, indicatorMax, t);

        // Por defecto asume rotación Z del indicador (se puede sobrescribir).
        indicatorTransform.localRotation = Quaternion.Euler(0f, 0f, indicatorValue);
    }

    /// <summary>
    /// Eventos opcionales para los hijos.
    /// </summary>
    protected virtual void OnInteractionStart() { }
    protected virtual void OnInteractionEnd() { }
}
