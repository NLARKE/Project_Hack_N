using UnityEngine;

public class FlameBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 1; // Player ကို ပေးမည့် ထိခိုက်မှု ပမာဏ

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    // ★ Player နဲ့ ဝင်တိုက်မိသည့်အခါ အလုပ်လုပ်မည် (Trigger သုံးထားလျှင်)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ထိမိသော Object တွင် "Player" Tag ရှိမရှိ စစ်ဆေးခြင်း
        if (collision.CompareTag("Player"))
        {
            // Player ၏ Health Script ကို ရှာဖွေပြီး Damage ပေးမည်
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // ကျည်ဆန်ကို ဖျက်ဆီးမည်
            Destroy(gameObject);
        }
    }
}