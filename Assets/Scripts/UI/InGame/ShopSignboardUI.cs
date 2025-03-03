using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopSignboardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (300,256)
    private const float insidePositionY = 280;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    [SerializeField] private GameObject prefabLight;
    [SerializeField] private TextMeshProUGUI stageInfoText;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        //CreateLights();
    }

    public void OpenShopSignboardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Mobile) return;

        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseShopSignboardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Mobile) return;

        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }

    public void Initialize(int currentChapterIndex, int currentStageIndex)
    {
        string text = currentChapterIndex + " - " + currentStageIndex;

        stageInfoText.text = text;
    }

    private void CreateLights()
    {
        bool on = true;
        for (int x = -240 + 48; x <= 240 - 48; x += 48)
        {
            CreateLightAt(new Vector2(x, -224 + 48), on);
            on = !on;
        }
        for (int x = 240 - 48; x >= -240 + 48; x -= 48)
        {
            CreateLightAt(new Vector2(x, 224 - 48), on);
            on = !on;
        }
    }

    private void CreateLightAt(Vector2 localPosition, bool on = true)
    {
        GameObject instance = Instantiate(prefabLight);
        instance.transform.SetParent(transform);
        instance.transform.localScale = new Vector2(1, 1); // ������ �ʱ�ȭ
        instance.transform.localPosition = localPosition; // ���� ��ǥ�� ��ġ ����
        instance.GetComponent<ShopSignboardNeonSign>().on = on;
    }
}

