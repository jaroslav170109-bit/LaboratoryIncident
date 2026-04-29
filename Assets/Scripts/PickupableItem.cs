using UnityEngine;

// Добавил AudioSource, так как твой скрипт Sounds его требует через GetComponent
[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AudioSource))]
public class PickupableItem : Sounds // Наследуемся от твоего скрипта Sounds
{
    private Rigidbody rb;
    private Collider coll;

    // Флаг, чтобы предмет не пытался издавать звуки, пока мы несем его в руке
    private bool isHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPosition)
    {
        isHeld = true; // Предмет в руках

        rb.isKinematic = true;
        coll.enabled = false;

        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Transform cameraTransform)
    {
        isHeld = false; // Предмет выпущен

        transform.SetParent(null);

        coll.enabled = true;
        rb.isKinematic = false;

        rb.AddForce(cameraTransform.forward * 4f, ForceMode.Impulse);
    }

    // Метод Unity, который срабатывает при столкновении коллайдеров
    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем: предмет не в руках + массив звуков не пустой + первый звук назначен
        if (!isHeld && sounds != null && sounds.Length > 0 && sounds[0] != null)
        {
            // Звук проиграется только если предмет ударился достаточно сильно (значение > 0.5f).
            // Это спасет от бага, когда предмет лежит на полу и "шуршит" от микро-движений физики.
            if (collision.relativeVelocity.magnitude > 0.5f)
            {
                // Запускаем звук падения (элемент 0 в твоем массиве sounds)
                // Громкость можно привязать к силе удара, но пока оставим стандартную
                PlaySound(sounds[0]);
            }
        }
    }
}