using TMPro;
using UnityEngine;

using static CellGameObject;
using static GenerateGrid;

public class CellManager : MonoBehaviour
{
    [SerializeField] private GenerateGrid generator;

    private Category[] categories;
    private CellGameObject[,] logicGrid;
    private string[,] tableValues;
    private Transform tableTransform;
    private int items, catgs, gLength;

    public CellGameObject[,] GridArray
    {
        get { return logicGrid; }
        set { logicGrid = value; }
    }

    /*
     * 
     * Elimination - If one trait is related to another, eliminate
     * all other possibilities for each trait to be releated to
     * any other
     * 
     * Elimination Exclude - Circle -> 8 Cross
     * Elimination Deduce - 4 Cross -> Circle
     * 
     * Relation - If one trait relates to another, and the same trait
     * relates/does not relate to a different trait, then the other
     * trait does not relate to that different trait
     * 
     * Relation Relate -> Circle Cross (any order) -> Cross
     * Relation Isolate -> Circle Circle (any order) -> Circle
     * 
     */

    private void Start()
    {
        items = generator.Items;
        catgs = generator.Categories;
        gLength = generator.GridLength;
        tableTransform = generator.Table;
        categories = generator.CategoryArr;

        tableValues = new string[items + 1, catgs + 1];

        for (int i = 0; i < catgs; i++)
        {
            tableValues[0, i] = categories[i].title;
        }

        for(int i = 0; i < items; i++)
        {
            tableValues[i + 1, 0] = categories[0].items[i];
        }
    }

    public void Solve()
    {
        Exclude();
        Deduce();
        Relate();
        FillTable();
    }

    private void Exclude()
    {
        for(int i = 0; i < gLength; i++)
        {
            for (int j = 0; j < gLength; j++)
            {
                if (i <= gLength - j - 1 && logicGrid[i, j].CellState == State.Valid)
                {
                    for (int n = 0; n < items; n++)
                    {
                        logicGrid[i, n + items * (j / items)].SetCellState(false);
                        logicGrid[n + items * (i / items), j].SetCellState(false);
                    }
                }
            }
        }
    }

    private void Deduce()
    {
        int invalidCount, nCache;

        for (int i = 0; i < gLength; i += items)
        {
            for (int j = 0; j < gLength; j += items)
            {
                if (i <= gLength - j - 1)
                {
                    for (int m = 0; m < items; m++)
                    {
                        invalidCount = 0;
                        nCache = 0;

                        for (int n = 0; n < items; n++)
                        {
                            if (logicGrid[i + n, j + m].CellState == State.Invalid)
                            {
                                invalidCount++;
                            }
                            else
                            {
                                nCache = n;
                            }
                        }

                        if (invalidCount == items - 1)
                        {
                            logicGrid[i + nCache, j + m].SetCellState(true);
                        }

                        invalidCount = 0;
                        nCache = 0;

                        for (int n = 0; n < items; n++)
                        {
                            if (logicGrid[i + m, j + n].CellState == State.Invalid)
                            {
                                invalidCount++;
                            }
                            else
                            {
                                nCache = n;
                            }
                        }

                        if (invalidCount == items - 1)
                        {
                            logicGrid[i + m, j + nCache].SetCellState(true);
                        }
                    }
                }
            }
        }
    }

    private void Relate()
    {
        for(int i = 0; i < catgs - 2; i++)
        {
            for(int j = 0; j < catgs - 2 - i; j++)
            {
                for(int m = 1; m < catgs - i - j - 1; m++)
                {
                     checkTriangle(i, j, m, (catgs - i - j - 1) - m);
                }
            }
        }
    }

    private void checkTriangle(int i, int j, int m, int n)
    {

        //Check TR for valid cell

        for (int x1 = (i + m) * items; x1 < (i + m + 1) * items; x1++)
        {
            for (int y1 = j * items; y1 < (j + 1) * items; y1++)
            {
                if (logicGrid[x1, y1].CellState == State.Valid)
                {

                    //check BL for valid cell

                    for(int x2 = i * items; x2 < (i + 1) * items; x2++)
                    {
                        int y2 = x1 % items + items * (n + y1 / items);

                        //fill TL

                        if (logicGrid[x2, y2].CellState == State.Valid)
                        {
                            logicGrid[x2, y1].SetCellState(true);
                        }

                        if (logicGrid[x2, y2].CellState == State.Invalid)
                        {
                            logicGrid[x2, y1].SetCellState(false);
                        }

                        if (logicGrid[x2, y1].CellState == State.Invalid)
                        {
                            logicGrid[x2, y2].SetCellState(false);
                        }
                    }
                }
            }
        }

        //check BL for valid cell

        for (int x1 = i * items; x1 < (i + 1) * items; x1++)
        {
            for (int y1 = (j + n) * items; y1 < (j + n + 1) * items; y1++)
            {
                if (logicGrid[x1, y1].CellState == State.Valid)
                {
                    //check TL for valid cell

                    for (int y2 = j * items; y2 < (j + 1) * items; y2++)
                    {
                        int x2 = y1 % items + items * (m + x1 / items);

                        //fill TR

                        if (logicGrid[x1, y2].CellState == State.Valid)
                        {
                            logicGrid[x2, y2].SetCellState(true);
                        }

                        if (logicGrid[x1, y2].CellState == State.Invalid)
                        {
                            logicGrid[x2, y2].SetCellState(false);
                        }

                        if (logicGrid[x2, y2].CellState == State.Invalid)
                        {
                            logicGrid[x1, y2].SetCellState(false);
                        }
                    }
                }
            }
        }

        //check TL for valid cell

        for (int x1 = i * items; x1 < (i + 1) * items; x1++)
        {
            for (int y1 = j * items; y1 < (j + 1) * items; y1++)
            {
                if (logicGrid[x1, y1].CellState == State.Valid)
                {
                    //check TR for valid cell

                    for (int x2 = (i + m) * items; x2 < (i + m + 1) * items; x2++)
                    {
                        int y2 = x2 % items + items * (n + y1 / items);

                        //fill BL

                        if (logicGrid[x2, y1].CellState == State.Valid)
                        {
                            logicGrid[x1, y2].SetCellState(true);
                        }

                        if (logicGrid[x2, y1].CellState == State.Invalid)
                        {
                            logicGrid[x1, y2].SetCellState(false);
                        }

                        if (logicGrid[x1, y2].CellState == State.Invalid)
                        {
                            logicGrid[x2, y1].SetCellState(false);
                        }
                    }
                }
            }
        }
    }

    private void FillTable()
    {
        for (int i = 0; i < items; i++)
        {
            for(int j = 0; j < gLength; j++)
            {
                if(logicGrid[j, i].CellState == State.Valid)
                {
                    tableValues[i + 1, j / items + 1] = categories[j / items + 1].items[j % items];
                }
            }
        }

        for(int i = 0; i < gLength + items + catgs; i++)
        {
            TextMeshProUGUI textComp = tableTransform.GetChild(i).GetComponent<TextMeshProUGUI>();

            textComp.text = tableValues[i % (items + 1), i / (items + 1)];
        }
    }
}
