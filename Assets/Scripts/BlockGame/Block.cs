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
    public List<EffectData> Effects { get; private set; }
    public int ReuseCount { get; private set; }
    public GameObject Prefab { get; private set; }

    public void Initialize(BlockData blockData)
    {
        Id = blockData.id;
        Type = blockData.type;
        Shape = blockData.shape.Clone() as Vector2Int[];
        Effects = new List<EffectData>(blockData.effects);
        ReuseCount = blockData.reuseCount;
        Prefab = blockData.prefab;
    }

    /// <summary>
    /// 블록을 시계방향으로 90도 회전시킴
    /// </summary>
    public void RotateShape()
    {
        Vector2Int[] rotatedShape = new Vector2Int[Shape.Length];
        for (int i = 0; i < Shape.Length; i++)
        {
            Vector2Int pos = Shape[i];
            rotatedShape[i] = new Vector2Int(-pos.y, pos.x);
        }
        Shape = rotatedShape;
        
        //transform.Rotate(0f, 0f, -90f);
    }

    private void MakeShapeArray(Vector2Int[] shape)
    {
        // shape 양식에 맞게
        Shape = new Vector2Int[shape.Length];
        for (int i = 0; i < shape.Length; i++)
        {
            Shape[i] = shape[i];
        }
    }

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
}
