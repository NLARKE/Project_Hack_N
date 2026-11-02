using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private StageUIManager uiManager;

    [Header("Black Core Setup")]
    [SerializeField] private GameObject blackCoreObject; // Black Core Object ကို ချိတ်ရန်

    [Header("Game States")]
    public bool isGameOver = false;
    public bool isStageCleared = false;

    private int totalTurrets; // ဇာတ်စင်ပေါ်ရှိ Turret စုစုပေါင်း အရေအတွက်

    // ကစားချိန်ကို မှတ်သားရန်
    private float matchTimer = 0f;
    private bool isTimerRunning = true;

    // စုစုပေါင်း Turret အရေအတွက်နှင့် ဖျက်ဆီးပြီး Turret အရေအတွက်
    private int initialTurrets = 0;
    private int destroyedTurretsCount = 0;

    void Update()
    {
        if (isTimerRunning && !isGameOver && !isStageCleared)
        {
            matchTimer += Time.deltaTime; // အချိန်ကို တိုးမြှင့်နေမည်
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // ဂိမ်းစစချင်း Black Core ကို သေချာ ပိတ်ထားမည်
        if (blackCoreObject != null)
        {
            blackCoreObject.SetActive(false);
        }
    }
    void Start()
    {
        // ★ TurretEnemy စခရစ်အစား "Enemy" Tag ပါသော Object များကိုသာ တိကျစွာ ရေတွက်မည်
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        totalTurrets = enemies.Length;
        initialTurrets = totalTurrets; // မူလအရေအတွက်ကို သိမ်းထားမည်

        Debug.Log($"<color=yellow>Actual Found Turrets (by Tag): {totalTurrets}</color>");

        if (blackCoreObject != null)
        {
            blackCoreObject.SetActive(false);
        }

        isTimerRunning = true;
        matchTimer = 0f;
    }
    private bool isStageOver = false;

    // Turret တစ်ခု သေဆုံးတိုင်း TurretEnemy script မှ ဤ Method ကို လှမ်းခေါ်မည်
    public void OnTurretDestroyed()
    {
        if (isGameOver || isStageCleared || isStageOver) return;

        // တစ်ကြိမ်လျှင် ၁ ခုသာ တိကျစွာ လျော့မည်
        totalTurrets--;
        destroyedTurretsCount++; // ဖျက်ဆီးပြီး Turret အရေအတွက်ကို ၁ ခု တိုးမည်
        Debug.Log( $"<color=orange>OnTurretDestroyed called! Remaining Turrets: {totalTurrets}</color>");

        // Turrets တွေ အားလုံး (၂ ခုစလုံး) ကုန်မှသာ Black Core ပေါ်မည်
        if (totalTurrets <= 0)
        {
            totalTurrets = 0;
            isStageOver = true;
            SpawnBlackCore();
        }
    }

    private void SpawnBlackCore()
    {
        // ★ Turrets အရေအတွက် 0 သို့မဟုတ် အောက်ရောက်မှသာ၊ ပြီးတော့ Black Core ကို တစ်ကြိမ်သာ ပေါ်စေမည်
        if (totalTurrets <= 0 && blackCoreObject != null && !blackCoreObject.activeSelf)
        {
            blackCoreObject.SetActive(true);
            Debug.Log("<color=cyan>All Turrets Destroyed! Black Core has appeared!</color>");
        }
    }

    // UI သို့ ပေးပို့ရန် Public Methods များ
    public float GetMatchTime()
    {
        return matchTimer;
    }

    public string GetTurretStats()
    {
        return $"{destroyedTurretsCount} / {initialTurrets}";
    }

    public void TriggerStageWin()
    {
        if (isGameOver || isStageCleared) return;

        isStageCleared = true;
        isTimerRunning = false; // အချိန်ရေတွက်ခြင်းကို ရပ်မည်
        Debug.Log("<color=green>STAGE WIN: Stage Cleared!</color>");

        // ★ လက်ရှိ ကစားနေသော Scene အမည် (သို့မဟုတ် Stage အဆင့်) ကို မူတည်ပြီး UnlockedStage ကို တိုးမြှင့်မည်
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        int currentUnlocked = PlayerPrefs.GetInt("UnlockedStage", 1);

        if (currentSceneName == "Stage1" && currentUnlocked < 2)
        {
            PlayerPrefs.SetInt("UnlockedStage", 2);
            PlayerPrefs.Save();
        }
        else if (currentSceneName == "Stage2" && currentUnlocked < 3)
        {
            PlayerPrefs.SetInt("UnlockedStage", 3); // Stage 2 ပြီးလျှင် Stage 3 ကို ဖွင့်မည်
            PlayerPrefs.Save();
        }

        Time.timeScale = 0f; // Pause gameplay

        if (uiManager != null)
        {
            uiManager.ShowWinUI(matchTimer, GetTurretStats());
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver || isStageCleared) return;

        isGameOver = true;
        Debug.Log("<color=red>GAME OVER: Player defeated!</color>");

        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowGameOverUI();
        }
    }
}