using System.Collections;
using UnityEngine;

public class BossWarningManager : MonoBehaviour
{
    [Header("Warning UI Setup")]
    [SerializeField] private GameObject warningTextObject; // Warning Text GameObject ကို ချိတ်ရန်
    [SerializeField] private AudioClip warningSound;       // သတိပေးသံ (အသံဖိုင်ရှိလျှင် ထည့်ရန်)

    [Header("Audio Cut Settings")]
    [SerializeField] private float soundDurationLimit = 4.0f; // ★ အသံမည်မျှကြာအောင် (စက္ကန့်) မြည်ပြီးလျှင် ရပ်မည်ကို ဤနေရာတွင် ချိန်နိုင်သည်

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // စစချင်းတွင် Warning Text ကို ပိတ်ထားမည်
        if (warningTextObject != null)
        {
            warningTextObject.SetActive(false);
        }
    }

    // Turrets တွေ အကုန်သေဆုံးသွားတဲ့အခါ StageManager သို့မဟုတ် အခြားနေရာမှ ဤ Method ကို လှမ်းခေါ်မည်
    public void TriggerBossWarning(System.Action onWarningFinished)
    {
        StartCoroutine(ShowWarningRoutine(onWarningFinished));
    }

    private IEnumerator ShowWarningRoutine(System.Action onWarningFinished)
    {
        if (warningSound != null && audioSource != null)
        {
            // အသံကို စတင်ဖွင့်မည်
            audioSource.PlayOneShot(warningSound);

            // ★ အသံကို သတ်မှတ်ထားသော အချိန် (ဥပမာ ၂ စက္ကန့်) ပြီးသည်နှင့် ရပ်ပစ်ရန် Coroutine ကို စတင်မည်
            StartCoroutine(StopSoundAfterDelay(soundDurationLimit));
        }

        if (warningTextObject != null)
        {
            warningTextObject.SetActive(true);
        }

        // Warning စာတန်း ပေါ်နေမည့် ကြာချိန် (ဥပမာ ၄ စက္ကန့်)
        float duration = 4.0f;
        float timer = 0f;

        CanvasGroup canvasGroup = warningTextObject != null ? warningTextObject.GetComponent<CanvasGroup>() : null;
        if (canvasGroup == null && warningTextObject != null)
        {
            canvasGroup = warningTextObject.AddComponent<CanvasGroup>();
        }

        while (timer < duration)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.PingPong(Time.time * 5f, 1f);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (warningTextObject != null)
        {
            warningTextObject.SetActive(false);
        }

        // Warning ပြီးဆုံးမှသာ Boss ပေါ်လာမည်
        onWarningFinished?.Invoke();
    }

    // ★ သတ်မှတ်ထားသော စက္ကန့်ပြည့်ပါက အသံကို ရပ်တန့်ပေးမည့် Routine
    private IEnumerator StopSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // အသံကို ချက်ချင်း ရပ်ပစ်မည်
        }
    }
}