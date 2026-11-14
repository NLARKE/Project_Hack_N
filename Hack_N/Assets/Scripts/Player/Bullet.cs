using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    [SerializeField] private GameObject hitSparksPrefab;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after lifetime second
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime); // Bullet Up Direction
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // ထိမှန်သည့်နေရာတွင် Sparks VFX ဖန်တီးမည်
        if (hitSparksPrefab != null)
        {
            Instantiate(hitSparksPrefab, transform.position, Quaternion.identity);
        }

        if (other.CompareTag("Shield"))
        {
            Shield shield = other.GetComponent<Shield>();
            if (shield != null)
            {
                shield.TakeDamage(1);
            }
            Destroy(gameObject); // Destroy bullet on impact with shield
        }
        else if (other.CompareTag("BlackCore"))
        {
            BlackCore core = other.GetComponent<BlackCore>();
            if (core != null)
            {
                core.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            // ★ TurretEnemy script ရှိမရှိ စစ်ဆေးပြီး Damage ပေးမည်
            TurretEnemy enemy = other.GetComponent<TurretEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // HP ၁ မှတ်စီ လျော့မည်
            }

            Destroy(gameObject); // ကျည်ဆန်ကို ဖျက်မည်
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);       // Destroy Bullet on Wall
        }
    }
}