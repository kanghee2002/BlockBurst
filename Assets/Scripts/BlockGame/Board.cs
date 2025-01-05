using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Cell[,] cells {  get; private set; }

    public bool PlaceBlock(Block block, Vector2Int pos)
    {
        bool isPlaced = false;

        if (CanPlace(block, pos))
        {
            isPlaced = true;
            foreach (Vector2Int shape in block.Shape)
            {
                Vector2Int curPos = pos + shape;

                cells[curPos.x, curPos.y].SetBlock();
            }
        }

        ProcessMatches();

        return isPlaced;
    }

    private bool CanPlace(Block block, Vector2Int pos)
    {
        foreach (Vector2Int shape in block.Shape)
        {
            Vector2Int curPos = pos + shape;

            if (IsOutOfBoard(curPos)) return false;

            if (IsBlocked(curPos)) return false;
        }

        return true;
    }
    
    public void ProcessMatches()
    {
        // Temporary Implement

        for (int i = 0; i < 8; i++)
        {
            // Check Row
            bool rowLine = true;
            for (int j = 0; j < 8; j++)
            {
                if (!cells[i, j].IsBlocked)
                {
                    rowLine = false;
                    break;
                }
            }
            if (rowLine)
            {
                matcedLineCount++;
                for (int j = 0; j < 8; j++)
                {
                    cells[i, j].ClearBlock();
                }
            }

            // Check Column
            bool colLine = true;
            for (int j = 0; j < 8; j++)
            {
                if (!cells[j, i].IsBlocked)
                {
                    colLine = false;
                    break;
                }
            }
            if (colLine)
            {
                matcedLineCount++;
                for (int j = 0; j < 8; j++)
                {
                    cells[j, i].ClearBlock();
                }
            }
        }

        Debug.Log(matcedLineCount);
    }

    private bool IsOutOfBoard(Vector2Int pos)
    {
        // Need to change 8 to board size
        if (pos.x < 0 || pos.x >= 8 || pos.y < 0 || pos.y >= 8)
            return true;
        else
            return false;
    }

    private bool IsBlocked(Vector2Int pos)
    {
        if (cells[pos.x, pos.y].IsBlocked) return true;
        else return false;
    }

    // Test Code  ////////////////////////////////////////////////////////////
    [SerializeField] private Cell[] tmpCells;

    [SerializeField] private BlockData[] blocks;

    private List<GameObject> blockObjects;

    private int matcedLineCount = 0;

    private void Start()
    {
        cells = new Cell[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tmpCells[i * 8 + j].transform.position = new Vector3(j, -i);
                
                /*
                if ((i + j) % 2 == 0)
                {
                    tmpCells[i * 8 + j].GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
                }*/

                tmpCells[i * 8 + j].cellPosition = new Vector2Int(j, i);
                cells[j, i] = tmpCells[i * 8 + j];
            }
        }


        blockObjects = new List<GameObject>();
        foreach (BlockData block in blocks)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject obj = Instantiate(block.prefab);
                blockObjects.Add(obj);
                obj.transform.position = new Vector3(10, 10);

                obj.GetComponent<Block>().Initialize(block);
            }
        }
    }

    public GameObject GetRandomBlock()
    {
        int randomIdx = Random.Range(0, blockObjects.Count);

        GameObject blockObj = blockObjects[randomIdx];
        blockObjects.RemoveAt(randomIdx);

        return blockObj;
    }
    //////////////////////////////////////////////////////////////////////
}
