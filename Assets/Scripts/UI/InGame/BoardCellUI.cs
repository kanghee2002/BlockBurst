using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardCellUI : MonoBehaviour
{
    private Vector2Int cellIndex = new Vector2Int(0, 0);

    [SerializeField] private Image image;

    public void SetCellIndex(Vector2Int indexToSet)
    {
        cellIndex = indexToSet;
    }
}
