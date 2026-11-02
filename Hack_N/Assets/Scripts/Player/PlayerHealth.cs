using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public int CurrentHealth => currentHealth;

    [Header("Heart UI Setup")]
    [SerializeField] private GameObject[] heartIcons; // နှလုံးသား/အသက်ပြ UI Image များကို ဤနေရာသို့ Drag & Drop လုပ်ရန်

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damageAmount) // Method for player taking Damage
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"Player took {damageAmount} damage! Current Hp : {currentHealth}/{maxHealth}");

        // အသက်လျော့သွားတိုင်း Heart UI ကို အပ်ဒိတ်လုပ်မည်
        UpdateHeartsUI();

        // Camera Shake အလုပ်လုပ်ရန်
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeNotice(0.1f, 0.08f);
        }
        else
        {
            Debug.LogWarning("CameraShake.Instance is NULL! Please check Main Camera in Scene");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHeartsUI()
    {
        // အသက်အရေအတွက်အလိုက် Heart Icon များကို ပြသခြင်း သို့မဟုတ် ဖျောက်ခြင်း
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null)
            {
                if (i < currentHealth)
                {
                    heartIcons[i].SetActive(true);  // အသက်ရှိသေးရင် ပြမည်
                }
                else
                {
                    heartIcons[i].SetActive(false); // အသက်ကုန်သွားရင် ဖျောက်မည်
                }
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player Destroyed & Defeated!");

        if (StageManager.Instance != null)
        {
            StageManager.Instance.TriggerGameOver();
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        // Temporary Test Key To Give Damage To Player (T ခလုတ်နှိပ်၍ စမ်းသပ်နိုင်သည်)
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }
}