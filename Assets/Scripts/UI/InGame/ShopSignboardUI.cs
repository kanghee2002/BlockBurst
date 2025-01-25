using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSignboardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (300,256)
    private const float insidePositionY = 256;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    [SerializeField] private GameObject prefabLight;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        CreateLights();
    }

    public void OpenShopSignboardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseShopSignboardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
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
        instance.transform.localScale = new Vector2(1, 1); // 스케일 초기화
        instance.transform.localPosition = localPosition; // 로컬 좌표에 위치 설정
        instance.GetComponent<ShopSignboardNeonSign>().on = on;
    }
}

