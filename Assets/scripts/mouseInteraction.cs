using System.Collections.Generic;
using UnityEngine;

public class mouseInteraction : MonoBehaviour
{
    [Header("prefabs")]
    public GameObject spawnTemplate;
    public Army army;
    Camera mainCamera;
    bool firstPointDrawn;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        drawLine();
        spawnUnitOnField();
    }

    void checkRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click down");
        }
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Right click up");
        }
    }

    //mouse raycast to 'real world' position
    Vector3? MouseCast()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return null;
        }

    }
    Vector3 firstPoint = new Vector3();
    Vector3 secondPoint = new Vector3();
    bool rightClickDown = false;
    bool rightClickUp = false;
    Stack<GameObject> previousPoints = new Stack<GameObject>();

    void drawLine()
    {
        if (Input.GetMouseButtonDown(1) && !rightClickDown)
        {
            rightClickDown = true;
        }

        if (rightClickDown)
        {
            Vector3? currentPoint = MouseCast();
            if (currentPoint != null)
            {
                if (!firstPointDrawn)
                {
                    firstPoint = currentPoint.Value;
                    Instantiate(spawnTemplate, firstPoint, Quaternion.identity);
                    firstPointDrawn = true;
                }
                else
                {
                    if (previousPoints.Count > 0)
                    {
                        GameObject obj2Destroy = previousPoints.Pop();
                        Destroy(obj2Destroy);
                    }
                    secondPoint = currentPoint.Value;
                    previousPoints.Push(Instantiate(spawnTemplate, secondPoint, Quaternion.identity));
                }
            }
        }

        if (Input.GetMouseButtonUp(1) && !rightClickUp)
        {
            rightClickUp = true;
        }

        if (rightClickUp && firstPointDrawn)
        {
            rightClickUp = false;
            rightClickDown = false;
            firstPointDrawn = false;
            previousPoints.Clear();
            Debug.Log($"finishes --> firstPos: {firstPoint} second point: {secondPoint}");
            //TODO: call spawn method for current selected unit

            firstPoint = Vector3.zero;
            secondPoint = Vector3.zero;
        }
    }


    public void spawnUnitOnField()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3? currentPoint = MouseCast();
            int unitID = UIControl.uiInstance.currentSelectedUnitId;
            GameObject unitObject = army.GetUnitById(unitID);

            if (unitObject != null)
            {
                Debug.Log(unitObject);
                if(unitObject.TryGetComponent<Unit>(out Unit unit))
                {
                    Debug.Log($"point to spawn: {currentPoint.Value}");
                    unit.spawnUnitOnPoint(currentPoint.Value);
                }
             
            }
            else
            {
                Debug.Log($"unitID: {unitID} returned null from army");
            }

        }

    }




}
