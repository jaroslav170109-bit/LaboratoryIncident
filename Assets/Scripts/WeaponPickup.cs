using UnityEngine;

public class WeaponPickup : Sounds
{
    private Transform player;
    private bool isPickedUp = false;

    [Header("Настройки взаимодействия")]
    public float interactDistance = 3f;
    public GameObject hintText; // Текст "Нажми E чтобы подобрать"

    [Header("Что активируем")]
    public GameObject[] objectsToActivate; // Сюда кидай Пистолет в руках и Текст с патронами

    void Start()
    {
        // Ищем игрока по тегу
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

    private void Pickup()
    {
        isPickedUp = true;

        // 1. Играем звук (индекс 0)
        if (sounds.Length > 0 && sounds[0] != null) PlaySound(sounds[0]);

        // 2. ВКЛЮЧАЕМ объекты из списка (Пистолет в руках и UI)
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }

        // 3. Убираем подсказку
        if (hintText != null) hintText.SetActive(false);

        // 4. Прячем модельку на полу и удаляем её
        if (TryGetComponent<MeshRenderer>(out var mr)) mr.enabled = false;
        if (TryGetComponent<Collider>(out var col)) col.enabled = false;

        Debug.Log("Оружие подобрано!");
        Destroy(gameObject, 1.5f); // Удаляем через время, чтобы звук доиграл
    }
}