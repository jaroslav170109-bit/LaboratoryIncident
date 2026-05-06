using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для смены сцен
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class L1Door : MonoBehaviour
{
    [Header("Настройки перехода")]
    public string nextSceneName = "Level 1.5"; // Имя сцены для загрузки
    public float interactDistance = 3.0f;

    [Header("Настройки ключа")]
    public string requiredItemName = "KeyCard"; // Название нужного предмета

    [Header("Звуки")]
    public AudioClip lockedSound;    // Звук запертой двери
    public AudioClip openDoorSound; // Звук открытия (перед загрузкой)

    [Header("Взаимодействие (Интерфейс)")]
    public GameObject interactHint;   // Слот для "[E] Open"
    public GameObject lockedHint;     // Слот для текста "Нужен ключ..."
    public GameObject loadingScreen;  // Слот для Canvas/Картинки экрана загрузки
    public float loadDelay = 2.5f;    // Задержка перед сменой сцены

    [Header("Ручная ссылка на инвентарь")]
    public PlayerInventory playerInventory; // Перетащи сюда объект с инвентарем

    // Внутренние переменные
    private AudioSource asource;
    private GameObject playerObject;
    private bool isShowingLockedHint = false;
    private bool isLoading = false; // Чтобы не нажать 'E' много раз во время загрузки

    void Start()
    {
        asource = GetComponent<AudioSource>();

        // Ищем игрока по тегу только для расчета дистанции
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerInventory == null)
        {
            Debug.LogError($"На двери перехода {gameObject.name} не назначен инвентарь в инспекторе!");
        }

        // Инициализация состояний UI
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(false);
        if (loadingScreen != null) loadingScreen.SetActive(false);
    }

    void Update()
    {
        // Если уже идет загрузка, объект игрока или инвентарь не найдены - отключаем логику
        if (isLoading || playerObject == null || playerInventory == null) return;

        // Считаем дистанцию
        float dist = Vector3.Distance(transform.position, playerObject.transform.position);

        if (dist <= interactDistance)
        {
            // Показываем подсказку "[E]", если не показан текст "Заперто"
            if (!isShowingLockedHint && interactHint != null)
            {
                interactHint.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }
        else
        {
            // Выключаем подсказки, если отошли
            if (interactHint != null) interactHint.SetActive(false);
            if (lockedHint != null) lockedHint.SetActive(false);
            isShowingLockedHint = false;
        }
    }

    public void TryInteract()
    {
        // Проверяем предмет через ПРЯМУЮ ссылку на инвентарь (как в твоем коде)
        if (playerInventory.currentItem != null)
        {
            if (playerInventory.currentItem.itemName == requiredItemName)
            {
                // Успех -> Начинаем загрузку
                StartCoroutine(LoadLevelRoutine());
            }
            else
            {
                // Не тот предмет
                AccessDenied();
            }
        }
        else
        {
            // Нет предмета в руках
            AccessDenied();
        }
    }

    private void AccessDenied()
    {
        // Играем звук "Заперто" один раз
        if (asource != null && lockedSound != null)
            asource.PlayOneShot(lockedSound);

        // Показываем корутину с текстом
        if (!isShowingLockedHint)
            StartCoroutine(ShowLockedHintSequence());
    }

    // Корутина загрузки (взята за основу из твоего референса)
    IEnumerator LoadLevelRoutine()
    {
        isLoading = true; // Блокируем повторное нажатие 'E'

        // 1. Прячем UI взаимодействия
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(false);

        // 2. Играем звук открытия
        if (asource != null && openDoorSound != null)
        {
            asource.PlayOneShot(openDoorSound);
        }

        // 3. Показываем экран загрузки
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // 4. Ждем
        yield return new WaitForSeconds(loadDelay);

        // 5. Загружаем сцену
        SceneManager.LoadScene(nextSceneName);
    }

    // Твоя оригинальная корутина для показа текста
    IEnumerator ShowLockedHintSequence()
    {
        isShowingLockedHint = true;
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(true);

        yield return new WaitForSeconds(4.0f);

        if (lockedHint != null) lockedHint.SetActive(false);
        isShowingLockedHint = false;
    }
}