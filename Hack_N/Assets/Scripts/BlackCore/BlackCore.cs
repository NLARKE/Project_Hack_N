using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlackCore : MonoBehaviour
{
    [Header("Core Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI - HP Bar Setup")]
    [SerializeField] private GameObject bossHPBarObject; // HP Bar ၏ Game Object ကို ချိတ်ရန်
    [SerializeField] private Slider bossHPBarSlider;       // Slider Component

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f; // Player ကို လိုက်မည့် အမြန်နှုန်း
    private Transform playerTransform;

    [Header("Normal Bullet Setup")]
    [SerializeField] private GameObject normalBulletPrefab; // ပုံမှန်ကျည်ဆန် Prefab
    [SerializeField] private float normalFireInterval = 1.5f; // ကျည်ဆန်ပစ်မည့် ကြားကာလ (စက္ကန့်)

    [Header("Phase 2 Setup (Shield & Laser)")]
    [SerializeField] private GameObject bossShieldObject; // Black Core ရဲ့ Shield Object
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
    private Coroutine flashCoroutine; // အရောင်ပြောင်းခြင်းကို သီးသန့်ထိန်းချုပ်ရန်

    private Collider2D coreCollider;

    void Start()
    {
        currentHealth = maxHealth;

        // BlackCore ပေါ်လာသည်နှင့် HP Bar ကို အလိုအလျောက် ပေါ်လာစေမည်
        if (bossHPBarObject != null)
        {
            bossHPBarObject.SetActive(true);
        }
        else if (bossHPBarSlider != null)
        {
            bossHPBarSlider.gameObject.SetActive(true);
        }

        // HP Bar တန်ဖိုးများကို အစပြုခြင်း
        if (bossHPBarSlider != null)
        {
            bossHPBarSlider.maxValue = maxHealth;
            bossHPBarSlider.value = currentHealth;
        }

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

        // Player ကို ရှာဖွေမည် ("Player" Tag ဖြင့်)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // ဂိမ်းစတာနဲ့ သေဆုံးတဲ့အထိ ပုံမှန်ကျည်ဆန်များကို အမြဲတမ်း ပစ်ခတ်နေမည့် Routine ကို စတင်မည်
        if (normalBulletPrefab != null)
        {
            StartCoroutine(FireNormalBulletRoutine());
        }
    }

    void Update()
    {
        // Player ရှိနေမှသာ နှင့် Time.timeScale 0 မဖြစ်မှသာ ဖြည်းဖြည်းချင်း လိုက်မည်
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
        // အကယ်၍ Shield ပွင့်နေသေးလျှင် Black Core က Damage လုံးဝ မထိခိုက်စေရန်
        if (bossShieldObject != null && bossShieldObject.activeSelf)
        {
            Debug.Log("Shield is active! Black Core is protected!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"BlackCore Hit! Remaining HP : {currentHealth} / {maxHealth}");

        // HP Bar ကို အပ်ဒိတ်လုပ်ခြင်း
        if (bossHPBarSlider != null)
        {
            bossHPBarSlider.value = currentHealth;
        }

        // အရောင်ပြောင်းခြင်းကိုသာ သီးသန့် ထိန်းချုပ်မည် (ကျည်ပစ်ခြင်း မရပ်တန့်စေရန်)
        if (spriteRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashHitColor());
        }

        // HP တစ်ဝက် (50%) အောက် ရောက်သွားပြီလား စစ်ဆေးခြင်း
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

    // ဂိမ်းစစချင်းမှ သေဆုံးသည်အထိ ပုံမှန်ကျည်ဆန်များကို အမြဲတမ်း ပစ်ခတ်နေမည့် Routine
    private IEnumerator FireNormalBulletRoutine()
    {
        while (currentHealth > 0)
        {
            if (Time.timeScale > 0f)
            {
                if (normalBulletPrefab != null)
                {
                    Transform spawnPoint = firePoint != null ? firePoint : transform;
                    Instantiate(normalBulletPrefab, spawnPoint.position, spawnPoint.rotation);
                    Debug.Log("Black Core fired a normal bullet!");
                }
            }
            yield return new WaitForSeconds(normalFireInterval);
        }
    }

    // Overpower Laser ကို မီးတောက်မှုတ်ထုတ်သလို ဆက်တိုက် မှုတ်ထုတ်မည့် Routine
    private IEnumerator FireOverpowerLaserRoutine()
    {
        while (currentHealth > 0)
        {
            yield return new WaitForSeconds(1.5f);

            float breathDuration = 2.0f;
            float timer = 0f;

            while (timer < breathDuration && currentHealth > 0)
            {
                if (overpowerLaserPrefab != null)
                {
                    int laserCount = 4;
                    float angleStep = 360f / laserCount;

                    for (int i = 0; i < laserCount; i++)
                    {
                        float currentAngle = i * angleStep;
                        Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

                        Transform spawnPoint = firePoint != null ? firePoint : transform;
                        Instantiate(overpowerLaserPrefab, spawnPoint.position, rotation);
                    }
                }

                yield return new WaitForSeconds(0.15f);
                timer += 0.15f;
            }

            Debug.Log("Black Core finished breathing fire/laser!");

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
        if (explosionVFXPrefab != null)
        {
            Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
        }

        float soundDuration = 1.0f;
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
            soundDuration = explosionSound.length;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (coreCollider != null) coreCollider.enabled = false;
        if (bossShieldObject != null) bossShieldObject.SetActive(false);

        // Boss သေဆုံးပါက HP Bar ကို ဖျောက်ထားမည်
        if (bossHPBarObject != null)
        {
            bossHPBarObject.SetActive(false);
        }
        else if (bossHPBarSlider != null)
        {
            bossHPBarSlider.gameObject.SetActive(false);
        }

        Debug.Log($"Black Core Destroyed! Waiting for Explosion Sound ({soundDuration}s)...");

        yield return new WaitForSeconds(soundDuration);

        if (StageManager.Instance != null)
        {
            StageManager.Instance.TriggerStageWin();
        }

        Destroy(gameObject);
    }
}