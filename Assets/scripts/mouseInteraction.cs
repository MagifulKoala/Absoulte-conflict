using System.Collections.Generic;
using UnityEngine;

public class mouseInteraction : MonoBehaviour
{
    [Header("prefabs")]
    public GameObject spawnTemplate;
    Camera mainCamera;
    bool firstPointDrawn;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        drawLine();
    }

    void checkRightClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click down");
        }
        if(Input.GetMouseButtonUp(1))  
        {
            Debug.Log("Right click up");
        }
    }

    //mouse raycast to 'real world'
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
                        Debug.Log("hi");
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

            firstPoint = Vector3.zero;
            secondPoint = Vector3.zero;
        }
    }




}
