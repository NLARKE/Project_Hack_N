using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;

    [SerializeField] private GameObject hitSparksPrefab;

    // ★ ၁။ ဒီနေရာတွင် moveDirection Variable ကြေညာပေးရန် လိုအပ်ပါသည်
    private Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Player ဆီသို့ ဦးတည်ချက် တွက်ချက်ခြင်း
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            moveDirection = (player.transform.position - transform.position).normalized;
        }
        else
        {
            moveDirection = transform.up;
        }
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ★ ၂။ Shield ကို ထိပါက ဘာမှမလုပ်ဘဲ ဖောက်ထွက်သွားစေမည် (Return လုပ်မည်)
        if (other.CompareTag("Shield"))
        {
            return;
        }

        // အခြား Object များ (Player / Wall) ကို ထိမှန်မှသာ Sparks VFX ဖန်တီးမည်
        if (hitSparksPrefab != null)
        {
            Instantiate(hitSparksPrefab, transform.position, Quaternion.identity);
        }

        // Player ကို ထိပါက Player အား Damage ပေးပြီး ကျည်ဆန်ဖျက်မည်
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        // Wall ကို ထိပါက ကျည်ဆန်ကို ဖျက်မည်
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}