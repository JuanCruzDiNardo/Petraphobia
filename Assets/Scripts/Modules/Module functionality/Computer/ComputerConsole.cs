using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class ComputerConsole : MonoBehaviour
{
    /* ===============================
       SINGLETON
       =============================== */
    public static ComputerConsole Instance { get; private set; }

    /* ===============================
       ESTADOS
       =============================== */
    public enum State
    {
        Unlocked,
        Locked,
        NoPower
    }

    public State CurrentState { get; private set; } = State.Unlocked;

    /* ===============================
       REFS
       =============================== */
    [Header("References")]
    [SerializeField] private ConsoleInput consoleInput;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    /* ===============================
       CONFIG
       =============================== */
    [Header("Lock Settings")]
    public float lockInterval = 120f;
    public float lockTimeLimit = 60f;

    /* ===============================
       INTERNAL
       =============================== */
    private float countdown;
    private string expectedCode;
    private bool hasFocus = false;

    private readonly char[] symbols = { '♥', '♦', '♣', '♠' };

    private readonly Dictionary<char, string> symbolWords = new()
    {
        { '♥', "nuc1e0" },
        { '♦', "t350ro" },
        { '♣', "5u3r73" },
        { '♠', "am3na2a" }
    };

    /* ===============================
       UNITY
       =============================== */
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        EnergyManager.OnOverload += PowerOff;
        StartCoroutine(LockRoutine());
    }

    /* ===============================
       LOCK CYCLE
       =============================== */
    IEnumerator LockRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(lockInterval);

            if (CurrentState == State.Unlocked)
                LockComputer();
        }
    }

    void LockComputer()
    {
        if (CurrentState == State.NoPower)
            return;

        CurrentState = State.Locked;

        GenerateCode(out string symbolCode, out expectedCode);

        ConsoleTextPrinter.Instance.Clear();
        ConsoleTextPrinter.Log("SYSTEM LOCKED");
        ConsoleTextPrinter.Log($"SECURITY CODE: {symbolCode}");
        ConsoleTextPrinter.Log("CLICK KEYBOARD TO ENTER CODE");

        countdown = lockTimeLimit;
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        while (countdown > 0f && CurrentState == State.Locked)
        {
            countdown -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(countdown).ToString();
            yield return null;
        }

        if (countdown <= 0f && CurrentState == State.Locked)
        {
            ConsoleTextPrinter.Instance.Clear();
            ConsoleTextPrinter.Log("ERROR DE VALIDACION: FORZANDO REINICIO...");
            EnergyManager.ForceOverload();
        }            
    }

    /* ===============================
       INPUT FOCUS
       =============================== */
    public void FocusConsole()
    {
        //Debug.Log("hola");

        if (CurrentState != State.Locked)
            return;

        if (hasFocus == true)
        {
            hasFocus = false;
            consoleInput.EnableInput(false);
            ConsoleTextPrinter.Log("INPUT MODE DISABLED");
        }
        else
        {
            hasFocus = true;
            consoleInput.EnableInput(true);
            ConsoleTextPrinter.Log("INPUT MODE ENABLED");
        }
        //Debug.Log("adios");        
    }

    public void LoseFocus()
    {
        hasFocus = false;
        consoleInput.EnableInput(false);
    }

    /* ===============================
       VALIDATION
       =============================== */
    public void ValidateInput(string playerInput)
    {
        if (!hasFocus || CurrentState != State.Locked)
            return;

        if (playerInput.Trim() == expectedCode)
            Unlock();
        else
            ConsoleTextPrinter.Log("INVALID CODE");
    }

    void Unlock()
    {
        CurrentState = State.Unlocked;
        hasFocus = false;

        consoleInput.EnableInput(false);
        timerText.text = "";

        ConsoleTextPrinter.Log("ACCESS GRANTED");
    }

    /* ===============================
       POWER
       =============================== */
    void PowerOff()
    {
        CurrentState = State.NoPower;
        hasFocus = false;

        consoleInput.EnableInput(false);
        timerText.text = "";
               
        EnergyManager.ForceOverload();
    }

    public void OnPowerRestored()
    {
        CurrentState = State.Locked;
        LockComputer();
    }

    /* ===============================
       CODE GENERATION
       =============================== */
    void GenerateCode(out string symbolsOut, out string wordsOut)
    {
        StringBuilder s = new();
        StringBuilder w = new();

        for (int i = 0; i < 3; i++)
        {
            char symbol = symbols[Random.Range(0, symbols.Length)];

            s.Append(symbol);
            w.Append(symbolWords[symbol]);

            if (i < 2)
                w.Append(" ");
        }

        symbolsOut = s.ToString();
        wordsOut = w.ToString();
    }
}
