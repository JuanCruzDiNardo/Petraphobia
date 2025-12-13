using UnityEngine;
using TMPro;

public class ConsoleInput : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public ComputerConsole computer;

    public static bool active; //varible que bloquea los demas inputs
    private string buffer = "";

    void Update()
    {
        if (!active) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b' && buffer.Length > 0)
            {
                buffer = buffer[..^1];
            }
            else if (c == '\n' || c == '\r')
            {
                Submit();
            }
            else
            {
                buffer += c;
            }
        }

        inputText.text = buffer;
    }

    void Submit()
    {
        computer.ValidateInput(buffer);
        buffer = "";
        inputText.text = "";
    }

    public void EnableInput(bool value)
    {
        active = value;
        buffer = "";
        inputText.text = "";
    }
}
