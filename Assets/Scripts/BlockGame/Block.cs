using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType Type { get; private set; }
    public Vector2Int[] Shape { get; private set; }

    public GameObject originCell;      // 블록 모양의 기준 칸

    public void Initialize(BlockData blockData)
    {
        Type = blockData.type;
        Shape = blockData.shape;
    }
}
