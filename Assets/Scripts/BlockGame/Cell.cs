using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public BlockType? Type { get; private set; }
    public string BlockID { get; private set; }
    public bool IsBlocked { get; private set; }

    public void Initialize()
    {
        Type = null;
        BlockID = "";
        IsBlocked = false;
    }

    public void SetBlock(BlockType type, string blockId)
    {
        Type = type;
        BlockID = blockId;
        IsBlocked = true;
    }

    public void ClearBlock()
    {
        if (IsBlocked && BlockID == "")
        {
            return;
        }

        Type = null;
        BlockID = "";
        IsBlocked = false;
    }

    public void BlockCell()
    {
        Type = null;
        IsBlocked = true;
        BlockID = "";
    }

    /*
    // Test Code ////////////////////////////////////////////////////////////
    public Vector2Int cellPosition;

    private void Start()
    {
        IsBlocked = false;
    }

    /////////////////////////////////////////////////////////////////////////
    */
}
