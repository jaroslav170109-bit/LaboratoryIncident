using UnityEngine;
using System.Collections;

public class ReadDokument : MonoBehaviour
{
    public GameObject textPanel; // Панель с текстом
    public GameObject interactHint; // Подсказка "[E] Читать"
    public float interactDistance = 3.0f;

    private GameObject player; // Ссылка на игрока (с маленькой буквы по стандарту)

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (interactHint != null) interactHint.SetActive(false);
        if (textPanel != null) textPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        // Проверка дистанции
        if (dist <= interactDistance)
        {
            // Показываем подсказку, только если панель текста еще не открыта
            if (interactHint != null && !textPanel.activeSelf)
                interactHint.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Если панель уже открыта — закрываем, если закрыта — открываем
                if (textPanel.activeSelf)
                {
                    Close();
                }
                else
                {
                    Read();
                }
            }
        }
        else
        {
            // Если игрок отошел далеко — всё выключаем
            if (interactHint != null) interactHint.SetActive(false);
            if (textPanel.activeSelf) Close();
        }
    }

    public void Read()
    {
        textPanel.SetActive(true);
        if (interactHint != null) interactHint.SetActive(false);

        // Тут можно добавить отключение скрипта ходьбы игрока (PlayerMovement)
        // Time.timeScale = 0; // Или поставить игру на паузу
    }

    public void Close()
    {
        textPanel.SetActive(false);
        // Тут можно включить скрипт ходьбы обратно
    }
}