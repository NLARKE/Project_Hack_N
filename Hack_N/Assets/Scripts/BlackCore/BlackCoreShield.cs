using System.Collections;
using UnityEngine;

public class BlackCoreShield : MonoBehaviour
{
    [SerializeField] private int maxShieldHealth = 3;
    [SerializeField] private bool isDestructible = true;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;
    private Color originalColor;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxShieldHealth;

        // SpriteRenderer ကို မချိတ်ထားလျှင် အလိုအလျောက် ရှာမည်
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // မူလအရောင်ကို မှတ်သားထားမည်
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isDestructible) return;

        currentHealth -= damage;
        Debug.Log($"Shield Hit! Remaining Health: {currentHealth} / {maxShieldHealth}");

        // ★ ကျည်ဆန်ထိမှန်သည့်အခါ အနီရောင် ခဏပြောင်းမည့် Visual Feedback ကို ဖွင့်မည်
        if (spriteRenderer != null)
        {
            StopAllCoroutines(); // ယခင် Flash လုပ်နေသည်များ ရှိပါက ရပ်မည်
            StartCoroutine(FlashHitColor());
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Shield destroyed!");
            gameObject.SetActive(false);
        }
    }

    // အရောင်ခဏပြောင်းပြီး မူလအရောင်သို့ ပြန်ပြောင်းမည့် Coroutine
    private IEnumerator FlashHitColor()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f); // 0.1 စက္ကန့်ကြာ အနီရောင်ပြမည်
        spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }
}