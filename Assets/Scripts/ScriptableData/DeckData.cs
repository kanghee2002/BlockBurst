using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Deck", menuName = "BlockBurst/Deck")]
public class DeckData : ScriptableObject
{
    public string id;
    public DeckType type;
    public int defaultRerlollCount;
    public List<EffectData> effects;
    [HideInInspector] public List<BlockType> defaultBlocks;


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
