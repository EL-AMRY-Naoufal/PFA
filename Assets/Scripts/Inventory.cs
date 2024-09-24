using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private ItemActionSystem itemActionSystem;

    [SerializeField]
    private CraftingSystem craftingSystem;

    [SerializeField]
    private BuildSystem buildSystem;    

    [Header("INVENTORY SYSTEM VARIABLES")]

    [SerializeField]
    private List<ItemInInventory> Content = new List<ItemInInventory>();

    [SerializeField]
    private GameObject InventoryPanel;

    [SerializeField]
    private Transform inventorySlotsParent;

    public Sprite emptySlotVisual;

    public static Inventory instance; 

    const int InventorySize = 24;
    private bool isOpen=false;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CloseInventory();
        RefreshContent();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(isOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    public void AddItem(ItemData item)
    {
        ItemInInventory[] itemInInventory = Content.Where(elem => elem.itemData==item).ToArray();

        bool itemAdded = false;

        if(itemInInventory.Length > 0 && item.stackable)
        {
            for (int i = 0; i < itemInInventory.Length; i++)
            {
                if(itemInInventory[i].count < item.maxStack)
                {
                    itemAdded=true;
                    itemInInventory[i].count++;
                    break;
                }
            }

            if(!itemAdded)
            {
                Content.Add(
                    new ItemInInventory
                    {
                        itemData=item,
                        count=1
                    }
                );
            }

        }
        else
        {
            Content.Add(
                new ItemInInventory
                {
                    itemData=item,
                    count=1
                }
            );
        }

        RefreshContent();
    }

    public void RemoveItem(ItemData item)
    {
        ItemInInventory itemInInventory = Content.Where(elem=>elem.itemData==item).FirstOrDefault();
        if(itemInInventory != null && itemInInventory.count > 1)
        {
            itemInInventory.count--;
        }
        else
        {
            Content.Remove(itemInInventory);
        }
        RefreshContent();
    }

    public List<ItemInInventory> GetContent()
    {
        return Content;
    }
    
    public void OpenInventory()
    {
        InventoryPanel.SetActive(true);
        RefreshContent();
        isOpen = true;
    }

    public void CloseInventory()
    {
        InventoryPanel.SetActive(false);
        itemActionSystem.actionPanel.SetActive(false);
        ToolTipSystem.instance.Hide();
        isOpen=false;
    }

    public void RefreshContent()
    {
        for (int i=0;i<inventorySlotsParent.childCount;i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countText.enabled = false;
        }

        for (int i = 0; i < Content.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = Content[i].itemData;
            currentSlot.itemVisual.sprite = Content[i].itemData.Visual;
            if(currentSlot.item.stackable)
            {
                currentSlot.countText.enabled=true;
                currentSlot.countText.text = Content[i].count.ToString();
            }
        }

        equipment.UpdateEquipementsDesequipButtons();
        craftingSystem.UpdateDisplayedRecipes();
        buildSystem.UpdateDisplayedCosts();
    }

    public bool IsFull()
    {
        return InventorySize == Content.Count;
    }

    public void LoadData(List<ItemInInventory> savedData)
    {
        Content = savedData;
        RefreshContent();
    }

    public void ClearInventoryContent()
    {
        Content.Clear();
    }
}

[System.Serializable]
public class ItemInInventory
{
    public ItemData itemData;
    public int count;
}