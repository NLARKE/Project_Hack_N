using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private int maxShieldHealth = 3;
    [SerializeField] private bool isDestructible = true;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxShieldHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!isDestructible) return;

        currentHealth -= damage;

        Debug.Log($"Shield Hit! Remaining Health: {currentHealth} / {maxShieldHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Shield destroyed!");

            // ★ ဤနေရာတွင် StageManager ကို အကြောင်းမကြားတော့ပါ (TurretEnemy ကသာ Die() တွင် အကြောင်းကြားမည်)

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }
}