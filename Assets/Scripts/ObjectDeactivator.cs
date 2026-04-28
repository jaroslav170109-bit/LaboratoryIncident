using UnityEngine;

public class ObjectDeactivator : MonoBehaviour
{
    [Header("Список объектов для выключения")]
    public GameObject[] objectsToDisable;

    [Header("Настройки")]
    public bool destroyTriggerAfterUse = true; // Удалить триггер после срабатывания?

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел именно игрок
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false); // Выключаем каждый объект из списка
                }
            }

            Debug.Log("Объекты деактивированы через триггер: " + gameObject.name);

            if (destroyTriggerAfterUse)
            {
                Destroy(gameObject); // Убираем сам триггер, чтобы не работал лишний раз
            }
        }
    }
}