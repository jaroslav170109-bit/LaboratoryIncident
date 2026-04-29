using UnityEngine;
using System.Collections;

public class PaperObjective : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public PlayerInventory playerInventory; // Скрипт инвентаря (висит на игроке)
    public PickupableItem paperItem;        // Скрипт PickupableItem (на самой бумажке)

    [Header("Что активируем при подборе")]
    public GameObject corridorTrigger;      // Твой невидимый триггер-активатор в коридоре
    public GameObject objectiveUI;          // Текст подсказки на Canvas (например, "Отнесите документы...")
    public float uiDisplayTime = 5f;        // Сколько секунд будет висеть текст

    private bool isActivated = false;

    void Start()
    {
        // Для надежности выключаем триггер и текст на старте игры
        if (corridorTrigger != null) corridorTrigger.SetActive(false);
        if (objectiveUI != null) objectiveUI.SetActive(false);
    }

    void Update()
    {
        // Проверяем: если событие еще не срабатывало И в руках игрока именно ЭТА бумажка
        if (!isActivated && playerInventory != null && playerInventory.currentItem == paperItem)
        {
            isActivated = true; // Запоминаем, что событие сработало, чтобы не спамить
            StartCoroutine(TriggerObjectiveSequence());
        }
    }

    IEnumerator TriggerObjectiveSequence()
    {
        // 1. Активируем триггер в коридоре (теперь игрок может в него войти)
        if (corridorTrigger != null)
            corridorTrigger.SetActive(true);

        // 2. Включаем подсказку на экране
        if (objectiveUI != null)
            objectiveUI.SetActive(true);

        // 3. Ждем несколько секунд
        yield return new WaitForSeconds(uiDisplayTime);

        // 4. Выключаем подсказку, чтобы не мешала
        if (objectiveUI != null)
            objectiveUI.SetActive(false);
    }
}