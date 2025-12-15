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

    private Queue<string> messageQueue = new Queue<string>();
    private Queue<string> lines = new Queue<string>();

    private bool isProcessing = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /* ===============================
       STATIC ENTRY POINT
       =============================== */

    public static void Log(string message)
    {
        if (Instance == null)
        {
            Debug.LogWarning("ConsoleTextPrinter no inicializado.");
            return;
        }

        Instance.Enqueue(message);
    }

    /* ===============================
       QUEUE CONTROL
       =============================== */

    void Enqueue(string message)
    {
        messageQueue.Enqueue(message);

        if (!isProcessing)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (messageQueue.Count > 0)
        {
            string msg = messageQueue.Dequeue();

            StartNewLine();

            bool shouldType = messageQueue.Count == 0;

            if (shouldType)
                yield return TypeLine(msg);
            else
                PrintInstant(msg);
        }

        isProcessing = false;
    }

    /* ===============================
       PRINT METHODS
       =============================== */

    IEnumerator TypeLine(string message)
    {
        EnsureLineExists();

        for (int i = 0; i < message.Length; i++)
        {
            ReplaceLastLine(GetLastLine() + message[i]);
            RefreshText();
            yield return new WaitForSeconds(charDelay);
        }
    }

    void PrintInstant(string message)
    {
        EnsureLineExists();
        ReplaceLastLine(message);
        RefreshText();
    }

    /* ===============================
       LINE MANAGEMENT
       =============================== */

    void EnsureLineExists()
    {
        if (lines.Count == 0)
            lines.Enqueue("");
    }


    string GetLastLine()
    {
        if (lines.Count == 0)
            return "";

        string[] arr = lines.ToArray();
        return arr[arr.Length - 1];
    }

    void ReplaceLastLine(string newLine)
    {
        if (lines.Count == 0)
        {
            lines.Enqueue(newLine);
            return;
        }

        string[] arr = lines.ToArray();
        arr[arr.Length - 1] = newLine;

        lines.Clear();
        foreach (var l in arr)
            lines.Enqueue(l);
    }

    void RefreshText()
    {
        consoleText.text = string.Join("\n", lines.ToArray());
    }

    public void Clear()
    {
        StopAllCoroutines();
        messageQueue.Clear();
        lines.Clear();
        consoleText.text = "";
        isProcessing = false;
    }

    void StartNewLine()
    {
        lines.Enqueue("");

        if (lines.Count > maxLines)
            lines.Dequeue();
    }

}
