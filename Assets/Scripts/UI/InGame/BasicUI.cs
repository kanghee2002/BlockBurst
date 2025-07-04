using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    private enum Axis
    {
        X, Y
    }

    private RectTransform rectTransform;

    [SerializeField] private float insidePosition;
    [SerializeField] private float outsidePositionOffset;
    [SerializeField] private Axis moveAxis;
    [SerializeField] private float duration = 0.2f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenUI()
    {
        gameObject.SetActive(true);

        if (moveAxis == Axis.X)
        {
            UIUtils.OpenUI(rectTransform, "X", insidePosition, duration);
        }
        else
        {
            UIUtils.OpenUI(rectTransform, "Y", insidePosition, duration);
        }
    }

    public void CloseUI()
    {
        if (moveAxis == Axis.X)
        {
            UIUtils.CloseUI(rectTransform, "X", insidePosition, outsidePositionOffset, duration);
        }
        else
        {
            UIUtils.CloseUI(rectTransform, "Y", insidePosition, outsidePositionOffset, duration);
        }
    }
}
