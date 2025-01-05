using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Block : MonoBehaviour
{
    public BlockType Type { get; private set; }
    public bool[,] Shape { get; private set; }

    public GameObject originCell;      // 블록 모양의 기준 칸
    
    public void Initialize(BlockData blockData)
    {
        Type = blockData.type;
        MakeShapeArray(blockData.shape);
    }

    /// <summary>
    /// 블록을 시계방향으로 90도 회전시킴
    /// </summary>
    public void RotateShape()
    {
        int width = Shape.GetLength(0);
        int height = Shape.GetLength(1);

        bool[,] newShape = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newShape[height - 1 - y, x] = Shape[x, y];
            }
        }

        Shape = newShape;

        transform.Rotate(0f, 0f, -90f);
    }

    private void MakeShapeArray(Vector2Int[] shape)
    {
        int maxX = shape.Max(s => s.x);
        int maxY = shape.Max(s => s.y);

        Shape = new bool[maxX + 1, maxY + 1];

        foreach (Vector2Int s in shape) 
        {
            Shape[s.x, s.y] = true;
        }
    }
}
