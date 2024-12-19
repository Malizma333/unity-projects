using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GenerateGrid : MonoBehaviour
{
    [Serializable]
    public struct Category
    {
        public string title;
        public string[] items;
    }

    [Space(10)]
    [Header("References")]
    [SerializeField] private Transform tablePrefab;
    [SerializeField] private Transform containerPrefab;
    [SerializeField] private Transform cellPrefab;
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform textPrefab;
    [SerializeField] private Transform textCanvas;
    [SerializeField] private CellManager cellManager;

    [Space(10)]
    [SerializeField] private Category[] categories;

    private Transform tableTransform;
    private int numCategories;
    private int numItems;

    public int Categories { get { return numCategories; } }
    public int Items { get { return numItems; } }
    public int GridLength { get { return (numCategories - 1) * numItems; } }
    public Transform Table { get { return tableTransform; } }
    public Category[] CategoryArr { get { return categories; } }

    private void Start()
    {
        defineReferences();
        generateCells();
        generateLabels();
        generateTable();
    }

    //defines necessary references
    private void defineReferences()
    {
        numCategories = categories.Length;
        numItems = categories[0].items.Length;
    }

    //generates a grid of useable cells
    private void generateCells()
    {
        CellGameObject[,] cellList = new CellGameObject[GridLength,GridLength];

        for (int i = 0; i < GridLength; i++)
        {
            for (int j = 0; j < GridLength; j++)
            {
                if (i / Items <= (GridLength - j - 1) / Items)
                {
                    if (i % Items == 0 && j % Items == 0)
                    {
                        Instantiate(
                            containerPrefab,
                            new Vector2(i + 2, -j - 2),
                            Quaternion.identity
                        );
                    }

                    cellList[i, j] = createCell(
                        new Vector3(i, -j, -1), i, j
                    );
                }
            }
        }

        cellManager.GridArray = cellList;
    }

    //creates a single cell
    private CellGameObject createCell(Vector3 pos, int i, int j)
    {
        Transform cellTransform = Instantiate(cellPrefab, cellParent);

        cellTransform.position = pos;
        cellTransform.GetComponent<CellGameObject>().Index = new Vector2(i, j);

        return cellTransform.GetComponent<CellGameObject>();
    }

    //generates labels for each row/column
    private void generateLabels()
    {
        for (int i = 1; i < numCategories; i++)
        {
            for (int j = 0; j < numItems; j++)
            {
                createLabel(
                    new Vector3((i - 1) * numItems + j, 1, 0),
                    Quaternion.Euler(0, 0, 90),
                    categories[i].items[j],
                    new Vector4(0, 0, -75, 0),
                    TextOverflowModes.Overflow
                );
            }
        }

        for (int j = 1; j < numItems + 1; j++)
        {
            createLabel(
                new Vector3(-1, j - numItems, 0),
                Quaternion.Euler(0, 0, 0),
                categories[0].items[numItems - j],
                new Vector4(-75, 0, 0, 0),
                TextOverflowModes.Page
            );

            for (int i = 2; i < numCategories; i++)
            {
                createLabel(
                    new Vector3(-1, numItems * (i - 2) + j - GridLength, 0),
                    Quaternion.Euler(0, 0, 0),
                    categories[i].items[numItems - j],
                    new Vector4(-75, 0, 0, 0),
                    TextOverflowModes.Page
                );
            }
        }
    }

    //creates a single label
    private void createLabel(Vector3 pos, Quaternion rot, string txt, Vector4 margin, TextOverflowModes overflow)
    {
        Transform textTransform = Instantiate(textPrefab, textCanvas);

        textTransform.position = pos;
        textTransform.rotation = rot;

        TextMeshProUGUI tmp = textTransform.GetComponent<TextMeshProUGUI>();

        tmp.text = txt;
        tmp.margin = margin;
        tmp.overflowMode = overflow;
    }

    //generates a table with categories/items in each
    private void generateTable()
    {
        tableTransform = Instantiate(tablePrefab, textCanvas);

        tableTransform.GetComponent<RectTransform>().sizeDelta =
            new Vector2((numItems + 1) * 100, numCategories * 20);

        tableTransform.position = new Vector2(GridLength, -GridLength);

        for(int i = 0; i < numCategories; i++)
        {
            for(int j = 0; j < numItems + 1; j++)
            {
                Transform textTransform = Instantiate(textPrefab, tableTransform);

                SpriteRenderer squareSprite = textTransform.gameObject.GetComponentInChildren<SpriteRenderer>();
                
                squareSprite.enabled = true;
                squareSprite.GetComponent<RectTransform>().localScale = .9f * new Vector2(100, 20);

                TextMeshProUGUI tmp = textTransform.GetComponent<TextMeshProUGUI>();

                tmp.text = "";
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.overflowMode = TextOverflowModes.Page;
            }
        }
    }
}
