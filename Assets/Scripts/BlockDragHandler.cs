using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BlockDragHandler : MonoBehaviour
{
    [SerializeField] private Board board;

    private Transform clickedBlock;
    private Vector2 offset;
    private Vector2Int currentOriginCellPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FindBlockOnMouse();
        }

        if (Input.GetMouseButton(0) && clickedBlock != null)
        {
            DragBlock();

            ShowPlacementPreview();
        }

    }

    private void FindBlockOnMouse()
    {
        bool isBlockClicked = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Block"))
            {
                isBlockClicked = true;
                clickedBlock = hit.collider.transform;
                offset = ray.origin - clickedBlock.transform.position;
            }
        }

        if (!isBlockClicked)
        {
            clickedBlock = null;
        }
    }

    private void DragBlock()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        Vector2 objPos = Camera.main.ScreenToWorldPoint(mousePos);

        clickedBlock.transform.position = objPos - offset;
    }

    private void ChangeCellsColor(Vector2Int originCellPosition, Vector2Int[] shapes, Color color)
    {
        foreach (Vector2Int shape in shapes)
        {
            Vector2Int cellPosition = originCellPosition + shape;

            // Need to change 8 to board size
            if (cellPosition.x < 0 || cellPosition.x >= 8 || cellPosition.y < 0 || cellPosition.y >= 8)
            {
                continue;
            }

            Cell cell = board.cells[cellPosition.x, cellPosition.y];
            cell.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void ShowPlacementPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

        bool isRaycastingCell = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Cell"))
            {
                isRaycastingCell = true;
                if (clickedBlock.TryGetComponent(out Block block))
                {
                    // Get Distance from OriginCell
                    Vector2 distance = ray.origin - block.originCell.transform.position;
                    Vector2Int cellDistance = new Vector2Int(Mathf.RoundToInt(distance.x), -Mathf.RoundToInt(distance.y));

                    // Get Cell Position of OriginCell
                    Vector2Int currentCellPosition = hit.transform.GetComponent<Cell>().cellPosition;

                    Vector2Int newOriginCellPosition = currentCellPosition - cellDistance;

                    // If Cell is Changed, Remove Old Preview
                    if (currentOriginCellPosition != null)
                    {
                        if (currentOriginCellPosition != newOriginCellPosition)
                        {
                            ChangeCellsColor(currentOriginCellPosition, block.Shape, new Color(1f, 1f, 1f));
                        }
                    }

                    // Set Preview
                    currentOriginCellPosition = newOriginCellPosition;

                    ChangeCellsColor(currentOriginCellPosition, block.Shape, new Color(0.4f, 0.4f, 0.4f));
                }
                else
                {
                    Debug.Log("Clicked Block has no Block Script");
                }
            }
        }

        // If Mouse is out of cell, Remove Preview
        if (!isRaycastingCell)
        {
            ChangeCellsColor(currentOriginCellPosition, clickedBlock.GetComponent<Block>().Shape, new Color(1f, 1f, 1f));
        }
    }

}
