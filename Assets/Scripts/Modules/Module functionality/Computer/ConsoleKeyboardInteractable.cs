using System;
using UnityEngine;

public class ConsoleKeyboardInteractable : MonoBehaviour
{
    public ComputerConsole computer;

    void OnMouseDown()
    {
        Debug.Log("keyboard press");
        computer.FocusConsole();
    }
}
