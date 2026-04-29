using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class InventoryLockedDoor : MonoBehaviour
    {
        [Header("Настройки двери")]
        public bool open;
        public float smooth = 1.0f;
        public float DoorOpenAngle = -90.0f;

        [Header("Настройки ключа")]
        public PickupableItem requiredKey; // ПЕРЕТАЩИ СЮДА НУЖНЫЙ КЛЮЧ СО СЦЕНЫ
        public AudioClip lockedSound;

        [Header("Звуки")]
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Взаимодействие")]
        public GameObject hintText;
        public float interactDistance = 3.0f;

        private PlayerInventory playerInventory;
        private Quaternion closedRotation;
        private Quaternion openRotation;
        private bool isUnlocked = false;

        void Start()
        {
            asource = GetComponent<AudioSource>();

            // Ищем инвентарь на игроке
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerInventory = player.GetComponent<PlayerInventory>();

            if (hintText != null) hintText.SetActive(false);

            closedRotation = transform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0, DoorOpenAngle, 0);
        }

        void Update()
        {
            Quaternion targetRotation = open ? openRotation : closedRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);

            if (playerInventory == null) return;

            float dist = Vector3.Distance(transform.position, playerInventory.transform.position);

            if (dist <= interactDistance)
            {
                if (hintText != null) hintText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    TryOpen();
                }
            }
            else
            {
                if (hintText != null) hintText.SetActive(false);
            }
        }

        public void TryOpen()
        {
            // 1. Если уже открыта ключом, просто работаем
            if (isUnlocked)
            {
                ToggleDoor();
                return;
            }

            // 2. Проверяем, держит ли игрок именно тот ключ, который нужен этой двери
            if (playerInventory.currentItem != null && playerInventory.currentItem == requiredKey)
            {
                isUnlocked = true;
                ToggleDoor();
                Debug.Log("Дверь открыта подходящим ключом!");

                // Если хочешь, чтобы ключ исчез после использования, раскомментируй:
                // Destroy(requiredKey.gameObject); 
            }
            else
            {
                // Звук "Заперто", если ключа нет или он не тот
                if (asource != null && lockedSound != null)
                    asource.PlayOneShot(lockedSound);

                Debug.Log("Этот ключ не подходит или руки пусты!");
            }
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
    }
}