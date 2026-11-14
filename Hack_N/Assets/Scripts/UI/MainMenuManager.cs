using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "Stage1"; // Spacebar နှိပ်လျှင် သွားမည့် Scene အမည်
    [SerializeField] private string demoSceneName = "DemoScene";    // ၅ စက္ကန့် ငြိမ်နေလျှင် သွားမည့် Demo Scene အမည်

    [Header("Demo Settings")]
    [SerializeField] private float idleTimeLimit = 5.0f; // ★ ၅ စက္ကန့် ငြိမ်နေပါက Demo သို့ သွားမည်
    private float idleTimer = 0f;

    void Update()
    {
        // ★ ၁။ ကစားသမားက Keyboard ပေါ်ရှိ Spacebar ကို နှိပ်လိုက်ခြင်း ရှိမရှိ စစ်ဆေးခြင်း
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartHack();
        }

        // ၂။ New Input System ဖြင့် Keyboard နှင့် Mouse လှုပ်ရှားမှုများကို စစ်ဆေးခြင်း (Idle Timer အတွက်)
        bool isKeyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool isMouseMoved = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f;
        bool isMouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        // ကစားသမားက လှုပ်ရှားမှု တစ်ခုခု ပြုလုပ်လျှင် Timer ကို သုည (0) သို့ ပြန်ပို့မည်
        if (isKeyboardPressed || isMouseMoved || isMouseClicked)
        {
            idleTimer = 0f;
        }
        else
        {
            // ဘာမှမလုပ်ဘဲ ငြိမ်နေပါက အချိန်ကို ရေတွက်မည်
            idleTimer += Time.deltaTime;

            // ၅ စက္ကန့် ပြည့်သွားပါက Demo Scene သို့ အလိုအလျောက် ခုန်ကူးမည်
            if (idleTimer >= idleTimeLimit)
            {
                StartDemoScene();
            }
        }
    }

    public void StartHack()
    {
        Debug.Log("[SYSTEM]: Starting Game via Spacebar...");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(gameplaySceneName); // Stage1 သို့ ဝင်မည်
    }

    public void StartDemoScene()
    {
        Debug.Log("Player is idle. Switching to Demo Scene automatically...");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(demoSceneName);
    }

    public void TerminateGame()
    {
        Debug.Log("[SYSTEM]: Terminating Application...");
        Application.Quit();
    }
}