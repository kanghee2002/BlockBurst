using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool IsBlocked { get; private set; }

    public void SetBlock()
    {
        IsBlocked = true;

        //TEST
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
    }

    public void ClearBlock()
    {
        IsBlocked = false;

        //TEST
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    }

    // Test Code ////////////////////////////////////////////////////////////
    public Vector2Int cellPosition;

    private void Start()
    {
        IsBlocked = false;
    }

    /////////////////////////////////////////////////////////////////////////
}
