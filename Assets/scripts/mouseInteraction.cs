using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class mouseInteraction : MonoBehaviour
{
    [Header("prefabs")]
    public GameObject spawnTemplate;
    public Army army;
    public GameObject directionArrowPrefab;
    public GameObject startPointPrefab;
    public GameObject soldierPosPrefab;
    GameObject currentDirectionArrowPrefab;
    GameObject currentStartPointPrefab;
    List<GameObject> currentSoldierPositions;
    Camera mainCamera;
    bool firstPointDrawn;
    LineRenderer lineRenderer;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        formLine();
        //spawnUnitOnFieldControl();
    }

    void DrawLine(Vector3 pInitPoint, Vector3 pEndPoint)
    {
        DestroyPreviousPoints();

        lineRenderer.SetPosition(0, pInitPoint);
        lineRenderer.SetPosition(1, pEndPoint);
        lineRenderer.enabled = true;

        drawPoints(pInitPoint, pEndPoint);
    }

    private void DestroyPreviousPoints()
    {
        if (currentDirectionArrowPrefab != null)
        {
            Destroy(currentDirectionArrowPrefab);
            Destroy(currentStartPointPrefab);
        }
    }

    private void drawPoints(Vector3 pInitPoint, Vector3 pEndPoint)
    {
        //draw start point
        currentStartPointPrefab = Instantiate(startPointPrefab, pInitPoint, Quaternion.identity);
        currentStartPointPrefab.transform.Rotate(new Vector3(90, 0, 0));
        currentStartPointPrefab.transform.position = currentStartPointPrefab.transform.position + new Vector3(0, 0.20f, 0);

        //draw end arrow
        Vector3 dir = Vector3.Normalize(pEndPoint - pInitPoint);
        currentDirectionArrowPrefab = Instantiate(directionArrowPrefab, pEndPoint, Quaternion.LookRotation(dir));
        currentDirectionArrowPrefab.transform.Rotate(new Vector3(90, 0, 0));
        currentDirectionArrowPrefab.transform.position = currentDirectionArrowPrefab.transform.position + new Vector3(0, 0.20f, 0);
    }

    void EraseLine()
    {
        if (currentDirectionArrowPrefab != null)
        {
            Destroy(currentDirectionArrowPrefab);
        }
        lineRenderer.enabled = false;
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

    void formLine()
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
                    DrawLine(firstPoint, secondPoint);
                }
            }
        }

        if (Input.GetMouseButtonUp(1) && !rightClickUp)
        {
            rightClickUp = true;
        }

        if (rightClickUp && firstPointDrawn)
        {
            EraseLine();
            rightClickUp = false;
            rightClickDown = false;
            firstPointDrawn = false;
            previousPoints.Clear();
            Debug.Log($"finishes --> firstPos: {firstPoint} second point: {secondPoint}");


            //spawn current unit on line 
            spawnCurrentUnitOnLine(firstPoint, secondPoint);


            //reset points
            firstPoint = Vector3.zero;
            secondPoint = Vector3.zero;
        }
    }

    void spawnCurrentUnitOnLine(Vector3 pInitPoint, Vector3 pEndPoint)
    {
        float dist = Vector3.Distance(pInitPoint, pEndPoint);

        Unit currentUnit = GetCurrenSelectedUnit().GetComponent<Unit>();
        float currentUnitSpacing = currentUnit.unitXSpacing;
        int maxColumns = currentUnit.currentSoldiers;

        int possibleNewColumns = Mathf.CeilToInt(dist / currentUnitSpacing);

        Debug.Log($"currentUnitSpacing: {currentUnitSpacing} , possibleCols: {possibleNewColumns}, dist: {dist}");

        //TODO: set var for minimum number of columns
        if (possibleNewColumns >= 2 && currentUnit.isDeployed)
        {
            if (possibleNewColumns > maxColumns)
            {
                possibleNewColumns = maxColumns;
            }
            currentUnit.updateFormation(possibleNewColumns);
            spawnUnitOnFieldControl(pInitPoint, pEndPoint, false);
        }
        else
        {
            spawnUnitOnFieldControl(pInitPoint, pEndPoint, true);
        }

    }


    //controls how the currently selected unit is spawn on the field
    public void spawnUnitOnFieldControl(Vector3 spawnPoint, Vector3 endPoint, bool keepPreviousFormation)
    {
        Unit currentUnit = GetCurrenSelectedUnit().GetComponent<Unit>();
        if (currentUnit != null)
        {
            if (currentUnit.isDeployed)
            {
                currentUnit.deleteCurrentSoldiers();
            }
            SpawnCurrentUnit(spawnPoint, endPoint, keepPreviousFormation);
        }

    }

    private GameObject GetCurrenSelectedUnit()
    {
        int unitID = UIControl.uiInstance.currentSelectedUnitId;
        return army.GetUnitById(unitID);
    }

    private void SpawnCurrentUnit(Vector3 spawnPoint, Vector3 endPoint, bool keepPreviousFormation)
    {
        Debug.Log("spawn current units called");
        Vector3? currentPoint = spawnPoint;
        GameObject unitObject = GetCurrenSelectedUnit();

        if (unitObject != null)
        {
            //Debug.Log(unitObject);
            if (unitObject.TryGetComponent<Unit>(out Unit unit))
            {
                if (currentPoint != null)
                {
                    Debug.Log($"point to spawn: {currentPoint.Value}");
                    unit.spawnUnitOnPoint(spawnPoint, endPoint, keepPreviousFormation);
                }
            }

        }
        else
        {
            Debug.Log($"unitID: {unitObject} returned null from army");
        }
    }
}
