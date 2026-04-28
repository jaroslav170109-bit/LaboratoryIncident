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
        // Проверка на паузу (убедись, что скрипт PauseManager у тебя уже есть)
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

        // Проигрываем звук выстрела (первый в списке)
        if (sounds != null && sounds.Length > 0 && sounds[0] != null)
            PlaySound(sounds[0]);

        onGunShoot?.Invoke();

        // Стрельба лучом (Raycast)
        Ray gunRay = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, bulletRange))
        {
            // Наносим урон, если у объекта есть компонент Entity
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
        // Проигрываем звук пустого магазина (второй в списке)
        if (sounds != null && sounds.Length > 1 && sounds[1] != null)
            PlaySound(sounds[1]);
    }
}