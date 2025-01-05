using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool IsBlocked { get; private set; }

    public void SetBlock()
    {
        IsBlocked = true;
    }

    // Test Code ////////////////////////////////////////////////////////////
    public Vector2Int cellPosition;

    private void Start()
    {
        IsBlocked = false;
    }

    /////////////////////////////////////////////////////////////////////////
}
