using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleTextPrinter : MonoBehaviour
{
    public static ConsoleTextPrinter Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI consoleText;

    [Header("Settings")]
    public int maxLines = 12;
    public float charDelay = 0.03f;

    private Queue<string> lines = new Queue<string>();
    private Coroutine typingRoutine;

    void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /* ===============================
       PUBLIC STATIC ENTRY POINT
       =============================== */

    public static void Log(string message)
    {
        if (Instance == null)
        {
            Debug.LogWarning("ConsoleTextPrinter no inicializado.");
            return;
        }

        Instance.PrintLine(message);
    }

    /* ===============================
       INTERNAL LOGIC
       =============================== */

    public void PrintLine(string line)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        lines.Enqueue("");

        if (lines.Count > maxLines)
            lines.Dequeue();

        for (int i = 0; i < line.Length; i++)
        {
            string current = lines.Dequeue();
            current += line[i];
            lines.Enqueue(current);

            RefreshText();
            yield return new WaitForSeconds(charDelay);
        }
    }

    void RefreshText()
    {
        consoleText.text = string.Join("\n", lines.ToArray());
    }

    public void Clear()
    {
        lines.Clear();
        consoleText.text = "";
    }
}
