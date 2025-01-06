using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<BlockData> deck;
    private List<BlockData> discardPile;
    private RunData runData;

    // 덱 초기화
    public void Initialize(RunData data)
    {

    }

    // 블록 뽑기
    public BlockData DrawBlock()
    {
        return null;
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
}