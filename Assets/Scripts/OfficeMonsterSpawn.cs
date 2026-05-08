using UnityEngine;

public class OfficeMonsterSpawn : MonoBehaviour
{
    [Header("Ручная ссылка на инвентарь")]
    public PlayerInventory playerInventory;
    public string TriggerItem = "ManagerCard";
    public GameObject OfficeMonsterTrigger;
    private GameObject playerObject; // Ссылка для расчета дистанции (по тегу)


    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        }
    void Update()
    {
        if (playerInventory.currentItem.itemName == TriggerItem)
        {
            OfficeMonsterTrigger.SetActive(true);
        }
}
}
