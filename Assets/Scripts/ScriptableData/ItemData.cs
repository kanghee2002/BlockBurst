using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "BlockBurst/Item")]
public class ItemData : ScriptableObject
{
    public string id;                  // 아이디
    public List<EffectData> effects;   // 효과
    public int cost;                   // 가격
    public ItemType type;              // 타입
    public BlockData block;            // 블록
}
