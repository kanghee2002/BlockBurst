using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUI : MonoBehaviour
{
    public void UpdateDeck(List<BlockData> deck);
    public void UpdateRerollCount(int count);
    public void ShowCurrentBlock(BlockData block);
    public void ShowBlockDetail(BlockData block);
    public void UpdateBlockReuses(Dictionary<string, int> reuseCounts);
}
