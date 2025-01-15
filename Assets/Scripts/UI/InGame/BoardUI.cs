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
    public Dictionary<string, GameObject> activeBlocks = new Dictionary<string, GameObject>();

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
                newObject.GetComponent<BoardCellUI>().SetCellIndex(new Vector2Int(column, height - row - 1)); // ���� ������ ��¥

                listRow.Add(newObject.GetComponent<BoardCellUI>());
            }
            boardCellsUI.Add(listRow);
        }
    }

    public void OnBlockPlaced(BlockData block, Vector2Int pos) {
        GameObject blockObj = activeBlocks[block.id];
        DecomposeBlockToBoard(blockObj, block, pos);
        activeBlocks.Remove(block.id);
        Destroy(blockObj);
    }

    private void DecomposeBlockToBoard(GameObject blockObj, BlockData block, Vector2Int pos) {
        BlockUI blockUI = blockObj.GetComponent<BlockUI>();
        foreach (Vector2Int shapePos in block.shape) {
            Vector2Int cellPos = pos + shapePos;
            BoardCellUI cellUI = boardCellsUI[cellPos.y][cellPos.x];
            cellUI.SetBlockInfo(block.id);
            cellUI.CopyVisualFrom(blockUI);
        }
    }

    public void ProcessMatchAnimation(List<Match> matches) {
        StartCoroutine(MatchAnimationCoroutine(matches));
    }

    private IEnumerator MatchAnimationCoroutine(List<Match> matches) {
        // 하이라이트 효과
        foreach (Match match in matches) {
            if (match.matchType == MatchType.ROW) {
                for (int x = 0; x < width; x++) {
                    boardCellsUI[match.index][x].PlayHighlightAnimation();
                }
            } else if (match.matchType == MatchType.COLUMN) {
                for (int y = 0; y < height; y++) {
                    boardCellsUI[y][match.index].PlayHighlightAnimation();
                }
            }
        }
        yield return new WaitForSeconds(0.3f);

        // 제거 애니메이션
        foreach (Match match in matches) {
            if (match.matchType == MatchType.ROW) {
                for (int x = 0; x < width; x++) {
                    boardCellsUI[match.index][x].PlayClearAnimation();
                }
            } else if (match.matchType == MatchType.COLUMN) {
                for (int y = 0; y < height; y++) {
                    boardCellsUI[y][match.index].PlayClearAnimation();
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
            foreach (Match match in matches) {
            if (match.matchType == MatchType.ROW) {
                for (int x = 0; x < width; x++) {
                    boardCellsUI[match.index][x].SetBlockInfo("");
                }
            } else if (match.matchType == MatchType.COLUMN) {
                for (int y = 0; y < height; y++) {
                    boardCellsUI[y][match.index].SetBlockInfo("");
                }
            }
        }
    }
}
