using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour ,IPointerEnterHandler ,IPointerExitHandler
{
    public ItemData item;
    public Image itemVisual;
    public Text countText;

    [SerializeField]
    private ItemActionSystem itemActionSystem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            ToolTipSystem.instance.Show(item.Description,item.Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.instance.Hide();
    }

    public void ClickOnSlot()
    {
        itemActionSystem.OpenActionPanel(item,transform.position);
    }

}
