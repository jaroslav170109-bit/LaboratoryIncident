using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : Sounds
{
    [SerializeField] private Transform[] points;
    private Transform currentPoint;
    private enum State { potrul, walkToPlayer }

    [SerializeField] private State currentState = State.potrul;
    private GameObject player;
    private NavMeshAgent agent;

    [Header("Настройки")]
    public float catchDistance = 1.5f;
    public float footstepInterval = 0.6f;
    public string sceneToLoad = "Level 1";

    [Header("Новые ссылки")]
    public GameObject chaseMusicObject; // Объект с музыкой погони
    public GameObject gameOverUI;       // Экран "Вы проиграли"

    private float footstepTimer;
    private bool isGameOver = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        // Выключаем музыку и экран смерти на старте, на всякий случай
        if (chaseMusicObject != null) chaseMusicObject.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);

        WalkToNewPoint();
    }

    void Update()
    {
        if (isGameOver) return;

        // Логика звука шагов
        if (agent.velocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                if (sounds.Length > 0 && sounds[0] != null) PlaySound(sounds[0]);
                footstepTimer = footstepInterval;
            }
        }

        // Проверка дистанции до точки патруля
        if (currentPoint != null && (transform.position - currentPoint.position).magnitude < 2f)
        {
            WalkToNewPoint();
        }

        // Проверка поимки игрока
        if (Vector3.Distance(transform.position, player.transform.position) <= catchDistance)
        {
            StartCoroutine(CatchPlayer());
        }
    }

    void FixedUpdate()
    {
        if (isGameOver || player == null) return;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float d = Vector3.Dot(directionToPlayer, transform.forward);

        if (d > 0.2f)
        {
            Vector3 origin = transform.position + Vector3.up * 1.0f;
            Vector3 target = player.transform.position + Vector3.up * 1.0f;
            Vector3 direction = target - origin;

            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, 100f))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    if (currentState == State.potrul)
                    {
                        // КРИК (звук)
                        if (sounds.Length > 1 && sounds[1] != null)
                            PlaySound(sounds[1], 0.5f, false, 0.5f, 0.7f);

                        // ВКЛЮЧАЕМ ОБЪЕКТ МУЗЫКИ
                        if (chaseMusicObject != null) chaseMusicObject.SetActive(true);

                        CancelInvoke("DisableWalkToPlayer"); // Сбрасываем старые таймеры, если были
                        Invoke("DisableWalkToPlayer", 5);
                    }
                    currentState = State.walkToPlayer;
                }
            }
        }

        if (currentState == State.walkToPlayer)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    void DisableWalkToPlayer()
    {
        if (isGameOver || player == null) return;

        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 target = player.transform.position + Vector3.up * 1.0f;
        Vector3 direction = target - origin;

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, 100f))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                currentState = State.potrul;

                // ВЫКЛЮЧАЕМ ОБЪЕКТ МУЗЫКИ ПРИ ПОТЕРЕ ИГРОКА
                if (chaseMusicObject != null) chaseMusicObject.SetActive(false);

                WalkToNewPoint();
            }
            else
            {
                Invoke("DisableWalkToPlayer", 5);
            }
        }
    }

    void WalkToNewPoint()
    {
        if (points.Length == 0) return;
        currentPoint = points[Random.Range(0, points.Length)];
        agent.SetDestination(currentPoint.position);
    }

    IEnumerator CatchPlayer()
    {
        isGameOver = true;
        agent.isStopped = true;

        // Выключаем музыку погони при смерти
        if (chaseMusicObject != null) chaseMusicObject.SetActive(false);

        // Звук смерти
        if (sounds.Length > 2 && sounds[2] != null)
            PlaySound(sounds[2], 1f, false, 0.4f, 0.6f);

        // ВКЛЮЧАЕМ ЭКРАН ПРОИГРЫША
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Ждем немного и перезагружаем (или уберите SceneManager, если хотите просто оставить экран)
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneToLoad);
    }
}