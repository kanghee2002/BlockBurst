using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "BlockBurst/Block")]
public class BlockData : ScriptableObject
{
    public string id;
    public BlockType type;                 // 블록 타입
    public Vector2Int[] shape;             // 블록 모양
    public int reuseCount;                 // 재사용 횟수
    public List<EffectData> effects;       // 블록 효과

    public GameObject prefab;

    public BlockData Clone()
    {
        BlockData newBlock = CreateInstance<BlockData>();

        newBlock.id = id;
        newBlock.type = type;
        newBlock.shape = (Vector2Int[])shape.Clone();
        newBlock.reuseCount = reuseCount;
        newBlock.effects = new List<EffectData>(effects);

        newBlock.prefab = prefab;

        return newBlock;
    }
}
