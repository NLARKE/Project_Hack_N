using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageUnlockManager : MonoBehaviour
{
    [Header("Stage Buttons")]
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;

    void Start()
    {
        // 1. PlayerPrefs မှ အရင်က ေကျာ်ဖြတ်ထားသော Stage အဆင့်ကို စစ်ဆေးမည် (Default သည် Stage 1 ပြီးဆုံးပါက 2 ဖြစ်မည်)
        // unlcokedStage က 2 ဆိုလျှင် Stage 2 ပွင့်မည်၊ 3 ဆိုလျှင် Stage 3 ပါ ပွင့်မည်
        int unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);

        // 2. Stage 2 ကို ဖွင့်ရန်/ပိတ်ရန် စစ်ဆေးခြင်း
        if (stage2Button != null)
        {
            stage2Button.interactable = (unlockedStage >= 2);
        }

        // 3. Stage 3 ကို ဖွင့်ရန်/ပိတ်ရန် စစ်ဆေးခြင်း
        if (stage3Button != null)
        {
            stage3Button.interactable = (unlockedStage >= 3);
        }
    }

    // Stage 1 ကို နှိပ်သည့်အခါ
    public void PlayStage1()
    {
        SceneManager.LoadScene("Stage1"); // သင်၏ Stage 1 Scene အမည်ကို ထည့်ပါ
    }

    // Stage 2 ကို နှိပ်သည့်အခါ
    public void PlayStage2()
    {
        SceneManager.LoadScene("Stage2"); // သင်၏ Stage 2 Scene အမည်ကို ထည့်ပါ
    }

    // Stage 3 ကို နှိပ်သည့်အခါ
    public void PlayStage3()
    {
        SceneManager.LoadScene("Stage3"); // သင်၏ Stage 3 Scene အမည်ကို ထည့်ပါ
    }

    // (စမ်းသပ်ရန်) Save ချက်များကို ဖျက်လိုပါက ဒါကို သုံးနိုင်သည်
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Progress Reset!");
        // UI ကို ပြန်လည် Refresh လုပ်ရန် Scene ကို Reload လုပ်ပါ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}