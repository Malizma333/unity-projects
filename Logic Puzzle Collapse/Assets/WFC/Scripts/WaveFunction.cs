using System.Collections.Generic;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    [System.Serializable]
    private struct Tile { public Sprite sprite; public bool[] sockets; }

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform cellParent;

    [Space(10)]
    [SerializeField][Range(2, 10)] private int size = 2;
    [SerializeField] private Tile[] tiles;

    private int[,] grid;

    private void Start()
    {
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                GameObject currentCell = Instantiate(cellPrefab, cellParent);
                currentCell.transform.position = new Vector3(i, j, 0);
            }
        }
    }

    public void Generate()
    {
        Initialize();

        SelectRandom();

        Collapse();

        UpdateSprites();
    }

    private void Initialize()
    {
        grid = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                grid[i, j] = (int)Mathf.Pow(2, tiles.Length) - 1;
            }
        }
    }

    private void SelectRandom()
    {
        int minimumEntropy = minEntropy(grid);
        List<int[]> cellOptions = new List<int[]>();

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                if (cellPositions(grid[i,j]) == minimumEntropy)
                {
                    cellOptions.Add(new int[] { i, j });
                }
            }
        }

        int[] choice = cellOptions[Random.Range(0, cellOptions.Count)];
        List<int> collapseOptions = new List<int>();

        for(int i = 1; i < tiles.Length; i*=2)
        {
            if ((grid[choice[0], choice[1]] & i) != 0)
            {
                collapseOptions.Add(i);
            }
        }

        grid[choice[0], choice[1]] = collapseOptions[Random.Range(0, collapseOptions.Count)];
    }

    private int minEntropy(int[,] arr)
    {
        int min = tiles.Length;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int count = cellPositions(arr[i, j]);

                if(min > count && count > 1)
                {
                    min = count;
                }
            }
        }

        return min;
    }

    private int cellPositions(int cellState)
    {
        int count = 0;
        int maxCell = (int) Mathf.Pow(2, tiles.Length);

        for (int k = 1; k < maxCell; k *= 2)
        {
            if ((cellState & k) != 0)
            {
                count++;
            }
        }

        return count;
    }

    private void Collapse()
    {

    }

    private void UpdateSprites()
    {

    }
}
