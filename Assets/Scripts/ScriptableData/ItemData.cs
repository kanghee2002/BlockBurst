using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "BlockBurst/Item")]
public class ItemData : ScriptableObject
{
    public string id;                  // 아이디
    [TextArea] public string itemName;            // 아이템 이름
    public List<EffectData> effects;   // 효과
    public int cost;                   // 가격
    public ItemType type;              // 타입
    public BlockData block;            // 블록
    public bool isPurchasableAgain;    // 버리고 다시 구매 가능한지 (아이템만)
    public ItemEffectType effectType;  // 효과 타입
    public ItemRarity rarity;          // 레어도
}
