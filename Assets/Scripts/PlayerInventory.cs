using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("Настройки подбора")]
    public Transform handPosition;
    public float pickupRange = 3f;

    [Header("Интерфейс: Подсказка (при наведении)")]
    public GameObject hintObject; // Родительский объект подсказки
    public TextMeshProUGUI itemText; // Текст с названием предмета

    public PickupableItem currentItem = null;
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;

        // Скрываем подсказку при запуске
        if (hintObject != null) hintObject.SetActive(false);
    }

    void Update()
    {
        // Постоянно проверяем, куда смотрит игрок
        CheckForItems();

        // Подбор предмета
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUp();
        }

        // Выбрасывание предмета
        if (Input.GetKeyDown(KeyCode.Q) && currentItem != null)
        {
            DropItem();
        }
    }

    private void CheckForItems()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // ЛОГИКА "КАК У ДВЕРИ":
        // Если луч попал в объект И у него есть компонент PickupableItem
        if (Physics.Raycast(ray, out hit, pickupRange) && hit.collider.TryGetComponent(out PickupableItem item))
        {
            // ВКЛЮЧАЕМ текст, если мы смотрим на предмет
            if (hintObject != null)
            {
                hintObject.SetActive(true);
                if (itemText != null) itemText.text = item.itemName;
            }
        }
        else
        {
            // ВЫКЛЮЧАЕМ текст во всех остальных случаях (смотрим в стену, в пол или в небо)
            if (hintObject != null && hintObject.activeSelf)
            {
                hintObject.SetActive(false);
            }
        }
    }

    private void TryPickUp()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.TryGetComponent(out PickupableItem newItem))
            {
                // Если в руках уже что-то есть — выбрасываем
                if (currentItem != null) DropItem();

                // Берем новый предмет
                currentItem = newItem;
                currentItem.PickUp(handPosition);

                // Сразу выключаем подсказку, чтобы она не "висела" на поднятом предмете
                if (hintObject != null) hintObject.SetActive(false);
            }
        }
    }

    private void DropItem()
    {
        if (currentItem != null)
        {
            currentItem.Drop(playerCamera);
            currentItem = null;
        }
    }
}