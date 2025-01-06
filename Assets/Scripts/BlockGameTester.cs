using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameTester : MonoBehaviour
{
    [SerializeField] private Board board;

    [SerializeField] private DeckManager deckManager;

    [SerializeField] private List<BlockData> blockDatas;

    private Block currentBlock;
    private Vector2Int currentOriginCellPosition;

    private RunData runData;

    private void Start()
    {
        currentOriginCellPosition = new Vector2Int(0, 0);

        runData = new RunData()
        {
            availableBlocks = blockDatas
        };
        deckManager.Initialize(runData);
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
            currentBlock = deckManager.DrawBlock();
            currentBlock.transform.position = new Vector2(10f, -3.5f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool result = board.PlaceBlock(currentBlock.GetComponent<Block>(), currentOriginCellPosition);

            if (result)
            {
                currentBlock.gameObject.SetActive(false);

                currentBlock = deckManager.DrawBlock();
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
        // TEST: 8 -> Board Size로 변경 필요
        if (pos.x < 0 || pos.x >= 8 || pos.y < 0 || pos.y >= 8)
            return true;
        else
            return false;
    }

}
