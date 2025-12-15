using System;
using UnityEngine;

public class ConsoleKeyboardInteractable : MonoBehaviour
{
    public ComputerConsole computer;

    // Referencia al controlador del minijuego
    public EnergySwitchController linkedSwitch;

    void OnMouseDown()
    {
        if (linkedSwitch != null && !linkedSwitch.isOn)
            return;

        Debug.Log("keyboard press");
        computer.FocusConsole();
    }
}
