using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class StageSelectTerminal : MonoBehaviour
{
    [System.Serializable]
    public class StageData
    {
        public string displayName = "FIREWALL_BREACH";
        public string sceneName = "Stage1";
        [Range(1, 5)] public int difficulty = 1;
        public string enemies = "TURRET";
    }

    [Header("Stages (Element 0 = Stage 1, Element 1 = Stage 2, ...)")]
    [SerializeField] private StageData[] stages;
    [SerializeField] private string backSceneName = "MainMenu";

    [Header("UI")]
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text[] stageItems; // stages အရေအတွက် + BackItem
    [SerializeField] private TMP_Text infoText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0f, 0.55f, 0f);
    [SerializeField] private Color selectedColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color lockedColor = new Color(0.35f, 0.35f, 0.35f);

    [Header("Intro")]
    [SerializeField] private string headerString = "> SELECT_TARGET:";
    [SerializeField] private float charDelay = 0.04f;
    [SerializeField] private float itemDelay = 0.12f;

    [Header("Transition")]
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private TMP_Text transitionText;
    [SerializeField] private float fakeLoadTime = 1.5f;

    private int index;
    private bool ready;
    private bool busy;

    void Start()
    {
        Time.timeScale = 1f;
        transitionPanel.SetActive(false);
        infoText.text = "";
        headerText.text = "";

        foreach (var item in stageItems) item.gameObject.SetActive(false);

        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        foreach (char c in headerString)
        {
            headerText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        RefreshAll();

        int count = Mathf.Min(stages.Length + 1, stageItems.Length);
        for (int i = 0; i < count; i++)
        {
            stageItems[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(itemDelay);
        }

        ready = true;
    }

    void Update()
    {
        if (!ready || busy) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        bool up = kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame;
        bool down = kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame;
        bool confirm = kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame;
        bool back = kb.escapeKey.wasPressedThisFrame;

        int count = stages.Length + 1;

        if (up) { index = (index - 1 + count) % count; RefreshAll(); }
        if (down) { index = (index + 1) % count; RefreshAll(); }
        if (confirm) Choose(index);
        if (back) GoBack();

#if UNITY_EDITOR
        // စမ်းသပ်ရန် - F12 နှိပ်ရင် Progress အားလုံး ပြန်ဖျက်မယ် (Editor မှာပဲ အလုပ်လုပ်)
        if (kb.f12Key.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
#endif
    }

    // ---------- STATUS (StageManager နဲ့ ချိတ်ထား) ----------
    // stageIndex 0 = Stage 1
    bool IsUnlocked(int stageIndex)
    {
        int unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);
        return unlockedStage >= stageIndex + 1;
    }

    bool IsCleared(int stageIndex)
    {
        StageData s = stages[stageIndex];
        int unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);

        // StageManager က သိမ်းတဲ့ key (သို့) နောက် Stage ပွင့်နေပြီဆိုရင် ပြီးပြီလို့ သတ်မှတ်
        return PlayerPrefs.GetInt("Cleared_" + s.sceneName, 0) == 1
            || unlockedStage > stageIndex + 1;
    }

    // ---------- DISPLAY ----------
    void RefreshAll()
    {
        for (int i = 0; i < stageItems.Length && i <= stages.Length; i++)
        {
            bool sel = (i == index);
            string prefix = sel ? "> " : "  ";

            if (i < stages.Length)
            {
                StageData s = stages[i];
                bool unlocked = IsUnlocked(i);
                bool cleared = IsCleared(i);

                string status = cleared ? "<color=#00FF66>CLEARED</color>"
                              : unlocked ? "<color=#FFD400>AVAILABLE</color>"
                              : "<color=#FF3333>LOCKED</color>";

                stageItems[i].text = prefix + "[ TARGET_" + (i + 1).ToString("00") +
                                     " : " + s.displayName + " ]  " + status;
                stageItems[i].color = sel ? selectedColor : (unlocked ? normalColor : lockedColor);
            }
            else
            {
                stageItems[i].text = prefix + "[ BACK ]";
                stageItems[i].color = sel ? selectedColor : normalColor;
            }
        }

        RefreshInfo();
    }

    void RefreshInfo()
    {
        if (index >= stages.Length)
        {
            infoText.text = "RETURN TO MAIN MENU";
            return;
        }

        StageData s = stages[index];
        bool unlocked = IsUnlocked(index);
        bool cleared = IsCleared(index);

        string best = "--:--.--";
        string timeKey = "BestTime_" + s.sceneName;
        if (PlayerPrefs.HasKey(timeKey))
        {
            float t = PlayerPrefs.GetFloat(timeKey);
            int m = (int)(t / 60f);
            float sec = t - m * 60f;
            best = m.ToString("00") + ":" + sec.ToString("00.00");
        }

        string bar = new string('#', s.difficulty) + new string('-', 5 - s.difficulty);

        infoText.text =
            "TARGET: " + s.displayName + "\n\n" +
            "STATUS: " + (cleared ? "CLEARED" : unlocked ? "AVAILABLE" : "LOCKED") + "\n" +
            "DIFFICULTY: [" + bar + "]\n" +
            "BEST_TIME: " + best + "\n" +
            "ENEMIES: " + s.enemies;
    }

    // ---------- ACTIONS ----------
    void Choose(int i)
    {
        if (i >= stages.Length) { GoBack(); return; }

        if (!IsUnlocked(i))
        {
            StartCoroutine(Denied());
            return;
        }

        StageData s = stages[i];

        if (!Application.CanStreamedLevelBeLoaded(s.sceneName))
        {
            infoText.text = "<color=#FF3333>> SCENE NOT FOUND: " + s.sceneName +
                            "\n> ADD IT TO BUILD SETTINGS</color>";
            return;
        }

        StartCoroutine(HackTransition(s.sceneName, "> HACK IN PROGRESS..."));
    }

    void GoBack()
    {
        if (busy) return;
        StartCoroutine(HackTransition(backSceneName, "> DISCONNECTING..."));
    }

    IEnumerator Denied()
    {
        busy = true;
        infoText.text = "<color=#FF3333>> ACCESS DENIED\n> TARGET IS LOCKED</color>";

        RectTransform rt = contentRoot.GetComponent<RectTransform>();
        Vector2 basePos = rt.anchoredPosition;
        float end = Time.time + 0.35f;

        while (Time.time < end)
        {
            rt.anchoredPosition = basePos + new Vector2(
                Random.Range(-12f, 12f), Random.Range(-6f, 6f));
            yield return null;
        }

        rt.anchoredPosition = basePos;
        RefreshInfo();
        busy = false;
    }

    IEnumerator HackTransition(string sceneName, string title)
    {
        busy = true;
        Time.timeScale = 1f;

        contentRoot.SetActive(false);
        transitionPanel.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float t = 0f;
        while (t < fakeLoadTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fakeLoadTime);
            int filled = Mathf.RoundToInt(p * 20);
            transitionText.text =
                title + "\n[" +
                new string('#', filled) + new string('-', 20 - filled) +
                "] " + Mathf.RoundToInt(p * 100) + "%";
            yield return null;
        }

        while (op.progress < 0.9f) yield return null;

        transitionText.color = Color.green;
        transitionText.text = "> ACCESS GRANTED";
        yield return new WaitForSeconds(0.6f);

        op.allowSceneActivation = true;
    }
}