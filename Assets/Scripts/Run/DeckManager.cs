using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Block> currentBlocks { get; private set; }
    public int rerollCount { get; private set; }
    public int drawBlockCount { get; private set; }

    private List<Block> deck;
    private List<Block> discardPile;
    private RunData runData;

    private int idCount;

    // 덱 초기화
    public void Initialize(RunData data)
    {
        runData = data;
        deck = InstantiateBlocks(data.availableBlocks);
        discardPile = new List<Block>();
        idCount = 0;

        currentBlocks = new List<Block>();
        rerollCount = 3;
        drawBlockCount = 3;

        ShuffleDeck();
    }

    // 블록 뽑기
    public List<Block> DrawBlock()
    {
        for (int i = 0; i < drawBlockCount; i++)
        {
            if (deck.Count == 0)
            {
                Debug.Log("덱에 남은 블록 없음");
                break;
            }

            Block block = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);

            int rotateCount = Random.Range(0, 4);
            for (int j = 0; j < rotateCount; j++)
            {
                block.RotateShape();
            }

            currentBlocks.Add(block);
        }

        return currentBlocks;
    }

    // 덱 리롤
    public List<Block> RerollBlock()
    {
        if (rerollCount == 0)
        {
            return currentBlocks;
        }

        foreach (Block block in currentBlocks)
        {
            int randomIndex = Random.Range(0, deck.Count + 1);
            deck.Insert(randomIndex, block);
        }

        rerollCount--;

        return DrawBlock();
    }

    // 블록 사용 처리
    public void UseBlock(Block block)
    {
        for (int i = 0; i < currentBlocks.Count; i++)
        {
            if (currentBlocks[i].Id == block.Id)
            {
                currentBlocks[i].gameObject.SetActive(false);
                currentBlocks.RemoveAt(i);
            }
        }

        if (currentBlocks.Count == 0)
        {
            DrawBlock();
        }
    }

    // 블록 재사용 처리
    public void ProcessBlockReuse(int blockId)
    {

    }

    // 블록 추가
    public void AddBlock(Block block)
    {

    }

    // 블록 삭제
    public void DeleteBlock(Block block)
    {

    }

    private List<Block> InstantiateBlocks(List<BlockData> blockData)
    {
        List<Block> blocks = new List<Block>();

        foreach (BlockData data in blockData)
        {
            GameObject blockObject = Instantiate(data.prefab);
            Block block = blockObject.GetComponent<Block>();
            BlockType blockType = data.type;
            block.Initialize(data, idCount, runData.blockReuses[blockType]);
            idCount++;
            blocks.Add(block);

            // TEST
            blockObject.transform.position = new Vector3(10, 10);
        }

        return blocks;
    }

    // 덱 셔플
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
}