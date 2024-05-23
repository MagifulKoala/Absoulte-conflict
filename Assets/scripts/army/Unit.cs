using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject soldierPrefab;
    public int unitId { get; private set; }
    public float unitXSpacing;
    public float unitZSpacing;
    public int totalUnitSize = 4;
    public int currentSoldiers = 4;
    int rows;
    public int columns { get; private set; }
    public bool isDeployed { get; private set; }
    GameObject[,] soldiers;
    Vector3 previousDirection;

    private void OnEnable()
    {
        currentSoldiers = totalUnitSize;
        previousDirection = Vector3.right;
        initFormation();
    }

    public void setId(int pId)
    {
        unitId = pId;
    }

    void initFormation()
    {
        columns = Mathf.CeilToInt(Mathf.Sqrt(totalUnitSize));
        rows = Mathf.CeilToInt(totalUnitSize / columns);
        soldiers = new GameObject[rows, columns];
        Debug.Log($"initFormation called: {soldiers.GetLength(0)}, {soldiers.GetLength(1)}");
    }

    public void updateFormation(int pNewColumns)
    {
        Debug.Log("Unit: updating formation");
        Debug.Log($"currSoldier: {currentSoldiers}, cols: {pNewColumns}, curr/cols: {((float)currentSoldiers / (float)pNewColumns)}");
        rows = Mathf.CeilToInt(((float)currentSoldiers / (float)pNewColumns));
        int spawnCount = 0;
        GameObject[,] tempFormation = new GameObject[rows, pNewColumns];
        Queue<GameObject> soldierQueue = addSoldiersToQueue();


        Debug.Log($"tempFormation size: {rows * pNewColumns}, rc: {rows},{pNewColumns}, armySize: {currentSoldiers}");

        for (int i = 0; i < tempFormation.GetLength(0); i++)
        {
            for (int j = 0; j < tempFormation.GetLength(1); j++)
            {
                if (spawnCount >= currentSoldiers) { break; }
                tempFormation[i, j] = soldierQueue.Dequeue();
                spawnCount++;
            }
        }

        soldiers = tempFormation;
    }

    Queue<GameObject> addSoldiersToQueue()
    {
        Queue<GameObject> soldierQeue = new Queue<GameObject>();

        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                soldierQeue.Enqueue(soldiers[i, j]);
            }
        }

        return soldierQeue;
    }

    public void spawnUnitOnPoint(Vector3 pInitSpawnPoint, Vector3 pEndPoint, bool keepPreviousFormation)
    {
        Debug.Log($"spawnUnitOnPoint called. Unit id: {unitId}");
        Debug.Log($"formation: rows,cols: {soldiers.GetLength(0)},{soldiers.GetLength(1)}");
        Debug.Log($"spawnPoints: {pInitSpawnPoint},{pEndPoint}");
        Vector3 initSpawn = pInitSpawnPoint;
        Vector3 currentSpawnPoint = pInitSpawnPoint;
        int spawnCount = 0;
        //Debug.Log($"initSpawnPoint: {pInitSpawnPoint}");
        Vector3 direction = pEndPoint - pInitSpawnPoint;
        direction = Vector3.Normalize(direction);
        if (keepPreviousFormation)
        {
            direction = previousDirection;
        }
        previousDirection = direction;


        Vector3 depthDirection = Vector3.Cross(direction, Vector3.up);
        depthDirection = -Vector3.Normalize(depthDirection);

        //find rotation
        Quaternion rotationDirection = Quaternion.LookRotation(-depthDirection);

        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            //currentSpawnPoint.z = pInitSpawnPoint.z - i * unitZSpacing;
            initSpawn = pInitSpawnPoint + depthDirection * i * unitZSpacing;
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                //currentSpawnPoint.x = pInitSpawnPoint.x + j * unitXSpacing;
                currentSpawnPoint = initSpawn + direction * j * unitXSpacing;
                if (spawnCount >= currentSoldiers) { break; }
                //GameObject soldier = Instantiate(soldierPrefab, currentSpawnPoint, Quaternion.identity, transform);
                GameObject soldier = Instantiate(soldierPrefab, currentSpawnPoint, rotationDirection, transform);
                spawnCount++;
                soldiers[i, j] = soldier;
                //Debug.Log($"currentSpawn: {currentSpawnPoint} ij values: {i},{j}");
            }
        }

        isDeployed = true;
    }

    public Vector3[,] getFormationPoints(Vector3 pInitSpawnPoint, Vector3 pEndPoint,bool keepPreviousFormation)
    {
        int rows = Mathf.CeilToInt(((float)currentSoldiers / (float)columns));
        int spawnCount = 0;
        Vector3 initSpawn = pInitSpawnPoint;
        Vector3 currentSpawnPoint = pInitSpawnPoint;
        Vector3[,] formationMatrix = new Vector3[rows, columns];

        Vector3 direction = pEndPoint - pInitSpawnPoint;
        direction = Vector3.Normalize(direction);
        if (keepPreviousFormation)
        {
            direction = previousDirection;
        }
        previousDirection = direction;


        Vector3 depthDirection = Vector3.Cross(direction, Vector3.up);
        depthDirection = -Vector3.Normalize(depthDirection);

        //find rotation
        Quaternion rotationDirection = Quaternion.LookRotation(-depthDirection);

        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            initSpawn = pInitSpawnPoint + depthDirection * i * unitZSpacing;
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                currentSpawnPoint = initSpawn + direction * j * unitXSpacing;
                if (spawnCount >= currentSoldiers) { break; }
                formationMatrix[i,j] = currentSpawnPoint;
                spawnCount++;
            }
        }

        return formationMatrix;
    }




    public void deleteCurrentSoldiers()
    {
        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                Destroy(soldiers[i, j]);
            }

        }
    }

}
