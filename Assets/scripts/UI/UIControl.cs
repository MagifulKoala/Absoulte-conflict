using Unity.VisualScripting;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    public GameObject unitUIPrefab;
    public Transform viewPortParent;
    public Army army;
    public int currentSelectedUnitId;

    int currentUnits = 0;

    public static UIControl uiInstance { get; private set; }
    private void Awake()
    {
        if (uiInstance != null && uiInstance != this)
        {
            Destroy(this);
        }
        else
        {
            uiInstance = this;
        }
    }


    public void addUnit()
    {
        GameObject newUnitUI = Instantiate(unitUIPrefab, viewPortParent);
        newUnitUI.GetComponent<unitUI>().setId(currentUnits);
        army.addUnitToArmy(currentUnits);
        currentUnits++;
    }

    public void selectUnit(int pUnitId)
    {
        currentSelectedUnitId = pUnitId;
    }

}
