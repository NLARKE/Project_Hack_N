using System.Collections;
using UnityEngine;

public class PlayerAutoController : MonoBehaviour
{
    [Header("1. Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX = 7f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;
    private int moveDirectionX = 1;
    private int moveDirectionY = 1;

    [Header("2. Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.15f;
    private Coroutine shootCoroutine;

    [Header("3. Aiming & Target Settings")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string bossTag = "BlackCore"; // ★ Tag Error မတက်စေရန် BlackCore ဟု အတိအကျ သတ်မှတ်သည်
    [SerializeField] private float aimSpeed = 20f; // ★ ချိန်ရွယ်မည့် အမြန်နှုန်း (ဂဏန်းကြီးလေ ပိုမြန်လေ ဖြစ်သည်)
    private Transform currentTarget;

    void Start()
    {
        StartShooting();
    }

    void Update()
    {
        HandleMovement();
        UpdateTargeting();
        HandleAiming();
    }

    // --- 1. Movement Logic ---
    private void HandleMovement()
    {
        transform.Translate(Vector3.right * moveDirectionX * moveSpeed * Time.deltaTime, Space.World);
        transform.Translate(Vector3.up * moveDirectionY * (moveSpeed * 0.7f) * Time.deltaTime, Space.World);

        if (transform.position.x > maxX) { moveDirectionX = -1; }
        else if (transform.position.x < minX) { moveDirectionX = 1; }

        if (transform.position.y > maxY) { moveDirectionY = -1; }
        else if (transform.position.y < minY) { moveDirectionY = 1; }
    }

    // --- 2. Targeting Logic ---
    private void UpdateTargeting()
    {
        Transform bestTarget = null;
        float shortestDistance = Mathf.Infinity;

        // Boss ကို ရှာဖွေမည်
        GameObject bossObj = GameObject.FindGameObjectWithTag(bossTag);
        if (bossObj != null)
        {
            float distToBoss = Vector2.Distance(transform.position, bossObj.transform.position);
            if (distToBoss < shortestDistance)
            {
                shortestDistance = distToBoss;
                bestTarget = bossObj.transform;
            }
        }

        // Turret / Enemy များကို ရှာဖွေမည်
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distToEnemy < shortestDistance)
                {
                    shortestDistance = distToEnemy;
                    bestTarget = enemy.transform;
                }
            }
        }

        currentTarget = bestTarget;
    }

    // --- 3. Aiming Logic (ပိုမိုမြန်ဆန်ချောမွေ့စေရန် ပြင်ဆင်ထားသည်) ---
    private void HandleAiming()
    {
        if (currentTarget != null)
        {
            Vector2 direction = currentTarget.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // မူလထောင့်မှ ပစ်မှတ်ထောင့်သို့ အလွန်မြန်ဆန်စွာ (Aim Speed အလျင်ဖြင့်) လှည့်ပေးခြင်း
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, aimSpeed * Time.deltaTime * 100f);
        }
    }

    // --- 4. Shooting Logic ---
    private void StartShooting()
    {
        if (bulletPrefab != null)
        {
            shootCoroutine = StartCoroutine(AutoShootRoutine());
        }
    }

    private IEnumerator AutoShootRoutine()
    {
        while (true)
        {
            if (bulletPrefab != null)
            {
                Transform spawnPoint = firePoint != null ? firePoint : transform;
                Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    // Demo Scene အတွက် God Mode
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("BlackCore"))
        {
            return;
        }
    }
}