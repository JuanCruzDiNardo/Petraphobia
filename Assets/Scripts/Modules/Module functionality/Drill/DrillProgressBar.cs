using UnityEngine;
using UnityEngine.UI;

public class DrillProgressBarUI : MonoBehaviour
{
    [Header("Segmentos (10)")]
    public Image[] segments;

    private void OnEnable()
    {
        DrillManager.OnProgressChanged += UpdateBar;
    }

    private void OnDisable()
    {
        DrillManager.OnProgressChanged -= UpdateBar;
    }

    private void UpdateBar(float progress01)
    {
        int activeSegments = Mathf.RoundToInt(progress01 * segments.Length);

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].enabled = i < activeSegments;
        }
    }
}
