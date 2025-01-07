using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "BlockBurst/Block")]
public class BlockData : ScriptableObject
{
    public BlockType type;                 // 블록 타입
    public Vector2Int[] shape;             // 블록 모양

    public GameObject prefab;
}
