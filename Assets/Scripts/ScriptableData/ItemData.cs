using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "BlockBurst/Item")]
public class ItemData : ScriptableObject
{
    public string id;                  // 아이템 ID
    public List<EffectData> effects;   // 아이템 효과
    public int cost;                   // 구매 비용
    public string targetBlockId;       // 대상 블록 ID (필요한 경우)
}