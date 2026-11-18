using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "Stage1";
    [SerializeField] private string stageSelectSceneName = "StageSelect";
    [SerializeField] private string demoSceneName = "DemoScene";

    [Header("Demo Settings")]
    [SerializeField] private float idleTimeLimit = 5.0f;
    private float idleTimer = 0f;

    [Header("Boot Sequence")]
    [SerializeField] private TMP_Text bootText;
    [SerializeField] private GameObject titleObject;
    [SerializeField] private GameObject pressStartObject;
    [SerializeField]
    private string[] bootLines =
    {
        "> LOADING KERNEL... OK",
        "> MOUNTING VIRTUAL DRIVE... OK",
        "> BYPASSING FIREWALL... OK",
        "> INJECTING PAYLOAD... OK"
    };
    [SerializeField] private float charDelay = 0.03f;
    [SerializeField] private float lineDelay = 0.25f;

    [Header("Menu")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_Text[] menuItems;
    [SerializeField] private string[] menuLabels = { "START_HACK", "STAGE_SELECT", "TERMINATE" };
    [SerializeField] private Color normalColor = new Color(0f, 0.55f, 0f);
    [SerializeField] private Color selectedColor = new Color(0.2f, 1f, 0.2f);

    [Header("Transition")]
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private TMP_Text transitionText;
    [SerializeField] private float fakeLoadTime = 1.8f;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip typeClip;
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip confirmClip;
    [SerializeField] private AudioClip accessClip;

    private bool bootDone;
    private bool menuActive;
    private bool busy;
    private bool skipBoot;
    private int index;

    void Start()
    {
        Time.timeScale = 1.0f;

        titleObject.SetActive(false);
        pressStartObject.SetActive(false);
        menuPanel.SetActive(false);
        transitionPanel.SetActive(false);
        bootText.text = "";

        StartCoroutine(BootSequence());
    }

    void Update()
    {
        if (busy) return;

        var kb = Keyboard.current;
        bool confirm = kb != null && (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame);
        bool up = kb != null && (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame);
        bool down = kb != null && (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame);

        // Boot အတွင်း Space နှိပ်ရင် ကျော်မယ်
        if (!bootDone)
        {
            if (confirm) skipBoot = true;
            return; // Boot မပြီးမချင်း Demo timer မစဘူး
        }

        // Boot ပြီးပြီ၊ Menu မပေါ်သေး → Space စောင့်
        if (!menuActive)
        {
            if (confirm)
            {
                Play(confirmClip);
                ShowMenu();
                idleTimer = 0f;
                return;
            }
            HandleIdleTimer();
            return;
        }

        // Menu ပေါ်နေပြီ
        if (up) { index = (index - 1 + menuItems.Length) % menuItems.Length; Play(moveClip); RefreshMenu(); }
        if (down) { index = (index + 1) % menuItems.Length; Play(moveClip); RefreshMenu(); }
        if (confirm) { Play(confirmClip); Select(index); }

        HandleIdleTimer();
    }

    // ---------- IDLE → DEMO ----------
    void HandleIdleTimer()
    {
        bool isKeyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool isMouseMoved = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f;
        bool isMouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (isKeyboardPressed || isMouseMoved || isMouseClicked)
        {
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeLimit)
            {
                StartDemoScene();
            }
        }
    }

    // ---------- BOOT ----------
    IEnumerator BootSequence()
    {
        foreach (string line in bootLines)
        {
            foreach (char c in line)
            {
                bootText.text += c;
                if (c != ' ' && !skipBoot) PlayType();
                if (!skipBoot) yield return new WaitForSeconds(charDelay);
            }
            bootText.text += "\n";
            if (!skipBoot) yield return new WaitForSeconds(lineDelay);
        }

        titleObject.SetActive(true);
        pressStartObject.SetActive(true);
        bootDone = true;
        idleTimer = 0f;
    }

    // ---------- MENU ----------
    void ShowMenu()
    {
        pressStartObject.SetActive(false);
        menuPanel.SetActive(true);
        menuActive = true;
        index = 0;
        RefreshMenu();
    }

    void RefreshMenu()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            bool sel = (i == index);
            menuItems[i].text = (sel ? "> " : "  ") + "[ " + menuLabels[i] + " ]";
            menuItems[i].color = sel ? selectedColor : normalColor;
        }
    }

    void Select(int i)
    {
        switch (i)
        {
            case 0: StartHack(); break;
            case 1: StartCoroutine(HackTransition(stageSelectSceneName)); break;
            case 2: TerminateGame(); break;
        }
    }

    // ---------- PUBLIC (Button OnClick ကနေလည်း ခေါ်လို့ရ) ----------
    public void StartHack()
    {
        if (busy) return;
        Debug.Log("[SYSTEM]: Starting Game...");
        StartCoroutine(HackTransition(gameplaySceneName));
    }

    public void StartDemoScene()
    {
        if (busy) return;
        Debug.Log("Player is idle. Switching to Demo Scene automatically...");
        Time.timeScale = 1.0f;
        busy = true;
        SceneManager.LoadScene(demoSceneName);
    }

    public void TerminateGame()
    {
        Debug.Log("[SYSTEM]: Terminating Application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------- TRANSITION ----------
    IEnumerator HackTransition(string sceneName)
    {
        busy = true;
        Time.timeScale = 1.0f;

        menuPanel.SetActive(false);
        titleObject.SetActive(false);
        pressStartObject.SetActive(false);
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
                "> HACK IN PROGRESS...\n[" +
                new string('#', filled) + new string('-', 20 - filled) +
                "] " + Mathf.RoundToInt(p * 100) + "%";
            yield return null;
        }

        while (op.progress < 0.9f) yield return null;

        transitionText.color = Color.green;
        transitionText.text = "> ACCESS GRANTED";
        Play(accessClip);
        yield return new WaitForSeconds(0.7f);

        op.allowSceneActivation = true;
    }

    // ---------- AUDIO ----------
    void Play(AudioClip clip)
    {
        if (sfxSource != null && clip != null) sfxSource.PlayOneShot(clip);
    }

    void PlayType()
    {
        if (sfxSource == null || typeClip == null) return;
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(typeClip, 0.5f);
    }
}