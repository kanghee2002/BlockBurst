using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<Block> deck;
    private List<Block> discardPile;
    private RunData runData;

    private int idCount;

    // 덱 초기화
    public void Initialize(RunData data)
    {
        deck = InstantiateBlocks(data.availableBlocks);
        discardPile = new List<Block>();
        runData = data;
        idCount = 0;
    }

    // 블록 뽑기
    public Block DrawBlock()
    {
        if (deck.Count == 0)
        {
            Debug.Log("드로우 시도: 덱에 블록 없음");
            return null;
        }

        // TEST
        int randomIdx = Random.Range(0, deck.Count);
        Block block = deck[randomIdx];
        deck.RemoveAt(randomIdx);

        int rotateCount = Random.Range(0, 4);
        for (int i = 0; i < rotateCount; i++)
        {
            block.RotateShape();
        }

        return block;
    }

    // 덱 셔플
    private void ShuffleDeck()
    {

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