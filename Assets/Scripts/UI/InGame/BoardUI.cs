using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private GameObject boardUI;
    [SerializeField] private RectTransform rectTransform;

    private int width = 8;
    private int height = 8;
    [SerializeField] private GameObject prefabBoardCellUI;
    public List<BoardCellUI> boardCellsUI = new List<BoardCellUI>();

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;
    public void OpenBoardUI()
    {
        boardUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseBoardUI()
    {
        rectTransform.DOAnchorPosY(outsidePositionY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                boardUI.SetActive(false);
            });
    }

    private void Awake()
    {
        for (int i = 0; i < width * height; i++)
        {
            GameObject newObject = Instantiate(prefabBoardCellUI);
            newObject.transform.SetParent(boardUI.transform, false);

            boardCellsUI.Add(newObject.GetComponent<BoardCellUI>());
        }
    }
}
