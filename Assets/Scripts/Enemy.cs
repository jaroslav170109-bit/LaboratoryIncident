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

    private float footstepTimer;
    private bool isGameOver = false;
    private bool isMusicPlaying = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(player.transform.position);
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
        if ((transform.position - currentPoint.position).magnitude < 5f)
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

        // Угол зрения (видит только перед собой)
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float d = Vector3.Dot(directionToPlayer, transform.forward);

        if (d > 0.2f)
        {
            // Наш настроенный луч (на уровне груди)
            Vector3 origin = transform.position + Vector3.up * 1.0f;
            Vector3 target = player.transform.position + Vector3.up * 1.0f;
            Vector3 direction = target - origin;

            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, 100f))
            {
                // Наша "умная" проверка по тегу, которая всё починила
                if (hit.transform.CompareTag("Player"))
                {
                    if (currentState == State.potrul)
                    {
                        // КРИК (Элемент 1): Низкий питч (0.5 - 0.7)
                        if (sounds.Length > 1 && sounds[1] != null)
                            PlaySound(sounds[1], 0.5f, false, 0.5f, 0.7f);

                        // МУЗЫКА ПОГОНИ (Элемент 3)
                        if (!isMusicPlaying && sounds.Length > 3 && sounds[3] != null)
                        {
                            PlaySound(sounds[3], 20f, false, 1f, 1f);
                            isMusicPlaying = true;
                        }

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
            // Если луч уперся в стену и больше не видит тег "Player"
            if (!hit.transform.CompareTag("Player"))
            {
                currentState = State.potrul;
                isMusicPlaying = false; // Сбрасываем, чтобы музыка заиграла снова при новой встрече
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

        // ЗВУК СМЕРТИ (Элемент 2): Тоже низкий и жуткий
        if (sounds.Length > 2 && sounds[2] != null)
            PlaySound(sounds[2], 1f, false, 0.4f, 0.6f);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneToLoad); // Убедись, что сцена добавлена в Build Settings
    }
}