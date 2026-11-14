using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelayTime = 1.0f; // ပေါ်လာပြီးချင်း စောင့်မည့်အချိန် (၃ စက္ကန့်)
    private bool canAttackAndMove = false; // တိုက်ခိုက်မှုနှင့် လှုပ်ရှားမှုကို ခွင့်ပြုချက်
    private SpriteRenderer spriteRenderer;

    [Header("Target & Firing Setup")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2.0f; // တစ်ကြိမ်နှင့် တစ်ကြိမ် ကြားချိန်

    [Header("Burst Settings (3 Bullets in a row)")]
    [SerializeField] private int bulletsPerBurst = 3;      // တစ်ကြိမ်လျှင် ကျည်ဆန် ၃ ခု
    [SerializeField] private float burstInterval = 0.12f;  // ကျည်ဆန်တစ်တောင့်နှင့် တစ်တောင့်ကြား ကြာချိန် (စက္ကန့်)

    [Header("Audio Feedback")]
    [SerializeField] private AudioClip laserSound;     // Laser ပစ်ခတ်သံ (၁ ချက်တည်း မြည်ရန်)
    private AudioSource audioSource;

    private Transform playerTransform;
    private float nextFireTime = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // စစချင်း ပေါ်လာချိန်မှာ မမြင်ရအောင် သို့မဟုတ် ပွင့်လာအောင် (Fade-in effect ပြုလုပ်ရန်)
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0f; // အစတွင် လုံးဝ မမြင်ရအောင် (Alpha 0) ထားမည်
            spriteRenderer.color = color;
        }

        // ၃ စက္ကန့် စောင့်မည့် Routine ကို စတင်မည်
        StartCoroutine(SpawnRoutine());

        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            // ၁။ ကျည်ဆန်ကို ချက်ချင်း ဖျက်မည် (Bullet ကြီးက Turret ထဲမှာ ငြိပြီး ထပ်ခါထပ်ခါ Damage မဝင်စေရန်)
            Destroy(other.gameObject);

            // ၂။ ပြီးမှ Damage ပေးမည်
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Enemy hit! Remaining Hp : {currentHealth}/{maxHealth}");

        // ★ HP 0 သို့မဟုတ် အောက်ရောက်ပြီလား အသေအချာ စစ်ဆေးရန်
        if (currentHealth <= 0)
        {
            Debug.LogError(">>> CURRENT HEALTH IS 0, CALLING DIE() NOW! <<<");
            Die();
        }
    }

    private bool isDead = false; // တစ်ကြိမ်သာ သေဆုံးကြောင်း မှတ်သားရန်

    private void Die()
    {
        if (isDead) return; // အကယ်၍ သေပြီးသားဆိုလျှင် ထပ်မဝင်စေရန် တားဆီးခြင်း
        isDead = true;

        Debug.Log("Enemy Destroyed!");

        // Turret တစ်ခု သေဆုံးကြောင်း StageManager သို့ တစ်ကြိမ်တည်းသာ အကြောင်းကြားမည်
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnTurretDestroyed();
        }

        Destroy(gameObject);
    }

    void Update()
    {

        // အကယ်၍ ၃ စက္ကန့် မပြည့်သေးလျှင် အောက်ပါ တိုက်ခိုက်ခြင်း/လှုပ်ရှားခြင်းများကို လုံးဝ မလုပ်ဆောင်ပါ
        if (!canAttackAndMove) return;

        if (playerTransform == null) return;

        RotateTowardsPlayer(); // Player ကို မျက်နှာမူခြင်း

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            StartCoroutine(ShootBurstRoutine());
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator ShootBurstRoutine()
    {
        // ★ ၁။ ကျည်ဆန် ၃ ခု စပစ်မည့် အချိန်တွင် Laser အသံကို တစ်ကြိမ်တည်းသာ ထွက်စေမည်
        if (laserSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(laserSound);
        }

        // ၂။ ကျည်ဆန် ၃ တောင့်ကို ဆက်တိုက် ထုတ်ပေးမည်
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            if (enemyBulletPrefab != null)
            {
                Transform spawnPoint = firePoint != null ? firePoint : transform;
                Instantiate(enemyBulletPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // နောက်ကျည်ဆန်တစ်ခု မထွက်ခင် ခဏစောင့်မည်
            yield return new WaitForSeconds(burstInterval);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        // ၁။ အဖြည်းဖြည်းချင်း ပေါ်လာစေရန် (Fade In Effect - ဥပမာ ၁ စက္ကန့်အတွင်း ပေါ်လာမည်)
        float fadeTimer = 0f;
        float fadeDuration = 1.0f;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
            yield return null;
        }

        // ၂။ ပေါ်လာပြီးနောက် ကျန်ရှိသော အချိန်ကို ထပ်စောင့်မည် (စုစုပေါင်း ၃ စက္ကန့်ပြည့် تر အထိ)
        // ဥပမာ - Fade ဖို့ ၁ စက္ကန့်ကြာလျှင် ကျန် ၂ စက္ကန့် ဆက်စောင့်မည်
        float remainingWait = spawnDelayTime - fadeDuration;
        if (remainingWait > 0f)
        {
            yield return new WaitForSeconds(remainingWait);
        }

        // ၃။ ၃ စက္ကန့်ပြည့်သွားပါက တိုက်ခိုက်မှုနှင့် လှုပ်ရှားမှုကို စတင်ခွင့်ပြုမည်
        canAttackAndMove = true;
        Debug.Log("Turret Enemy activated and ready to attack!");
    }
}