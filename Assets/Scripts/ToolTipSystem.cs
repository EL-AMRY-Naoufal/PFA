using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem instance;

    [SerializeField]
    private ToolTip toolTip;

    public void Awake()
    {
        instance = this;    
    }

    public void Show(string Content,string Header)
    {
        toolTip.SetText(Content, Header);
        toolTip.gameObject.SetActive(true);
    }

    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }

}
