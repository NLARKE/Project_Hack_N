using System.Collections;
using TMPro;
using UnityEngine;

public class StageCountdownManager : MonoBehaviour
{
    [Header("Countdown UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownTime = 3;

    void Start()
    {
        // 1. ဂိမ်းစစချင်း Player ကို မလှုပ်နိုင်အောင် လုပ်မည်
        PlayerController.canMove = false;

        // 2. Time.timeScale ကို 0 ထားမည်
        Time.timeScale = 0f;

        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        int counter = countdownTime;

        while (counter > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = counter.ToString();
            }
            yield return new WaitForSecondsRealtime(1f);
            counter--;
        }

        if (countdownText != null)
        {
            countdownText.text = "GO!";
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // ★ 3. Countdown ပြီးသွားပါပြီ - ဇာတ်စင်ပေါ်ရှိ Turret အားလုံးကို ဖွင့်ပေးမည်
        TurretController[] allTurrets = FindObjectsByType<TurretController>(FindObjectsSortMode.None);
        foreach (TurretController turret in allTurrets)
        {
            turret.SetTurretActive(true);
        }

        // ★ 4. Player ကို လှုပ်ရှားခွင့် ပြန်ပေးမည်
        PlayerController.canMove = true;

        // ★ 5. ဂိမ်းကို ပုံမှန် အလုပ်လုပ်ခွင့် ပြန်ပေးမည်
        Time.timeScale = 1f;
    }
}