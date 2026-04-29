using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AudioSource))]
public class PickupableItem : Sounds
{
    public string itemName = "Предмет"; // Название предмета для подсказки

    private Rigidbody rb;
    private Collider coll;
    private bool isHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    // Метод для управления подсказкой (будет вызываться из инвентаря)
    public void ToggleHint(bool state)
    {
        // Мы передадим управление отображением в PlayerInventory, 
        // чтобы не искать UI-текст на каждом предмете отдельно.
    }

    public void PickUp(Transform holdPosition)
    {
        isHeld = true;
        rb.isKinematic = true;
        coll.enabled = false;
        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Transform cameraTransform)
    {
        isHeld = false;
        transform.SetParent(null);
        coll.enabled = true;
        rb.isKinematic = false;
        rb.AddForce(cameraTransform.forward * 4f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHeld && sounds != null && sounds.Length > 0 && sounds[0] != null)
        {
            if (collision.relativeVelocity.magnitude > 0.5f)
            {
                PlaySound(sounds[0]);
            }
        }
    }
}