using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private MoveBehaviour PlayerMoveBehaviour;

    [SerializeField]
    private Animator PlayerAnimator;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private Equipment equipmentSystem;

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;

    [HideInInspector]
    public bool isBusy = false;

    [SerializeField]
    private AudioSource audioSource;

    [Header("Tools Configuration")]
    [SerializeField]
    private GameObject pickAxeVisual;

    [SerializeField]
    private AudioClip pickaxeSound;

    [SerializeField]
    private GameObject AxeVisual;
    [SerializeField]
    private AudioClip axeSound;

    [Header("Other")]
    [SerializeField]
    private AudioClip pickUpSound;

    private Item CurrentItem;
    private Harvestable currentHarvestable;
    private Tool currentTool;

    private Vector3 spawnItemOffset = new Vector3(0, 0.5f, 0);

    public void DoPickUp(Item item)
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;

        if (inventory.IsFull())
        {
            Debug.Log("Inventory Full , can't pick up : " + item.name);
            return;
        }
            
        CurrentItem = item;
        PlayerAnimator.SetTrigger("PickUp");
        PlayerMoveBehaviour.canMove = false;
    }

    public void DoHarvest(Harvestable harvestable)
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;

        currentTool=harvestable.tool;
        EnableToolGameObjectFromEnum(currentTool);
        currentHarvestable=harvestable;
        PlayerAnimator.SetTrigger("Harvest");
        PlayerMoveBehaviour.canMove=false;
    }

    //courotine appelée depuis l'animation de "Harvesting"

    IEnumerator BreakHarvestable()
    {
        Harvestable currentlyHarvesting = currentHarvestable;

        currentlyHarvesting.gameObject.layer = LayerMask.NameToLayer("Default");

        if(currentlyHarvesting.disableKenematicOnharvest)
        {
            Rigidbody rigidbody = currentlyHarvesting.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.AddForce(transform.forward * 5000, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(currentlyHarvesting.DestroyDelay);

        for (int i = 0; i < currentlyHarvesting.harvestableItems.Length; i++)
        {
            Ressource ressource = currentlyHarvesting.harvestableItems[i];
            if(Random.Range(1,101) <= ressource.dropChance)
            {
                GameObject instantiatedRessource = Instantiate(ressource.itemData.Prefab);
                instantiatedRessource.transform.position = currentlyHarvesting.transform.position + spawnItemOffset;
            }
        }
        Destroy(currentlyHarvesting.gameObject);
    }

    public void AddItemToIntentory()
    {
        inventory.AddItem(CurrentItem.itemData);
        audioSource.PlayOneShot(pickUpSound);
        Destroy(CurrentItem.gameObject);
    }

    public void ReEnablePlayerMovement()
    {
        EnableToolGameObjectFromEnum(currentTool,false);
        PlayerMoveBehaviour.canMove = true; 
        isBusy = false;
    }

    private void EnableToolGameObjectFromEnum(Tool toolType,bool Enabled=true)
    {
        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == equipmentSystem.equipedWeaponItem).FirstOrDefault();
        if (equipmentLibraryItem != null)
        {
            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(Enabled);
            }
            equipmentLibraryItem.itemPrefab.SetActive(!Enabled);
        }
        
        switch (toolType)
        {
            case Tool.Pickaxe:
                pickAxeVisual.SetActive(Enabled);
                audioSource.clip = pickaxeSound;
                break;
            case Tool.Axe:
                AxeVisual.SetActive(Enabled);
                audioSource.clip = axeSound;
                break;
        }
    }

    public void PlayHarvestingSoundEffect()
    {
        audioSource.Play();
    }

}
