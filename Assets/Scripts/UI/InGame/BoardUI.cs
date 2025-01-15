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
    public BoardCellUI[,] boardCellsUI;

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
        boardCellsUI = new BoardCellUI[height, width];

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                GameObject newObject = Instantiate(prefabBoardCellUI);
                newObject.transform.SetParent(boardUI.transform, false);
                newObject.GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(col - (width - 1f) / 2, -(row - (height - 1f) / 2)) * block_size;
                
                var boardCellUI = newObject.GetComponent<BoardCellUI>();
                boardCellUI.SetCellIndex(new Vector2Int(row, col));
                boardCellsUI[row, col] = boardCellUI;
            }
        }
    }

    public void OnBlockPlaced(GameObject blockObj, Block block, Vector2Int pos)
    {
        DecomposeBlockToBoard(blockObj, block, pos);
        Destroy(blockObj);
    }

    private void DecomposeBlockToBoard(GameObject blockObj, Block block, Vector2Int pos)
    {
        Transform visualObj = blockObj.transform.GetChild(1);
        foreach (Vector2Int shapePos in block.Shape)
        {
            Vector2Int cellPos = pos + shapePos;
            boardCellsUI[cellPos.x, cellPos.y].SetBlockInfo(block.Id);
            boardCellsUI[cellPos.x, cellPos.y].CopyVisualFrom(visualObj);
        }
    }

    public void ProcessMatchAnimation(List<Match> matches)
    {
        StartCoroutine(MatchAnimationCoroutine(matches));
    }

    private IEnumerator MatchAnimationCoroutine(List<Match> matches)
    {
        // 하이라이트 효과
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < width; x++)
                {
                    boardCellsUI[match.index, x].PlayHighlightAnimation();
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < height; y++)
                {
                    boardCellsUI[y, match.index].PlayHighlightAnimation();
                }
            }
        }
        yield return new WaitForSeconds(0.3f);

        // 제거 애니메이션
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < width; x++)
                {
                    boardCellsUI[match.index, x].PlayClearAnimation();
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < height; y++)
                {
                    boardCellsUI[y, match.index].PlayClearAnimation();
                }
            }
        }
        yield return new WaitForSeconds(0.5f);

        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < width; x++)
                {
                    boardCellsUI[match.index, x].SetBlockInfo("");
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < height; y++)
                {
                    boardCellsUI[y, match.index].SetBlockInfo("");
                }
            }
        }
    }
}