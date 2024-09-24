using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private float interactRange=2.6f;

    public InteractBehaviour PLayerInteractBehaviour;

    [SerializeField]
    private GameObject interactText;

    [SerializeField]
    private LayerMask layerMask;

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit,interactRange,layerMask))
        {
            Text interactTextComponent = interactText.GetComponent<Text>();
            if (hit.transform.CompareTag("Harvestable"))
            {
                if (interactTextComponent != null)
                {
                    interactTextComponent.text = "Press E to harvest";
                }
            }
            else
            {
                if (interactTextComponent != null)
                {
                    interactTextComponent.text = "Press E to pick up";
                }
            }
            interactText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hit.transform.CompareTag("Item"))
                {
                    PLayerInteractBehaviour.DoPickUp(hit.transform.gameObject.GetComponent<Item>());
                }

                if (hit.transform.CompareTag("Harvestable"))
                {
                    PLayerInteractBehaviour.DoHarvest(hit.transform.gameObject.GetComponent<Harvestable>());
                }
                
            }
        }
        else
            interactText.SetActive(false);
    }
}
