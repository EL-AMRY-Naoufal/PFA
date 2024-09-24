using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField]
    private Text HeaderField;

    [SerializeField]
    private Text ContentField;
    
    [SerializeField]
    private LayoutElement layoutElement;

    [SerializeField]
    private int MaxChar;

    [SerializeField]
    private RectTransform rectTransform;

    public void SetText(string Content, string Header = "")
    {
        if(Header=="")
            HeaderField.gameObject.SetActive(false);
        else
        {
            HeaderField.gameObject.SetActive(true);
            HeaderField.text = Header;
        }
        
        ContentField.text = Content;

        int HeaderLenght = HeaderField.text.Length;
        int ContentLenght = ContentField.text.Length;

        layoutElement.enabled = (HeaderLenght > MaxChar || ContentLenght > MaxChar) ? true : false;
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;

        float pivotX = mousePosition.x / Screen.width;
        float pivotY = mousePosition.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        transform.position = mousePosition;
    }

}
