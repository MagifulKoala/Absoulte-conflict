using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour
{
    public GameObject unitPrefab;
    List<GameObject> units = new List<GameObject>();

    public void addUnitToArmy(int pUnitId)
    {
        GameObject newUnit = Instantiate(unitPrefab, transform);
        newUnit.GetComponent<Unit>().setId(pUnitId);
        units.Add(newUnit);
    }

    public GameObject GetUnitById(int pUnitId)
    {


        foreach (GameObject unit in units)
        {
            if(unit.GetComponent<Unit>().unitId == pUnitId)
            {
                return unit;
            }
        }

        return null;
    }

}
