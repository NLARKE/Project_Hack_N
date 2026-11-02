using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [Header("Stage Select Buttons")]
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;

    void Start()
    {
        // 1. သိမ်းဆည်းထားသော UnlockedStage ကို စစ်ဆေးမည် (Default သည် 1 ဖြစ်သည်)
        int unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);

        // 2. Stage 2 ကို ဖွင့်ရန်/ပိတ်ရန် (UnlockedStage 2 သို့မဟုတ် ထို့ထက်ပိုမှ ဖွင့်မည်)
        if (stage2Button != null)
        {
            stage2Button.interactable = (unlockedStage >= 2);
        }

        // 3. Stage 3 ကို ဖွင့်ရန်/ပိတ်ရန် (UnlockedStage 3 ဖြစ်မှ ဖွင့်မည်)
        if (stage3Button != null)
        {
            stage3Button.interactable = (unlockedStage >= 3);
        }
    }

    public void LoadStage1()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Stage1");
    }

    public void LoadStage2()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Stage2");
    }

    public void LoadStage3()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Stage3");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    // (စမ်းသပ်ရန်) Progress အားလုံးကို ပြန်ဖျက်လိုပါက ဤ Method ကို ခေါ်နိုင်သည်
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All progress reset!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}