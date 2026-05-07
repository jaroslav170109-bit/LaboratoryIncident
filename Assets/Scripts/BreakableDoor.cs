using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BreakableDoor : MonoBehaviour
{
    [Header("Настройки двери")]
    public bool open = false;
    public float smooth = 1.0f;
    public float DoorOpenAngle = -90.0f;

    [Header("Настройки инструмента (Молоток)")]
    public string requiredItemName = "crowbar";
    public AudioClip lockedSound;

    [Header("Звуки")]
    public AudioSource asource;
    public AudioClip breakDoorSound; // Звук выбивания

    [Header("Взаимодействие (Интерфейс)")]
    public GameObject interactHint;   // Слот для "[E] Open"
    public GameObject lockedHint;     // Слот для текста ошибки
    public float interactDistance = 3.0f;

    [Header("Ручная ссылка на инвентарь")]
    public PlayerInventory playerInventory;

    private GameObject playerObject;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isShowingLockedHint = false;

    void Start()
    {
        asource = GetComponent<AudioSource>();

        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerInventory == null)
        {
            Debug.LogError($"На двери {gameObject.name} не назначен инвентарь в инспекторе!");
        }

        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(false);

        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, DoorOpenAngle, 0);
    }

    void Update()
    {
        // Плавное движение двери (будет двигаться только один раз при open = true)
        Quaternion targetRotation = open ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);

        // Если дверь УЖЕ выбита, скрываем UI и прекращаем проверки
        if (open || playerObject == null || playerInventory == null) return;

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
        // Блокируем взаимодействие, если дверь уже выбита
        if (open) return;

        if (playerInventory.currentItem != null)
        {
            if (playerInventory.currentItem.itemName == requiredItemName)
            {
                BreakDoor();
            }
            else
            {
                AccessDenied();
            }
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

    private void BreakDoor()
    {
        open = true; // Открываем навсегда

        if (asource != null && breakDoorSound != null)
        {
            asource.PlayOneShot(breakDoorSound);
        }

        // Прячем подсказки сразу после открытия
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(false);
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