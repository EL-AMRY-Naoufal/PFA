using UnityEngine;

public class ItemActionSystem : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private PlayerStats playerStats;
 
    [Header("ITEM ACTION SYSTEM VARIBALES")]

    public GameObject actionPanel;

    [SerializeField]
    private Transform dropPoint; 

    [SerializeField]
    private GameObject UseItemButton;
    
    [SerializeField]
    private GameObject EquipItemButton;
    
    [SerializeField]
    private GameObject DropItemButton;

    [SerializeField]
    private GameObject DestroyItemButton;

    [HideInInspector]
    public ItemData itemCurrentlySelected;

    public void OpenActionPanel(ItemData item,Vector3 slotPosition)
    {
        itemCurrentlySelected=item; 

        if(item==null)
        {
            actionPanel.SetActive(false);
            return;
        }
        
        switch(item.itemtype)
        {
            case ItemType.Ressource:
                UseItemButton.SetActive(false);
                EquipItemButton.SetActive(false);
                break;
            case ItemType.Equipment:
                UseItemButton.SetActive(false);
                EquipItemButton.SetActive(true);
                break;
            case ItemType.Consumable:
                UseItemButton.SetActive(true);
                EquipItemButton.SetActive(false);
                break;
        }
        actionPanel.transform.position=slotPosition;
        actionPanel.SetActive(true);
    }

    public void CloseActionPanel()
    {
        actionPanel.SetActive(false);
        itemCurrentlySelected=null;
    }

    public void UseActionButton()
    {
        playerStats.ConsumeItem(itemCurrentlySelected.healthEffect, itemCurrentlySelected.hungerEffect, itemCurrentlySelected.thirstEffect);
        Inventory.instance.RemoveItem(itemCurrentlySelected);
        CloseActionPanel();
    }

    public void EquipActionButton()
    {
        equipment.EquipAction();
    }

    public void DropActionButton()
    {
        GameObject instantiatedItem = Instantiate(itemCurrentlySelected.Prefab);
        instantiatedItem.transform.position=dropPoint.position;
        Inventory.instance.RemoveItem(itemCurrentlySelected);
        Inventory.instance.RefreshContent();
        CloseActionPanel();
    }

    public void DestroyActionButton()
    {
        Inventory.instance.RemoveItem(itemCurrentlySelected);
        Inventory.instance.RefreshContent();
        CloseActionPanel();
    }

}
