using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StageUIManager : MonoBehaviour
{
    [Header("UI Overlay Panels")]
    [SerializeField] private GameObject resultPanel; // Main overlay background panel
    [SerializeField] private TextMeshProUGUI resultTitleText; // Text for "STAGE CLEAR" / "GAME OVER"

    [Header("Buttons Reference")]
    [SerializeField] private GameObject nextStageButton; // Next Stage ခလုတ်

    [Header("Stats UI Text")]
    [SerializeField] private TextMeshProUGUI timeText;   // အချိန်ပြမည့် Text
    [SerializeField] private TextMeshProUGUI statsText;  // Turret အရေအတွက်ပြမည့် Text

    void Start()
    {
        // Hide overlay panel at the beginning of the stage
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void ShowWinUI(float timeTaken, string turretStats)
    {
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultTitleText != null)
        {
            resultTitleText.text = "STAGE CLEAR";
            resultTitleText.color = Color.green;
        }

        // ★ အချိန်ကို မိနစ် နှင့် စက္ကန့် ပုံစံသို့ ပြောင်းလဲခြင်း (ဥပမာ - 01:25)
        int minutes = Mathf.FloorToInt(timeTaken / 60f);
        int seconds = Mathf.FloorToInt(timeTaken % 60f);

        if (timeText != null)
        {
            timeText.text = $"TIME: {minutes:00}:{seconds:00}";
        }

        // ★ ဖျက်ဆီးခဲ့သော Turret စာရင်းကို ပြသခြင်း
        if (statsText != null)
        {
            statsText.text = $"TURRETS DESTROYED: {turretStats}";
        }

        // ★ Stage Clear ဖြစ်လျှင် Next Stage ခလုတ်ကို ဖွင့်မည်
        if (nextStageButton != null)
        {
            nextStageButton.SetActive(true);
        }
    }

    public void ShowGameOverUI()
    {
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultTitleText != null)
        {
            resultTitleText.text = "GAME OVER";
            resultTitleText.color = Color.red;
        }

        // ★ Game Over ဖြစ်လျှင် Next Stage ခလုတ်ကို သေချာပေါက် ပိတ်မည်
        if (nextStageButton != null)
        {
            nextStageButton.SetActive(false);
        }
    }

    public void OnNextStageButton()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Stage1")
        {
            SceneManager.LoadScene("Stage2");
        }
        else if (currentScene == "Stage2")
        {
            SceneManager.LoadScene("Stage3");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}