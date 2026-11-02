using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BlackCore : MonoBehaviour
{
    [Header("Core Health Settings")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // Player ကို လိုက်မည့် အမြန်နှုန်း (ဖြည်းဖြည်းချင်းဖြစ်ရန် တန်ဖိုးငယ်ရန်)
    private Transform playerTransform;

    [Header("Phase 2 Setup (Shield & Laser)")]
    [SerializeField] private GameObject bossShieldObject; // Black Core ရဲ့ Shield Object ကို ချိတ်ရန်
    [SerializeField] private GameObject overpowerLaserPrefab; // Overpower Laser Prefab
    [SerializeField] private Transform firePoint;
    private bool isPhase2Active = false; // Phase 2 ကို တစ်ကြိမ်သာ လုပ်ဆောင်ရန်

    [Header("VFX & Audio Feedback")]
    [SerializeField] private GameObject explosionVFXPrefab; // Explosion VFX Prefab
    [SerializeField] private AudioClip explosionSound;       // ပေါက်ကွဲသံ အသံဖိုင်
    private AudioSource audioSource;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;
    private Color originalColor;

    private Collider2D coreCollider;

    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        coreCollider = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ဂိမ်းစစချင်း Shield ကို ပိတ်ထားမည်
        if (bossShieldObject != null) bossShieldObject.SetActive(false);

        // ★ Player ကို ရှာဖွေမည် ("Player" Tag ဖြင့်)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // ★ Player ရှိနေမှသာ နှင့် Time.timeScale 0 မဖြစ်မှသာ ဖြည်းဖြည်းချင်း လိုက်မည်
        if (playerTransform != null && Time.timeScale > 0f && currentHealth > 0)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    public void TakeDamage(int damage)
    {
        // အကယ်၍ Shield ပွင့်နေသေးလျှင် Black Core က Damage လုံးဝ မထိခိုက်စေရန် (Optional)
        if (bossShieldObject != null && bossShieldObject.activeSelf)
        {
            Debug.Log("Shield is active! Black Core is protected!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Black Core Hit! Remaining HP : {currentHealth} / {maxHealth}");

        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashHitColor());
        }

        // ★ HP တစ်ဝက် (50%) အောက် ရောက်သွားပြီလား စစ်ဆေးခြင်း
        if (currentHealth <= maxHealth / 2 && !isPhase2Active)
        {
            TriggerPhase2();
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DestroyCoreWithDelay());
        }

    }

    // Phase 2 သို့ ပြောင်းလဲသည့် လုပ်ဆောင်ချက်
    private void TriggerPhase2()
    {
        isPhase2Active = true;
        Debug.Log("<color=red>WARNING: Black Core entered PHASE 2! Shield & Laser activated!</color>");

        // ၁။ Shield ကို ဖွင့်ပေးမည်
        if (bossShieldObject != null)
        {
            bossShieldObject.SetActive(true);
        }

        // ၂။ Overpower Laser ကို စတင်ပစ်ခတ်မည်
        StartCoroutine(FireOverpowerLaserRoutine());
    }

    // Overpower Laser ကို ဆက်တိုက် ပစ်ခတ်မည့် Coroutine
    private IEnumerator FireOverpowerLaserRoutine()
    {
        while (currentHealth > 0)
        {
            // Laser မပစ်ခင် အချက်ပြ Warning (ခဏစောင့်မည်)
            yield return new WaitForSeconds(1.5f);

            if (overpowerLaserPrefab != null)
            {
                Transform spawnPoint = firePoint != null ? firePoint : transform;
                Instantiate(overpowerLaserPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("Overpower Laser Fired!");
            }

            // နောက်တစ်ကြိမ် လေဆာမပစ်မီ ကြားကာလ (Interval)
            yield return new WaitForSeconds(3.0f);
        }
    }

    private System.Collections.IEnumerator FlashHitColor()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private IEnumerator DestroyCoreWithDelay()
    {
        // ၁။ ပေါက်ကွဲမှု Effect ကို ဖန်တီးမည်
        if (explosionVFXPrefab != null)
        {
            Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
        }

        // ၂။ ပေါက်ကွဲသံ မြည်စေမည် နှင့် ကြာချိန် (Duration) ကို တွက်ယူမည်
        float soundDuration = 1.0f; // Default fallback time
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
            soundDuration = explosionSound.length; // အသံဖိုင်၏ အရှည် (စက္ကန့်) ကို ယူခြင်း
        }

        // ၃။ Core ၏ ပုံရိပ် (Sprite) နှင့် အထိအတွေ့ (Collider) ကို ဖျောက်ထားမည်
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (coreCollider != null) coreCollider.enabled = false;
        if (bossShieldObject != null) bossShieldObject.SetActive(false); // Shield ပါ ပိတ်မည်

        Debug.Log($"Black Core Destroyed! Waiting for Explosion Sound ({soundDuration}s)...");

        // ★ ၄။ ပေါက်ကွဲသံ အဆုံးတိုင်သည်အထိ အတိအကျ စောင့်မည်
        yield return new WaitForSeconds(soundDuration);

        // ၅။ အသံပြီးမှ Win UI ကို ခေါ်ယူမည်
        if (StageManager.Instance != null)
        {
            StageManager.Instance.TriggerStageWin();
        }

        // ၆။ Black Core Object ကို လုံးဝဖျက်ဆီးမည်
        Destroy(gameObject);
    }
}