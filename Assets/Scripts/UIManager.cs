using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] UIPanels;

    [SerializeField]
    private ThirdPersonOrbitCamBasic PlayerCameraScrpit;

    private float defaultHorizontalAimingSpeed;
    private float defaultVerticalalAimingSpeed;

    [HideInInspector]
    public bool atLeadtOnePanelOpened;

    void Start()
    {
        defaultHorizontalAimingSpeed = PlayerCameraScrpit.horizontalAimingSpeed;
        defaultVerticalalAimingSpeed = PlayerCameraScrpit.verticalAimingSpeed;
    }

    void Update()
    {
        atLeadtOnePanelOpened = UIPanels.Any((panel) => panel == panel.activeSelf);
        if (atLeadtOnePanelOpened)
        {
            PlayerCameraScrpit.horizontalAimingSpeed=0;
            PlayerCameraScrpit.verticalAimingSpeed=0;
        }
        else
        {
            PlayerCameraScrpit.horizontalAimingSpeed = defaultHorizontalAimingSpeed;
            PlayerCameraScrpit.verticalAimingSpeed = defaultVerticalalAimingSpeed;
        }
    }
}
