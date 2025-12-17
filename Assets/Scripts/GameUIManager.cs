using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private CustomMouseCursor customCursor;


    void Start()
    {
        ShowMainMenu();
        EnemyManager.OnPlayerDeath += ShowGameOver;
        DrillManager.OnDrillCompleted += ShowVictory;
    }

    // =======================
    // MENÚ PRINCIPAL
    // =======================
    public void ShowMainMenu()
    {
        
        Time.timeScale = 0f;

        Debug.Log("Show Main Menu");

        customCursor.SetActiveCursor(false);
        mainMenuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        victoryCanvas.SetActive(false);

        BlockGameplayInput(true);
    }

    public void StartGame()
    {
        Debug.Log("Start Game");

        Time.timeScale = 1f;
        BlockGameplayInput(false);
        customCursor.SetActiveCursor(true);
        mainMenuCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }

    // =======================
    // GAME OVER
    // =======================
    public void ShowGameOver()
    {
        Debug.Log("Game Over");
        BlockGameplayInput(true);
        customCursor.SetActiveCursor(false);
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game");
        BlockGameplayInput(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =======================
    // VICTORIA
    // =======================
    public void ShowVictory()
    {
        Debug.Log("Show Vicotry");
        BlockGameplayInput(true);
        //Time.timeScale = 0f;
        victoryCanvas.SetActive(true);
    }

    void BlockGameplayInput(bool block)
    {
        // BLOQUEAR INPUT GENERAL
        InputBlocker.SetBlocked(block);
    }
}
