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
    public int columns{get; private set;}
    public bool isDeployed{get; private set;}
    GameObject[,] soldiers;

    private void OnEnable()
    {
        currentSoldiers = totalUnitSize;
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
        Debug.Log($"currSoldier: {currentSoldiers}, cols: {pNewColumns}, curr/cols: {((float)currentSoldiers/(float)pNewColumns)}");
        rows = Mathf.CeilToInt(((float)currentSoldiers / (float)pNewColumns));
        int spawnCount = 0;
        GameObject[,] tempFormation = new GameObject[rows, pNewColumns];
        Queue<GameObject> soldierQueue = addSoldiersToQueue();


        Debug.Log($"tempFormation size: {rows*pNewColumns}, rc: {rows},{pNewColumns}, armySize: {currentSoldiers}");

        for (int i = 0; i < tempFormation.GetLength(0); i++)
        {
            for (int j = 0; j < tempFormation.GetLength(1); j++)
            {
                if(spawnCount >= currentSoldiers){break;}
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

    public void spawnUnitOnPoint(Vector3 pInitSpawnPoint, Vector3 pEndPoint)
    {
        Debug.Log($"spawnUnitOnPoint called. Unit id: {unitId}");
        Vector3 initSpawn = pInitSpawnPoint;
        Vector3 currentSpawnPoint = pInitSpawnPoint;
        int spawnCount = 0;
        //Debug.Log($"initSpawnPoint: {pInitSpawnPoint}");
        Vector3 direction = pEndPoint - currentSpawnPoint;
        direction = Vector3.Normalize(direction);

        Vector3 depthDirection = Vector3.Cross(direction, Vector3.up);
        depthDirection = -Vector3.Normalize(depthDirection);

        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            //currentSpawnPoint.z = pInitSpawnPoint.z - i * unitZSpacing;
            initSpawn = pInitSpawnPoint + depthDirection * i * unitZSpacing;
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                //currentSpawnPoint.x = pInitSpawnPoint.x + j * unitXSpacing;
                currentSpawnPoint = initSpawn + direction * j * unitXSpacing;
                if(spawnCount >= currentSoldiers){break;}
                //GameObject soldier = Instantiate(soldierPrefab, currentSpawnPoint, Quaternion.identity, transform);
                GameObject soldier = Instantiate(soldierPrefab, currentSpawnPoint, Quaternion.identity, transform);
                spawnCount++;
                soldiers[i, j] = soldier;
                //Debug.Log($"currentSpawn: {currentSpawnPoint} ij values: {i},{j}");
            }
        }

        isDeployed = true;

    }

    public void deleteCurrentSoldiers()
    {
        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                Destroy(soldiers[i,j]);
            }
            
        }
    }

}
