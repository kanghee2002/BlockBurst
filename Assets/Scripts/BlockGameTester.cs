using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameTester : MonoBehaviour
{
    [SerializeField] private Board board;

    [SerializeField] private DeckManager deckManager;

    [SerializeField] private List<BlockData> blockDatas;

    [SerializeField] private Transform currentCursor;

    private int currentBlockIndex;

    private Vector2Int currentOriginCellPosition;

    private RunData runData;
    private BlockGameData blockGameData;

    private void Start()
    {
        currentBlockIndex = 0;

        currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);

        currentOriginCellPosition = new Vector2Int(0, 0);

        runData = new RunData()
        {
            availableBlocks = blockDatas,
            baseBlockScores = new Dictionary<BlockType, int>()
            {
                { BlockType.I, 10 },
                { BlockType.O, 15 },
                { BlockType.Z, 25 },
                { BlockType.S, 25 },
                { BlockType.J, 20 },
                { BlockType.L, 20 },
                { BlockType.T, 20 },
            },
            baseRerollCount = 3,
            baseMultiplier = 1,
            blockReuses = new Dictionary<BlockType, int>()
            {
                { BlockType.I, 0 },
                { BlockType.O, 0 },
                { BlockType.Z, 0 },
                { BlockType.S, 0 },
                { BlockType.J, 0 },
                { BlockType.L, 0 },
                { BlockType.T, 0 },
            },
            baseMatchMultipliers = new Dictionary<MatchType, float>()
            {
                { MatchType.ROW, 1f },
                { MatchType.COLUMN, 1f },
                { MatchType.SQUARE, 1f }
            }
        };

        blockGameData = new BlockGameData();
        blockGameData.Initialize(runData);
        board.Initialize(blockGameData);

        deckManager.Initialize(runData);
    }

    private void Update()
    {
        HandleArrowInput();

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentBlockIndex--;
            if (currentBlockIndex < 0) currentBlockIndex = 0;
            currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            currentBlockIndex++;
            if (currentBlockIndex >= deckManager.currentBlocks.Count) 
                currentBlockIndex = deckManager.currentBlocks.Count - 1;
            currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            deckManager.DrawBlock();

            SetBlockPosition();
        }

        // 리롤
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Block block in deckManager.currentBlocks)
            {
                block.transform.position = new Vector2(10, 10);
            }

            deckManager.RerollBlock();

            SetBlockPosition();
        }

        // 블록 배치
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Block placingBlock = deckManager.currentBlocks[currentBlockIndex];

            bool result = board.PlaceBlock(placingBlock.GetComponent<Block>(), currentOriginCellPosition);

            if (result)
            {
                deckManager.UseBlock(placingBlock);

                currentBlockIndex--;
                if (currentBlockIndex < 0) currentBlockIndex = 0;
                currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);

                SetBlockPosition();
            }

            if (deckManager.currentBlocks.Count == 0)
            {
                SetBlockPosition();
                currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);
            }
        }
    }

    private void HandleArrowInput()
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

    private void SetBlockPosition()
    {
        for (int i = 0; i < deckManager.currentBlocks.Count; i++)
        {
            deckManager.currentBlocks[i].transform.position = new Vector2(10f, -3.5f * i);
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
