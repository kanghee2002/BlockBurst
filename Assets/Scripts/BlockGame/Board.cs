using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Cell[,] cells {  get; private set; }




    // Test Code  ////////////////////////////////////////////////////////////
    [SerializeField] private Cell[] tmpCells;

    [SerializeField] private BlockData[] blocks;

    private List<GameObject> blockObjects;

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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < 3; i++)
            {
                int idx = Random.Range(0, blockObjects.Count);

                GameObject curBlock = blockObjects[idx];
                blockObjects.RemoveAt(idx);

                curBlock.transform.position = new Vector3(10, -3 * i);
            }
        }
    }
    //////////////////////////////////////////////////////////////////////
}
