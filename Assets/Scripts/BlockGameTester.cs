using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameTester : MonoBehaviour
{
    [SerializeField] private Board board;

    [SerializeField] private List<BlockData> blockDatas;

    [SerializeField] private Transform currentCursor;

    [SerializeField] private EffectData[] effects;

    private List<Block> deck;

    private List<Block> currentBlocks;

    private int currentBlockIndex;

    private Vector2Int currentOriginCellPosition;

    private GameData gameData;
    private RunData runData;
    private BlockGameData blockGameData;

    private void Start()
    {
        // Deck
        deck = InstantiateBlocks(blockDatas);
        ShuffleDeck();

        // Data
        gameData = new GameData();
        gameData.Initialize();
        gameData.defaultBlockScores = new Dictionary<BlockType, int>()
        {

            { BlockType.I, 10 },
            { BlockType.O, 10 },
            { BlockType.Z, 10 },
            { BlockType.S, 10 },
            { BlockType.J, 10 },
            { BlockType.L, 10 },
            { BlockType.T, 10 },
        };
        runData = new RunData();
        runData.Initialize(gameData);
        blockGameData = new BlockGameData();
        blockGameData.Initialize(runData);

        // Effect
        EffectManager.instance.Initialize(ref runData);
        EffectManager.instance.InitializeBlockGameData(ref blockGameData);

        ApplyEffects();

        // Board
        board.Initialize(blockGameData);

        // Tester
        currentBlocks = new()
        {
            GetBlock(),
            GetBlock(),
            GetBlock(),
        };
        SetBlockPosition();

        currentBlockIndex = 0;

        currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);

        currentOriginCellPosition = new Vector2Int(0, 0);

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
            if (currentBlockIndex >= currentBlocks.Count) 
                currentBlockIndex = currentBlocks.Count - 1;
            currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);
        }

        // 리롤
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Block block in currentBlocks)
            {
                block.transform.position = new Vector2(10, 10);
            }

            //deckManager.RerollBlock();

            SetBlockPosition();
        }

        // 블록 배치
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Block placingBlock = currentBlocks[currentBlockIndex];

            bool result = board.PlaceBlock(placingBlock.GetComponent<Block>(), currentOriginCellPosition);

            if (result)
            {
                currentBlocks.RemoveAt(currentBlockIndex);
                placingBlock.gameObject.SetActive(false);

                currentBlockIndex--;
                if (currentBlockIndex < 0) currentBlockIndex = 0;
                currentCursor.position = new Vector2(14f, -3.5f * currentBlockIndex);

                SetBlockPosition();
            }

            if (currentBlocks.Count == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    currentBlocks.Add(GetBlock());
                }

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
        for (int i = 0; i < currentBlocks.Count; i++)
        {
            currentBlocks[i].transform.position = new Vector2(10f, -3.5f * i);
        }
    }

    private List<Block> InstantiateBlocks(List<BlockData> blockData)
    {
        List<Block> blocks = new List<Block>();

        foreach (BlockData data in blockData)
        {
            GameObject blockObject = Instantiate(data.prefab);
            Block block = blockObject.GetComponent<Block>();
            BlockType blockType = data.type;
            block.Initialize(data, 0, 0);
            blocks.Add(block);

            // TEST
            blockObject.transform.position = new Vector3(10, 10);
        }

        return blocks;
    }

    private Block GetBlock()
    {
        Block block = deck[0];
        deck.RemoveAt(0);
        return block;
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int j = Random.Range(i, deck.Count);
            Block tmp = deck[i];
            deck[i] = deck[j];
            deck[j] = tmp;
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

    private void ApplyEffects()
    {
        foreach (var effect in effects)
        {
            EffectManager.instance.AddEffect(effect);
            if (effect.trigger == TriggerType.ON_ACQUIRE)
            {
                EffectManager.instance.ApplyEffect(effect);
            }
        }
    }
}
