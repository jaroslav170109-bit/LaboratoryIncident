using UnityEngine;
namespace DoorScript
{

    [RequireComponent(typeof(AudioSource))]

    public class SimpleLockedDoor : MonoBehaviour

    {

        [Header("Настройки двери")]

        public bool open;

        public float smooth = 1.0f;

        public float DoorOpenAngle = -90.0f;


        [Header("Звуки")]

        public AudioSource asource;

        public AudioClip openDoor, closeDoor, lockedSound; // Добавил звук закрытого замка


        [Header("Взаимодействие")]

        public GameObject hintText;

        public float interactDistance = 3.0f;


        private GameObject player;

        private Quaternion closedRotation;

        private Quaternion openRotation;


        // Новая переменная, чтобы дверь знала, что её уже отперли

        private bool isUnlocked = false;


        void Start()

        {

            asource = GetComponent<AudioSource>();

            player = GameObject.FindGameObjectWithTag("Player");


            if (hintText != null) hintText.SetActive(false);


            // Запоминаем углы как в твоем рабочем коде

            closedRotation = transform.localRotation;

            openRotation = closedRotation * Quaternion.Euler(0, DoorOpenAngle, 0);

        }


        void Update()

        {

            // ПЛАВНОЕ ДВИЖЕНИЕ (ровно как в твоем исходнике)

            Quaternion targetRotation = open ? openRotation : closedRotation;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);


            if (player == null) return;


            float dist = Vector3.Distance(transform.position, player.transform.position);


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

            // Если дверь уже была открыта ключом ранее, она просто работает как обычная

            if (isUnlocked)

            {

                ToggleDoor();

                return;

            }


            // Проверка ключа

            if (RoomKey.hasRoomKey)

            {

                isUnlocked = true; // Снимаем блокировку навсегда

                RoomKey.hasRoomKey = false; // Тратим ключ

                ToggleDoor(); // Открываем

                Debug.Log("Дверь отперта ключом!");

            }

            else

            {

                // Если ключа нет - играем звук закрытой двери

                if (asource != null && lockedSound != null)

                {

                    asource.PlayOneShot(lockedSound);

                }

                Debug.Log("Дверь заперта, нужен RoomKey!");

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