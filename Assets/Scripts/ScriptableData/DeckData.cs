using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Deck", menuName = "BlockBurst/Deck")]
public class DeckData : ScriptableObject
{
    public string id;
    public DeckType type;                                       // 덱 타입
    public int defaultRerlollCount;                             // 기본 리롤 횟수
    public int maxItemCount;                                    // 소지 가능 아이템 수
    public int baseBoardRows;                                   // 보드 크기
    public int baseBoardColumns;                                // 보드 크기
    public List<EffectData> effects;                            // 기본 효과
    [HideInInspector] public List<BlockType> defaultBlocks;     // 블록 덱

    public void Initialize()
    {
        defaultBlocks = GetDefaultBlocks();
    }

    private List<BlockType> GetDefaultBlocks()
    {
        List<BlockType> defaultBlocks = new List<BlockType>();
        int defaultBlockCount = 21;

        switch (type)
        {
            case DeckType.Default:
            case DeckType.YoYo:
            case DeckType.Telescope:
            case DeckType.Bomb:
                foreach (BlockType blockType in Enums.GetEnumArray<BlockType>())
                {
                    if (Enums.IsDefaultBlockType(blockType))
                    {
                        for (int i = 0; i < defaultBlockCount / 7; i++)
                        {
                            defaultBlocks.Add(blockType);
                        }
                    }
                }
                break;
            case DeckType.Dice:
                for (int i = 0; i < defaultBlockCount; i++)
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        BlockType blockType = Enums.GetRandomEnum<BlockType>();

                        if (Enums.IsDefaultBlockType(blockType))
                        {
                            defaultBlocks.Add(blockType);
                            break;
                        }
                    }
                }
                break;
            case DeckType.Mirror:
                for (int i = 0; i < defaultBlockCount; i++)
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        BlockType blockType = Enums.GetRandomEnum<BlockType>();

                        if (Enums.IsSpecialBlockType(blockType))
                        {
                            defaultBlocks.Add(blockType);
                            break;
                        }
                    }
                }
                break;
        }

        return defaultBlocks;
    }
}
