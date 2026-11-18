using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private StageUIManager uiManager;

    [Header("Black Core & Warning Setup")]
    [SerializeField] private GameObject blackCoreObject; // Black Core Object ကို ချိတ်ရန်
    [SerializeField] private BossWarningManager warningManager; // Warning Manager စခရစ်ကို ချိတ်ရန်

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
    private bool isStageOver = false;

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
        // "Enemy" Tag ပါသော Object များကို ရေတွက်မည်
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        totalTurrets = enemies.Length;
        initialTurrets = totalTurrets; // မူလအရေအတွက်ကို သိမ်းထားမည်

        Debug.Log($"<color=yellow>Actual Found Turrets (by Tag): {totalTurrets}</color>");

        if (blackCoreObject != null)
        {
            blackCoreObject.SetActive(false);
        }

        // ★ ပြင်ထား - Unity 6 မှာ FindObjectOfType က deprecated ဖြစ်လို့ FindFirstObjectByType သုံးထား
        if (warningManager == null)
        {
            warningManager = FindFirstObjectByType<BossWarningManager>();
        }

        isTimerRunning = true;
        matchTimer = 0f;
    }

    // Turret တစ်ခု သေဆုံးတိုင်း TurretEnemy script မှ ဤ Method ကို ခေါ်မည်
    public void OnTurretDestroyed()
    {
        if (isGameOver || isStageCleared || isStageOver) return;

        totalTurrets--;
        destroyedTurretsCount++;
        Debug.Log($"<color=orange>OnTurretDestroyed called! Remaining Turrets: {totalTurrets}</color>");

        // Turrets အားလုံး ကုန်မှသာ Warning ပြပြီး Black Core ပေါ်မည်
        if (totalTurrets <= 0)
        {
            totalTurrets = 0;
            isStageOver = true;
            TriggerWarningAndSpawnBoss();
        }
    }

    private void TriggerWarningAndSpawnBoss()
    {
        if (warningManager != null)
        {
            Debug.Log("<color=yellow>All Turrets Destroyed! Triggering Warning...</color>");
            warningManager.TriggerBossWarning(() => {
                SpawnBlackCore();
            });
        }
        else
        {
            Debug.LogWarning("BossWarningManager is missing! Spawning BlackCore directly.");
            SpawnBlackCore();
        }
    }

    private void SpawnBlackCore()
    {
        if (blackCoreObject != null && !blackCoreObject.activeSelf)
        {
            blackCoreObject.SetActive(true);
            Debug.Log("<color=cyan>Warning finished! Black Core has appeared!</color>");
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

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedStage", 1);

        if (currentSceneName == "Stage1" && currentUnlocked < 2)
        {
            PlayerPrefs.SetInt("UnlockedStage", 2);
        }
        else if (currentSceneName == "Stage2" && currentUnlocked < 3)
        {
            PlayerPrefs.SetInt("UnlockedStage", 3);
        }

        // ★ အသစ် - Cleared မှတ်တမ်း (Stage 3 အပါအဝင်)
        PlayerPrefs.SetInt("Cleared_" + currentSceneName, 1);

        // ★ အသစ် - Best Time (အချိန်အတိုဆုံးကို သိမ်းမည်)
        string timeKey = "BestTime_" + currentSceneName;
        if (!PlayerPrefs.HasKey(timeKey) || matchTimer < PlayerPrefs.GetFloat(timeKey))
        {
            PlayerPrefs.SetFloat(timeKey, matchTimer);
        }

        PlayerPrefs.Save();

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