using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DemoManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float restartDelay = 3.0f; // Stage Clear / Over ဖြစ်လျှင် ၃ စက္ကန့်စောင့်ရန်
    private bool isDemoFinished = false;

    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool isDemoScene = (currentSceneName == "DemoScene");

        if (isDemoScene)
        {
            // ★ ကစားသမားက Spacebar ခလုတ် (သို့မဟုတ် Escape) နှိပ်လျှင် Main Menu သို့ ချက်ချင်းထွက်ရန်
            if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame))
            {
                ReturnToMainMenu();
            }

            // DemoScene တွင် Win သို့မဟုတ် Lose ဖြစ်ခြင်းကို စစ်ဆေးရန်
            CheckDemoCompletionState();
        }
    }

    private void CheckDemoCompletionState()
    {
        if (!isDemoFinished && StageManager.Instance != null)
        {
            // Stage Clear သို့မဟုတ် Game Over ဖြစ်သွားလျှင်
            if (StageManager.Instance.isStageCleared || StageManager.Instance.isGameOver)
            {
                // Game Over ဖြစ်ပါက "GAME OVER" စာသားပေါ်လာစေရန်
                if (StageManager.Instance.isGameOver)
                {
                    StageUIManager uiManager = FindObjectOfType<StageUIManager>();
                    if (uiManager != null)
                    {
                        uiManager.ShowGameOverUI();
                    }
                }

                StartCoroutine(RestartDemoRoutine());
            }
        }
    }

    private IEnumerator RestartDemoRoutine()
    {
        isDemoFinished = true;
        Debug.Log("Demo State Ended! Restarting in " + restartDelay + " seconds...");

        yield return new WaitForSecondsRealtime(restartDelay);

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Exiting Demo to Main Menu...");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu"); // Main Menu Scene သို့ ပြန်သွားမည်
    }
}