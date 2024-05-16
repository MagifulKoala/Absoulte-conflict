using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject soldierPrefab;
    float totalUnitSize = 10;
    float currentSoldiers = 10;
    string status;
    int rows;
    int columns;
    float unitSize;
    GameObject[,] soldiers;

    private void Start()
    {
        initFormation();
    }

    void initFormation()
    {
        columns = Mathf.CeilToInt(totalUnitSize / 2);
        rows = Mathf.CeilToInt(totalUnitSize / columns);

        soldiers = new GameObject[rows, columns];

        for (int i = 0; i < soldiers.Length; i++)
        {
            for (int j = 0; j < soldiers.Length; j++)
            {
                soldiers[i, j] = soldierPrefab;
            }

        }
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

}
