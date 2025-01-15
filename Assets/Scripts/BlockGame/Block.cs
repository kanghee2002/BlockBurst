using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Block
{
    public string Id { get; private set; }
    public BlockType Type { get; private set; }
    public Vector2Int[] Shape { get; private set; }
    public int ReuseCount { get; private set; }

    public void Initialize(BlockData blockData, int id)
    {
        Id = blockData.id + id.ToString();
        Type = blockData.type;
        Shape = blockData.shape.Clone() as Vector2Int[];
        ReuseCount = blockData.reuseCount;

        // 랜덤 위치로 회전
        for (int i = 0; i < Random.Range(0, 4); i++)
        {
            RotateShape();
        }
    }

    public void RotateShape()
    {
        Vector2Int[] rotatedShape = new Vector2Int[Shape.Length];
        for (int i = 0; i < Shape.Length; i++)
        {
            Vector2Int pos = Shape[i];
            rotatedShape[i] = new Vector2Int(-pos.y, pos.x);
        }
        Shape = rotatedShape;
    }

    /*
    public void TriggerEffects(TriggerType triggerType)
    {
        foreach (var effect in Effects)
        {
            if (effect.trigger == triggerType)
            {
                EffectManager.instance.TriggerEffects(triggerType, blockTypes: new BlockType[] { Type });
            }
        }
    }
    */
}
