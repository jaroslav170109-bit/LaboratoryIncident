using UnityEngine;

// Ключ теперь — это просто предмет, который можно поднять
public class InventoryKey : PickupableItem
{
    [Header("Идентификатор ключа")]
    public string keyID; // Например, "Office_Key" или "Lab_Key"
}