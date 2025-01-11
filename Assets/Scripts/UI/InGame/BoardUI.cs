using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private GameObject boardUI;
    [SerializeField] private RectTransform rectTransform;

    private const float block_size = 96f;
    private int width = 8;
    private int height = 8;
    [SerializeField] private GameObject prefabBoardCellUI;
    public List<List<BoardCellUI>> boardCellsUI = new List<List<BoardCellUI>>();

    // inside anchored position = (116,-96)
    private const float insidePositionY = -96;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;
    public void OpenBoardUI()
    {
        boardUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseBoardUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY + outsidePositionOffsetY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                boardUI.SetActive(false);
            });
    }

    private void Awake()
    {
        for (int row = 0; row < height; row++)
        {
            List<BoardCellUI> listRow = new List<BoardCellUI>();
            for (int column = 0; column < width; column++)
            {
                GameObject newObject = Instantiate(prefabBoardCellUI);
                newObject.transform.SetParent(boardUI.transform, false);
                newObject.GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(column - (width - 1f) / 2, row - (width - 1f) / 2) * block_size;
                newObject.GetComponent<BoardCellUI>().SetCellIndex(new Vector2Int(column, height - row - 1)); // 방향 좆같네 진짜

                listRow.Add(newObject.GetComponent<BoardCellUI>());
            }
            boardCellsUI.Add(listRow);
        }
    }
}
