using UnityEngine;

public class Entity : Sounds
{
    [SerializeField] private float startingHealth = 100f;
    public GameObject deadBody; // Та самая переменная для модели трупа

    private float health;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            Debug.Log($"Текущее здоровье: {health}");

            if (health <= 0f)
            {
                Die();
            }
        }
    }

    void Start()
    {
        Health = startingHealth;
        // На всякий случай выключаем труп на старте, если забыл в инспекторе
        if (deadBody != null) deadBody.SetActive(false);
    }

    private void Die()
    {
        if (sounds != null && sounds.Length > 0 && sounds[0] != null)
        {
            PlaySound(sounds[0]);
        }

        // Активируем труп
        if (deadBody != null)
        {
            deadBody.SetActive(true);
            deadBody.transform.parent = null; // Отцепляем от монстра, чтобы не удалился!
        }

        Destroy(gameObject);
    }
}