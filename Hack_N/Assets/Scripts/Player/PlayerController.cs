using UnityEngine;
using UnityEngine.InputSystem; // For New Input System Package

public class PlayerController : MonoBehaviour
{
    // ★ Countdown ပြီးမှ လှုပ်ရှားနိုင်ရန် Flag
    public static bool canMove = false;

    [Header("Movement & Aiming")]
    [SerializeField] private float moveSpeed = 8f; // Player Movement Speed
    [SerializeField] private Camera mainCamera;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // Transform placed at the tip of player
    [SerializeField] private float fireRate = 0.1f; // Delay seconds when firing

    [SerializeField] private AudioClip shootSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 mouseScreenPosition;

    private float nextFireTime = 0f; // Cooldown Timer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // To attach the player with Rigidbody
        if (mainCamera == null) mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ဂိမ်းစစချင်း (Stage ဝင်ချင်း) တွင် Player လုံးဝ မလှုပ်ရှားစေရန် false ပေးမည်
        canMove = false;
    }

    public void OnMove(InputValue value)    // Called PlayerInput Component
    {
        if (!canMove) return;               // Countdown ပြီးမှသာ အလုပ်လုပ်မည်
        moveInput = value.Get<Vector2>();   // Read Vector 2 input from WASD keys
    }

    public void OnLook(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();
    }

    void Update()
    {
        // Countdown မပြီးသေးလျှင် Update ထဲပါ တားဆီးမည်
        if (!canMove) return;

        RotateTowardsMouse();

        bool isHoldingLeftButton = Mouse.current != null && Mouse.current.leftButton.isPressed;

        // Only fires when button is held AND the cooldown time has passed
        if (isHoldingLeftButton && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }

        // ❌ မှတ်ချက်။ ။ ယခင်က ဤနေရာတွင် ရှိနေသော audioSource Play လိုင်းကို ဖယ်ရှားလိုက်ပါပြီ။
        // အကြောင်းမှာ Frame တိုင်း အသံထွက်နေခြင်းကို ရှောင်ရှားရန် Shoot() ထဲသို့ ရွှေ့လိုက်သောကြောင့် ဖြစ်သည်။
    }

    private void FixedUpdate()
    {
        if (!canMove) return; // Physics ဖြင့် ရွေ့လျားမှုကိုပါ တားဆီးမည်
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime); // Move player using Physics update
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null) return;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = (Vector2)mouseWorldPosition - rb.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;
        Transform spawnPoint = firePoint != null ? firePoint : transform;
        Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

        // ★ သေနတ်ပစ်ဖောက်သည့်အခါတိုင်း အသံတစ်ချက်စီ ထွက်စေရန် ဤနေရာသို့ ရွှေ့လိုက်ပါပြီ
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}