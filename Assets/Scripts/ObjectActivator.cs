using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [Header("Список объектов для включения")]
    public GameObject[] objectsToEnable;

    [Header("Настройки")]
    public bool destroyTriggerAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Здесь теперь точно objectsToEnable, как и в начале скрипта
            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            Debug.Log("Объекты активированы через триггер: " + gameObject.name);

            if (destroyTriggerAfterUse)
            {
                Destroy(gameObject);
            }
        }
    }
}