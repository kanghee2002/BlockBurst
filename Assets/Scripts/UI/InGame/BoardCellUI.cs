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

    public void SetShadowed(bool isShadowed)
    {
        if (isShadowed)
        {
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            image.color = Color.white;
        }
    }
}
