using System.Collections;
using UnityEngine;
using UnityEngine.UI; // ★ UI Namespace ကို သုံးရန် လိုအပ်သည်

public class BlackCoreShield : MonoBehaviour
{
    [SerializeField] private int maxShieldHealth = 3;
    [SerializeField] private bool isDestructible = true;

    [Header("UI - Shield HP Bar Setup")]
    [SerializeField] private GameObject shieldHPBarObject; // Shield HP Bar ၏ GameObject
    [SerializeField] private Slider shieldHPBarSlider;       // Shield Slider Component

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

        // ★ Shield စတင်ပေါ်လာချိန်တွင် Shield HP Bar ကို ဖွင့်ပေးပြီး တန်ဖိုးများကို အစပြုမည်
        ShowShieldHPBar();
    }

    private void ShowShieldHPBar()
    {
        if (shieldHPBarObject != null)
        {
            shieldHPBarObject.SetActive(true);
        }

        if (shieldHPBarSlider != null)
        {
            shieldHPBarSlider.maxValue = maxShieldHealth;
            shieldHPBarSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isDestructible) return;

        currentHealth -= damage;
        Debug.Log($"Shield Hit! Remaining Health: {currentHealth} / {maxShieldHealth}");

        // ★ Shield HP Bar Slider တန်ဖိုးကို အပ်ဒိတ်လုပ်မည်
        if (shieldHPBarSlider != null)
        {
            shieldHPBarSlider.value = currentHealth;
        }

        // ကျည်ဆန်ထိမှန်သည့်အခါ အနီရောင် ခဏပြောင်းမည့် Visual Feedback
        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashHitColor());
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Shield destroyed!");

            // ★ Shield ကုန်သွားပါက Shield HP Bar ကို ဖျောက်မည်
            if (shieldHPBarObject != null)
            {
                shieldHPBarObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashHitColor()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
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