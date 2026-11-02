using UnityEngine;
using TMPro;

public class StageTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeRemaining = 60f; // Stage time limit in seconds
    [SerializeField] private bool timerIsRunning = true;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText; // Reference to TextMeshPro

    void Start()
    {
        timerIsRunning = true;
        UpdateTimerDisplay(timeRemaining);
    }

    void Update()
    {
        if (!timerIsRunning) return;

        if (StageManager.Instance != null && (StageManager.Instance.isStageCleared || StageManager.Instance.isGameOver))
        {
            timerIsRunning = false;
            return;
        }

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            timerIsRunning=false;
            UpdateTimerDisplay(0);

            Debug.Log("<color=red>TIME'S UP! Game Over!</color>");
            if (StageManager.Instance != null)
            {
                StageManager.Instance.TriggerGameOver();
            }
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Formats as 01:30 or 00:05
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
