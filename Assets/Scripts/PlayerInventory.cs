using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Настройки подбора")]
    public Transform handPosition; // Точка, где будет висеть предмет
    public float pickupRange = 3f; // Дистанция, с которой можно взять предмет

    private PickupableItem currentItem = null; // Предмет, который сейчас в руках
    private Transform playerCamera;

    void Start()
    {
        // Находим главную камеру автоматически
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // Кнопка подбора (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUp();
        }

        // Кнопка сброса (Q)
        if (Input.GetKeyDown(KeyCode.Q) && currentItem != null)
        {
            DropItem();
        }
    }

    private void TryPickUp()
    {
        // Пускаем луч из центра камеры вперед
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            // Если луч попал в объект со скриптом PickupableItem
            if (hit.collider.TryGetComponent(out PickupableItem newItem))
            {
                // Если мы уже что-то держим - сначала выкидываем старое
                if (currentItem != null)
                {
                    DropItem();
                }

                // Подбираем новое
                currentItem = newItem;
                currentItem.PickUp(handPosition);
            }
        }
    }

    private void DropItem()
    {
        currentItem.Drop(playerCamera);
        currentItem = null;
    }
}