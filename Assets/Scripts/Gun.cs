using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Gun : Sounds
{
    [Header("Настройки оружия")]
    public float damage = 25f;
    public float bulletRange = 100f;
    public float fireCooldown = 0.5f;
    public bool automatic;

    [Header("Патроны")]
    public int totalAmmo = 60;
    public TextMeshProUGUI ammoDisplay;

    [Header("Редкий монстр (1/8)")]
    public RareMonster rareMonster; // Ссылка на скрипт редкого монстра
    private bool hasMonsterSpawned = false; // Чтобы спавн был только 1 раз

    [Header("Эффекты")]
    public UnityEvent onGunShoot;

    private float currentCooldown;
    private Transform playerCamera;

    void Start()
    {
        currentCooldown = 0f;
        playerCamera = Camera.main.transform;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (PauseManager.isPaused) return;
        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;

        bool isShooting = automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (isShooting && currentCooldown <= 0f)
        {
            if (totalAmmo > 0)
            {
                Shoot();
            }
            else
            {
                PlayEmptySound();
            }
        }
    }

    private void Shoot()
    {
        currentCooldown = fireCooldown;
        totalAmmo--;
        UpdateAmmoUI();

        if (sounds != null && sounds.Length > 0 && sounds[0] != null)
            PlaySound(sounds[0]);

        onGunShoot?.Invoke();

        // --- РУЛЕТКА:
        if (!hasMonsterSpawned && rareMonster != null)
        {
            int chance = Random.Range(0, 4); // Выдаст число 
            if (chance == 0) 
            {
                rareMonster.ActivateMonster();
                hasMonsterSpawned = true; // Больше не появится
            }
        }
        // ------------------------------

        Ray gunRay = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, bulletRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.Health -= damage;
            }
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoDisplay != null) ammoDisplay.text = "Ammo: " + totalAmmo.ToString();
    }

    private void PlayEmptySound()
    {
        currentCooldown = fireCooldown;
        if (sounds != null && sounds.Length > 1 && sounds[1] != null)
            PlaySound(sounds[1]);
    }
}