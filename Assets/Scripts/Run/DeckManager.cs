using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<BlockData> deck;
    private List<BlockData> discardPile;
    private RunData runData;

    // �� �ʱ�ȭ
    public void Initialize(RunData data)
    {

    }

    // ��� �̱�
    public BlockData DrawBlock()
    {
        return null;
    }

    // �� ����
    private void ShuffleDeck()
    {

    }

    // �� ����
    public bool RerollDeck()
    {
        return false;
    }

    // ��� ���� ó��
    public void ProcessBlockReuse(string blockId)
    {

    }

    // ��� �߰�
    public void AddBlock(BlockData block)
    {

    }

    // ��� ����
    public void RemoveBlock(BlockData block)
    {

    }
}