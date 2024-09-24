using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Equipment : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private ItemActionSystem itemActionSystem;

    [SerializeField]
    private PlayerStats playerStats;

    [Header("EQUIPEMENT SYSTEM VARIABLES")]

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;

    [SerializeField]
    private Image headSlotImage;

    [SerializeField]
    private Image ChestSlotImage;

    [SerializeField]
    private Image handsSlotImage;

    [SerializeField]
    private Image legsSlotImage;

    [SerializeField]
    private Image feetSlotImage;

    [SerializeField]
    private Image weaponSlotImage;

    //Garde une trace des equipement actuels
    [HideInInspector]
    public ItemData equipedHeadItem;
    [HideInInspector]
    public ItemData equipedChestItem;
    [HideInInspector]
    public ItemData equipedHandsItem;
    [HideInInspector]
    public ItemData equipedLegsItem;
    [HideInInspector]
    public ItemData equipedFeetItem;
    [HideInInspector]
    public ItemData equipedWeaponItem;

    [SerializeField]
    private Button headSlotDesequipButton;

    [SerializeField]
    private Button chestSlotDesequipButton;

    [SerializeField]
    private Button handsSlotDesequipButton;

    [SerializeField]
    private Button legSlotDesequipButton;

    [SerializeField]
    private Button feetSlotDesequipButton;

    [SerializeField]
    private Button weaponSlotDesequipButton;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip equipeSound;

    private void DisablePreviousEquipedEquipement(ItemData itemToDisable)
    {
        if(itemToDisable==null)
            return;

        EquipmentLibraryItem equipmentLibraryItem=equipmentLibrary.content.Where(elem=>elem.itemData==itemToDisable).First();
        if(equipmentLibraryItem!=null)
        {
            for(int i=0;i<equipmentLibraryItem.elementsToDisable.Length;i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(true);
            }
            equipmentLibraryItem.itemPrefab.SetActive(false);
        }

        playerStats.currentArmorpoints -= itemToDisable.armorPoints;

        Inventory.instance.AddItem(itemToDisable);
    }

    public void DesequipEquipement(EquipmentType equipmentType)
    {
        //1-Enlever le visual de l'equipement sur le personnage + ré-activer les parties visuelles qu'on avait désactiver pour cet objet
        //2-Enlever le visuel de l'équipement de la colonne équipement de l'inventaire
        //3-Renvoyer l'équipement dans l'inventaire du joueur
        //4-faire un refresh content à la fin

        if(Inventory.instance.IsFull())
            return;
        
        ItemData currentItem=null;
        switch(equipmentType)
        {
            case EquipmentType.Head:
                currentItem = equipedHeadItem;
                equipedHeadItem=null;
                headSlotImage.sprite=Inventory.instance.emptySlotVisual;
                break;
            case EquipmentType.Chest:
                currentItem = equipedChestItem;
                equipedChestItem=null;
                ChestSlotImage.sprite=Inventory.instance.emptySlotVisual;
                break;
            case EquipmentType.Hands:
                currentItem = equipedHandsItem;
                equipedHandsItem=null;
                handsSlotImage.sprite=Inventory.instance.emptySlotVisual;
                break;
            case EquipmentType.Legs:
                currentItem = equipedLegsItem;
                equipedLegsItem=null;
                legsSlotImage.sprite=Inventory.instance.emptySlotVisual;
                break;
            case EquipmentType.Feet:
                currentItem = equipedFeetItem;
                equipedFeetItem=null;
                feetSlotImage.sprite=Inventory.instance.emptySlotVisual;
                break;
            case EquipmentType.Weapon:
                currentItem = equipedWeaponItem;
                equipedWeaponItem = null;
                weaponSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;
        }

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == currentItem).FirstOrDefault();
        if(equipmentLibraryItem!=null)
        {
            for(int i=0;i<equipmentLibraryItem.elementsToDisable.Length;i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(true);
            }
            equipmentLibraryItem.itemPrefab.SetActive(false);
        }

        if(currentItem)
        {
            playerStats.currentArmorpoints -= currentItem.armorPoints;
            Inventory.instance.AddItem(currentItem);
        }
        Inventory.instance.RefreshContent();
    }

    public void UpdateEquipementsDesequipButtons()
    {
        headSlotDesequipButton.onClick.RemoveAllListeners();
        headSlotDesequipButton.onClick.AddListener(delegate {DesequipEquipement(EquipmentType.Head);});
        headSlotDesequipButton.gameObject.SetActive(equipedHeadItem);

        chestSlotDesequipButton.onClick.RemoveAllListeners();
        chestSlotDesequipButton.onClick.AddListener(delegate {DesequipEquipement(EquipmentType.Chest);});
        chestSlotDesequipButton.gameObject.SetActive(equipedChestItem);

        handsSlotDesequipButton.onClick.RemoveAllListeners();
        handsSlotDesequipButton.onClick.AddListener(delegate {DesequipEquipement(EquipmentType.Hands);});
        handsSlotDesequipButton.gameObject.SetActive(equipedHandsItem);

        legSlotDesequipButton.onClick.RemoveAllListeners();
        legSlotDesequipButton.onClick.AddListener(delegate {DesequipEquipement(EquipmentType.Legs);});
        legSlotDesequipButton.gameObject.SetActive(equipedLegsItem);

        feetSlotDesequipButton.onClick.RemoveAllListeners();
        feetSlotDesequipButton.onClick.AddListener(delegate {DesequipEquipement(EquipmentType.Feet);});
        feetSlotDesequipButton.gameObject.SetActive(equipedFeetItem);

        weaponSlotDesequipButton.onClick.RemoveAllListeners();
        weaponSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipement(EquipmentType.Weapon); });
        weaponSlotDesequipButton.gameObject.SetActive(equipedWeaponItem);
    }

    public void EquipAction(ItemData equipment = null)
    {
        ItemData itemToEquip = equipment ? equipment : itemActionSystem.itemCurrentlySelected;

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem=>elem.itemData==itemToEquip).First();

        if(equipmentLibraryItem!=null)
        {
            switch(itemToEquip.equipmentType)
            {
                case EquipmentType.Head:
                    DisablePreviousEquipedEquipement(equipedHeadItem);
                    headSlotImage.sprite=itemToEquip.Visual;
                    equipedHeadItem=itemToEquip;
                    break;
                case EquipmentType.Chest:
                    DisablePreviousEquipedEquipement(equipedChestItem);
                    ChestSlotImage.sprite=itemToEquip.Visual;
                    equipedChestItem=itemToEquip;
                    break;
                case EquipmentType.Hands:
                    DisablePreviousEquipedEquipement(equipedHandsItem);
                    handsSlotImage.sprite=itemToEquip.Visual;
                    equipedHandsItem=itemToEquip;
                    break;
                case EquipmentType.Legs:
                    DisablePreviousEquipedEquipement(equipedLegsItem);
                    legsSlotImage.sprite=itemToEquip.Visual;
                    equipedLegsItem=itemToEquip;
                    break;
                case EquipmentType.Feet:
                    DisablePreviousEquipedEquipement(equipedFeetItem);
                    feetSlotImage.sprite=itemToEquip.Visual;
                    equipedFeetItem=itemToEquip;
                    break;
                case EquipmentType.Weapon:
                    DisablePreviousEquipedEquipement(equipedWeaponItem);
                    weaponSlotImage.sprite = itemToEquip.Visual;
                    equipedWeaponItem = itemToEquip;
                    break;
            }

            for(int i=0;i<equipmentLibraryItem.elementsToDisable.Length;i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(false);
            }
            
            equipmentLibraryItem.itemPrefab.SetActive(true);

            playerStats.currentArmorpoints += itemToEquip.armorPoints;

            Inventory.instance.RemoveItem(itemToEquip);

            audioSource.PlayOneShot(equipeSound);

        }
        else
        {
            Debug.LogError("Equipment : "+ itemToEquip.Name+"non existant dans la library des equipment");
        }
        itemActionSystem.CloseActionPanel();
    }

    public void LoadEquipements(ItemData[] savedEquipements)
    {
        //1. on affecte le contenu de l'inventaire pour evité que DesequipEquipement ne soit bloqué par un inventaire full
        Inventory.instance.ClearInventoryContent();

        //2. on desqéuipe tous
        foreach (EquipmentType type  in System.Enum.GetValues(typeof(EquipmentType)))
        {
            DesequipEquipement(type);
        }

        //3.charger les equi 
        foreach (ItemData item in savedEquipements)
        {
            if(item)
                EquipAction(item);
        }
    }
}
