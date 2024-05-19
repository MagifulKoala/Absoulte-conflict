using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject soldierPrefab;
    public int unitId { get; private set; }
    public float unitXSpacing;
    public float unitZSpacing;
    public float totalUnitSize = 4;
    public float currentSoldiers = 4;
    string status;
    int rows;
    int columns;
    float unitSize;
    GameObject[,] soldiers;

    public Unit(GameObject soldierPrefab, float totalUnitSize, float currentSoldiers, string status, int rows, int columns, float unitSize, GameObject[,] soldiers)
    {
        this.soldierPrefab = soldierPrefab;
        this.totalUnitSize = totalUnitSize;
        this.currentSoldiers = currentSoldiers;
        this.status = status;
        this.rows = rows;
        this.columns = columns;
        this.unitSize = unitSize;
        this.soldiers = soldiers;
    }

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

    void updateFormation(int pNewColumns)
    {
        rows = Mathf.CeilToInt(totalUnitSize / columns);

        GameObject[,] tempFormation = new GameObject[rows, pNewColumns];
        Queue<GameObject> soldierQueue = addSoldiersToQueue();

        for (int i = 0; i < tempFormation.GetLength(0); i++)
        {
            for (int j = 0; j < tempFormation.GetLength(1); j++)
            {
                tempFormation[i, j] = soldierQueue.Dequeue();
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

    public void spawnUnitOnPoint(Vector3 pInitSpawnPoint)
    {
        Debug.Log($"spawnUnitOnPoint called. Unit id: {unitId}");
        Vector3 currentSpawnPoint = pInitSpawnPoint;
        int spawnCount = 0;
        //Debug.Log($"initSpawnPoint: {pInitSpawnPoint}");

        for (int i = 0; i < soldiers.GetLength(0); i++)
        {
            currentSpawnPoint.z = pInitSpawnPoint.z - i * unitZSpacing;
            for (int j = 0; j < soldiers.GetLength(1); j++)
            {
                currentSpawnPoint.x = pInitSpawnPoint.x + j * unitXSpacing;
                if(spawnCount >= currentSoldiers){break;}
                GameObject soldier = Instantiate(soldierPrefab, currentSpawnPoint, Quaternion.identity, transform);
                spawnCount++;
                soldiers[i, j] = soldier;
                Debug.Log($"currentSpawn: {currentSpawnPoint} ij values: {i},{j}");
            }
        }

    }

}
