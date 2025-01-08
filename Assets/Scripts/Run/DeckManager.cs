using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
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
        drawBlockCount = 3;

        ShuffleDeck();
    }

    // 블록 뽑기
    public List<Block> DrawBlock()
    {
        List<Block> drawedBlocks = new List<Block>();

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

            drawedBlocks.Add(block);
        }

        return drawedBlocks;
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

    // 덱 리롤
    public bool RerollDeck()
    {
        return false;
    }

    // 블록 재사용 처리
    public void ProcessBlockReuse(string blockId)
    {

    }

    // 블록 추가
    public void AddBlock(BlockData block)
    {

    }

    // 블록 제거
    public void RemoveBlock(BlockData block)
    {

    }

    private List<Block> InstantiateBlocks(List<BlockData> blockDatas)
    {
        List<Block> blocks = new List<Block>();

        foreach (BlockData blockData in blockDatas)
        {
            GameObject blockObject = Instantiate(blockData.prefab);
            Block block = blockObject.GetComponent<Block>();
            BlockType blockType = blockData.type;
            block.Initialize(blockData, idCount, runData.blockReuses[blockType]);
            idCount++;
            blocks.Add(block);

            // TEST
            blockObject.transform.position = new Vector3(10, 10);
        }

        return blocks;
    }
}