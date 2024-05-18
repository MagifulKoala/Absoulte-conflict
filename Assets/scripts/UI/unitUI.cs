using System;
using UnityEngine;
using UnityEngine.UI;

public class unitUI : MonoBehaviour
{
    Button button;
    int unitId;

    private void OnEnable()
    {
        button = GetComponent<Button>();  
        button.onClick.AddListener(OnButtonClick);  
    }

    private void OnButtonClick()
    {
        UIControl.uiInstance.selectUnit(unitId);
    }

    public void setId(int pId)
    {
        unitId = pId;
    }


}
