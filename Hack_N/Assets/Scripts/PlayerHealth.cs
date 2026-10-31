using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 2;

    private int currentHealth;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount) // Method for player taking Damage
    {
        currentHealth -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage! Current Hp : {currentHealth}/{maxHealth}");

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

    private void Die()
    {
        // ★ Die() ထဲရှိ Camera Shake ဟောင်းကို ဖျက်လိုက်ပါသည် (TakeDamage ထဲသို့ ရွှေ့လိုက်သောကြောင့် ဖြစ်သည်)
        Debug.Log("Player Destroyed!");

        if (StageManager.Instance != null)
        {
            StageManager.Instance.TriggerGameOver();
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        // Temporary Tesk Key To Give Damage To Player
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }
}
