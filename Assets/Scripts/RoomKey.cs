using UnityEngine;

public class RoomKey : Sounds
{
    // Статическая переменная, к которой будет обращаться дверь
    public static bool hasRoomKey = false;

    private Transform player;
    private bool isPickedUp = false;

    [Header("Настройки ключа")]
    public float interactDistance = 2.5f;
    public GameObject hintText; // Объект с текстом "Взять ключ [E]"

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (hintText != null) hintText.SetActive(false);
    }

    void Update()
    {
        if (player == null || isPickedUp) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= interactDistance)
        {
            if (hintText != null) hintText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Pickup();
            }
        }
        else
        {
            if (hintText != null && hintText.activeSelf)
                hintText.SetActive(false);
        }
    }

    void Pickup()
    {
        isPickedUp = true;
        hasRoomKey = true; // Теперь у игрока есть ключ для двери

        // Звук из базового класса Sounds (индекс 0)
        if (sounds.Length > 0 && sounds[0] != null)
            PlaySound(sounds[0]);

        // Прячем ключ
        if (hintText != null) hintText.SetActive(false);

        // Отключаем визуал (Renderer) и колайдер, чтобы нельзя было взять дважды
        if (GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        Debug.Log("Обычный ключ взят!");
        Destroy(gameObject, 2f); // Удаляем через 2 сек после проигрывания звука
    }
}