using UnityEngine;
using TMPro; // Если используешь TextMeshPro

public class PlayerInventory : MonoBehaviour
{
    [Header("Настройки подбора")]
    public Transform handPosition;
    public float pickupRange = 3f;

    [Header("Интерфейс")]
    public GameObject hintObject; // Объект с текстом (например, "Нажмите E чтобы взять")
    public TextMeshProUGUI itemText; // (Опционально) чтобы менять название предмета

    public PickupableItem currentItem = null;
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        if (hintObject != null) hintObject.SetActive(false);
    }

    void Update()
    {
        // 1. Проверка луча для подсказки
        CheckForItems();

        if (Input.GetKeyDown(KeyCode.E)) TryPickUp();
        if (Input.GetKeyDown(KeyCode.Q) && currentItem != null) DropItem();
    }

    private void CheckForItems()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.TryGetComponent(out PickupableItem item))
            {
                // Если навелись на предмет
                if (hintObject != null) hintObject.SetActive(true);

                // Если хочешь динамическое название (например: "Взять Отвертку")
                if (itemText != null) itemText.text = "Взять " + item.itemName;

                return; // Выходим из метода, чтобы не выключить текст ниже
            }
        }

        // Если луч никуда не попал или попал не в предмет — выключаем текст
        if (hintObject != null) hintObject.SetActive(false);
    }

    private void TryPickUp()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.TryGetComponent(out PickupableItem newItem))
            {
                if (currentItem != null) DropItem();
                currentItem = newItem;
                currentItem.PickUp(handPosition);
                // Скрываем текст после подбора
                if (hintObject != null) hintObject.SetActive(false);
            }
        }
    }

    private void DropItem()
    {
        currentItem.Drop(playerCamera);
        currentItem = null;
    }
}