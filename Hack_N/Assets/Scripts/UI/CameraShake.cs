using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Camera ၏ မူလ Position ကို သိမ်းဆည်းထားမည်
        originalPos = transform.localPosition;
    }

    // Player Damage ထိချိန်တွင် သေးငယ်သော Shake ဖြစ်ပေါ်စေမည့် Method
    public void ShakeNotice(float duration = 0.1f, float magnitude = 0.05f)
    {
        StopAllCoroutines();
        // Camera ကို မူလနေရာသို့ ပြန်ပို့ပြီးမှ Shake အသစ်စမည်
        transform.localPosition = originalPos;
        StartCoroutine(DoShakeNotice(duration, magnitude));
    }

    private IEnumerator DoShakeNotice(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Shake ပြီးသွားပါက မူလနေရာသို့ အတိအကျ ပြန်ထားမည်
        transform.localPosition = originalPos;
    }
}