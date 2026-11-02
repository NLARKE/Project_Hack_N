using UnityEngine;

public class TurretController : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers; // Turret နှင့် Child တွေပါ ယူရန်
    private Collider2D[] colliders;           // Turret နှင့် Child တွေပါ ယူရန်
    private TurretEnemy turretEnemy;

    void Awake()
    {
        // Turret ကိုယ်တိုင်ရော သူ့ရဲ့ Child Object တွေမှာပါရှိတဲ့ SpriteRenderer များနှင့် Collider များကိုပါ ရှာမည်
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
        turretEnemy = GetComponent<TurretEnemy>();

        // ဂိမ်းစစချင်း (Countdown နေစဉ်) တွင် အားလုံးကို ပိတ်ထားမည်
        SetTurretActive(false);
    }

    // Countdown ပြီးဆုံးသည့်အခါ ခေါ်ရန် Method
    public void SetTurretActive(bool isActive)
    {
        // Sprite Renderer အားလုံးကို ပိတ်/ဖွင့် လုပ်မည်
        foreach (var sr in spriteRenderers)
        {
            sr.enabled = isActive;
        }

        // Collider အားလုံးကို ပိတ်/ဖွင့် လုပ်မည်
        foreach (var col in colliders)
        {
            col.enabled = isActive;
        }

        if (turretEnemy != null)
        {
            turretEnemy.enabled = isActive;
        }
    }
}