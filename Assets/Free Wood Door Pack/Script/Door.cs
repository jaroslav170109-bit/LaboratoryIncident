using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        [Header("Настройки двери")]
        public bool open;
        public float smooth = 1.0f;
        public float DoorOpenAngle = -90.0f; // Угол открытия (можно менять в инспекторе)

        [Header("Звуки")]
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Взаимодействие")]
        public GameObject hintText;
        public float interactDistance = 3.0f;

        private GameObject player;

        // Переменные для запоминания правильных углов
        private Quaternion closedRotation;
        private Quaternion openRotation;

        void Start()
        {
            asource = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");

            if (hintText != null) hintText.SetActive(false);

            // 1. ЗАПОМИНАЕМ начальное положение как "Закрыто"
            closedRotation = transform.localRotation;

            // 2. ВЫСЧИТЫВАЕМ положение "Открыто" (текущий угол + DoorOpenAngle)
            openRotation = closedRotation * Quaternion.Euler(0, DoorOpenAngle, 0);
        }

        void Update()
        {
            // ПЛАВНОЕ ДВИЖЕНИЕ
            // Если open == true, крутим к openRotation, иначе к closedRotation
            Quaternion targetRotation = open ? openRotation : closedRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);

            // ВЗАИМОДЕЙСТВИЕ
            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist <= interactDistance)
            {
                if (hintText != null) hintText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoor();
                }
            }
            else
            {
                if (hintText != null) hintText.SetActive(false);
            }
        }

        public void OpenDoor()
        {
            open = !open;

            // Защита от ошибки, если звуки не назначены
            if (asource != null)
            {
                asource.clip = open ? openDoor : closeDoor;
                if (asource.clip != null) asource.Play();
            }
        }
    }
}