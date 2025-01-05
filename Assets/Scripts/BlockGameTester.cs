using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameTester : MonoBehaviour
{
    [SerializeField] private Board board;

    private GameObject currentBlock;
    private Vector2Int currentOriginCellPosition;

    private void Start()
    {
        currentOriginCellPosition = new Vector2Int(0, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetCellColor(1f);


            currentOriginCellPosition = new Vector2Int(currentOriginCellPosition.x - 1, currentOriginCellPosition.y);

            if (currentOriginCellPosition.x < 0)
            {
                currentOriginCellPosition = new Vector2Int(0, currentOriginCellPosition.y);
            }

            SetCellColor(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetCellColor(1f);

            currentOriginCellPosition = new Vector2Int(currentOriginCellPosition.x + 1, currentOriginCellPosition.y);

            if (currentOriginCellPosition.x >= 8)
            {
                currentOriginCellPosition = new Vector2Int(7, currentOriginCellPosition.y);
            }
            SetCellColor(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetCellColor(1f);

            currentOriginCellPosition = new Vector2Int(currentOriginCellPosition.x, currentOriginCellPosition.y + 1);

            if (currentOriginCellPosition.y >= 8)
            {
                currentOriginCellPosition = new Vector2Int(currentOriginCellPosition.x, 7);
            }

            SetCellColor(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetCellColor(1f);

            currentOriginCellPosition = new Vector2Int(currentOriginCellPosition.x, currentOriginCellPosition.y - 1);

            if (currentOriginCellPosition.y < 0)
            {
                currentOriginCellPosition = new Vector2Int(currentOriginCellPosition.x, 0);
            }

            SetCellColor(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentBlock = board.GetRandomBlock();
            currentBlock.transform.position = new Vector2(10f, -3.5f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool result = board.PlaceBlock(currentBlock.GetComponent<Block>(), currentOriginCellPosition);

            if (result)
            {
                currentBlock.SetActive(false);

                currentBlock = board.GetRandomBlock();
                currentBlock.transform.position = new Vector2(10f, -3.5f);
            }
        }
    }

    private void SetCellColor(float color)
    {
        if (!IsOutOfBoard(currentOriginCellPosition))
        {
            if (board.cells[currentOriginCellPosition.x, currentOriginCellPosition.y].IsBlocked)
            {
                return;
            }
            board.cells[currentOriginCellPosition.x, currentOriginCellPosition.y].GetComponent<SpriteRenderer>().color
                = new Color(color, color, color);
        }
    }

    private bool IsOutOfBoard(Vector2Int pos)
    {
        // Need to change 8 to board size
        if (pos.x < 0 || pos.x >= 8 || pos.y < 0 || pos.y >= 8)
            return true;
        else
            return false;
    }

}
