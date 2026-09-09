using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LockedDoor : MonoBehaviour
{
    [Header("Настройки двери")]
    public bool open;
    public float smooth = 1.0f;
    public float DoorOpenAngle = -90.0f;
    public int hasOpenedCount = 0; // Счетчик открытий двери

    [Header("Настройки ключа")]
    public string requiredItemName = "KeyCard";
    public AudioClip lockedSound;

    [Header("Звуки")]
    public AudioSource asource;
    public AudioClip openDoor, closeDoor;

    [Header("Взаимодействие (Интерфейс)")]
    public GameObject interactHint;   // Слот для "[E] Open"
    public GameObject lockedHint;     // Слот для твоего текста про стол
    public float interactDistance = 3.0f;

    [Header("Ручная ссылка на инвентарь")]
    public PlayerInventory playerInventory; // Сюда перетащи объект со скриптом инвентаря

    private GameObject playerObject; // Ссылка для расчета дистанции (по тегу)
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isShowingLockedHint = false;

    void Start()
    {
        asource = GetComponent<AudioSource>();

        // Ищем объект игрока по тегу ТОЛЬКО для расчета позиции/дистанции
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerInventory == null)
        {
            Debug.LogError($"На двери {gameObject.name} не назначен инвентарь в инспекторе!");
        }

        // Выключаем подсказки
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(false);

        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, DoorOpenAngle, 0);
    }

    void Update()
    {
        // Плавное движение двери
        Quaternion targetRotation = open ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);

        // Если объект с тегом Player или инвентарь не найдены - ничего не делаем
        if (playerObject == null || playerInventory == null) return;

        // Считаем дистанцию до объекта с тегом Player
        float dist = Vector3.Distance(transform.position, playerObject.transform.position);

        if (dist <= interactDistance)
        {
            if (!isShowingLockedHint && interactHint != null)
            {
                interactHint.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }
        else
        {
            if (interactHint != null) interactHint.SetActive(false);
            if (lockedHint != null) lockedHint.SetActive(false);
            isShowingLockedHint = false;
        }
    }

    public void TryInteract()
    {
        // Если дверь уже открыта, просто закрываем её
        if (open)
        {
            ToggleDoor();
            return;
        }

        // Проверяем: есть ли нужный ключ ИЛИ дверь уже открывалась ранее
        bool hasKey = playerInventory.currentItem != null && playerInventory.currentItem.itemName == requiredItemName;
        bool isAlreadyUnlocked = hasOpenedCount > 0;

        if (hasKey || isAlreadyUnlocked)
        {
            ToggleDoor();
            hasOpenedCount++;
        }
        else
        {
            AccessDenied();
        }
    }

    private void AccessDenied()
    {
        if (asource != null && lockedSound != null)
            asource.PlayOneShot(lockedSound);

        if (!isShowingLockedHint)
            StartCoroutine(ShowLockedHintSequence());
    }

    private void ToggleDoor()
    {
        open = !open;
        if (asource != null)
        {
            asource.clip = open ? openDoor : closeDoor;
            if (asource.clip != null) asource.Play();
        }
    }

    IEnumerator ShowLockedHintSequence()
    {
        isShowingLockedHint = true;
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(true);

        yield return new WaitForSeconds(4.0f);

        if (lockedHint != null) lockedHint.SetActive(false);
        isShowingLockedHint = false;
    }
}