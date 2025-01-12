using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public BlockType? Type { get; private set; }
    public int BlockID { get; private set; }
    public bool IsBlocked { get; private set; }

    public void Initialize()
    {
        Type = null;
        BlockID = -1;
        IsBlocked = false;
    }

    public void SetBlock(BlockType type, int blockId)
    {
        Type = type;
        BlockID = blockId;
        IsBlocked = true;
        

        //TEST
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
    }

    public void ClearBlock()
    {
        Type = null;
        BlockID = -1;
        IsBlocked = false;

        //TEST
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    }

    public void BlockCell()
    {
        Type = null;
        IsBlocked = true;
        BlockID = -1;
    }

    // Test Code ////////////////////////////////////////////////////////////
    public Vector2Int cellPosition;

    private void Start()
    {
        IsBlocked = false;
    }

    /////////////////////////////////////////////////////////////////////////
}
