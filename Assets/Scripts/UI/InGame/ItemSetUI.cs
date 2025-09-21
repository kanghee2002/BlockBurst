using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Linq;

public class ItemSetUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;

    [SerializeField] private GameObject itemIconPrefab;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float hoverDuration = 0.2f;

    [Header("Windows")]
    [SerializeField] private float windowsIconOverlap = 30f;
    [SerializeField] private float windowsStartPos = -300f;
    [SerializeField] private float windowsItemScale = 1f;

    [Header("Mobile")]
    [SerializeField] private float mobileStartPos = -100f;
    [SerializeField] private float mobileEndPos = 100f;
    [SerializeField] private float mobileItemScale = 0.5f;
    private List<GameObject> itemIcons = new List<GameObject>();

    [Header("Auto Sizing")]
    [SerializeField] private RectTransform deskMatRect; // Left
    [SerializeField] private RectTransform deckButtonUIRect; // Right

    [Header("Item Full Animation")]
    [SerializeField] private Image showItemFullImage;

    private const float insidePositionY = -369;
    private const float outsidePositionOffsetY = 1000;
    private const float duration = 0.2f;

    private float discardAnimationDelay;
    private List<Tween> blockRelatedShakeTweens;
    private List<Tween> shakeTweens;

    private Vector2 effectTextPos;

    private Sequence slowShakeSequence;

    private void Start()
    {
        originalAnchoredPosition = rectTransform.anchoredPosition;
        blockRelatedShakeTweens = new List<Tween>();
        shakeTweens = new List<Tween>();
        effectTextPos = itemIconPrefab.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition;
    }

    private void Update()
    {
        //AutoSizing();
    }

    public void Initialize(List<ItemData> items, int maxItemCount, int discardIndex = -1)
    {
        discardAnimationDelay = 0.3f;
        
        //AutoSizing();
        
        Clear(discardIndex);

        float iconWidth = itemIconPrefab.GetComponent<RectTransform>().rect.width;
        float spacing, startPos, scale;

        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            spacing = iconWidth - windowsIconOverlap;
            startPos = windowsStartPos;
            scale = windowsItemScale;
        }
        else
        {
            spacing = (mobileEndPos - mobileStartPos) / (maxItemCount - 1);
            startPos = mobileStartPos;
            scale = mobileItemScale;
        }

        int addedIndex = 0;

        foreach (ItemData item in items)
        {
            if (itemIcons.Count == discardIndex)
            {
                addedIndex++;
            }

            GameObject itemIcon = Instantiate(itemIconPrefab, transform);
            
            RectTransform rectTransform = itemIcon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startPos + spacing * (itemIcons.Count + addedIndex), 0);

            rectTransform.localScale = Vector3.one * scale;

            rectTransform.DOAnchorPos(new Vector2(startPos + spacing * itemIcons.Count, 0), 0.2f)
                .SetEase(Ease.OutBack)
                .SetDelay(discardAnimationDelay);

            itemIcon.transform.GetChild(0).GetComponent<Image>().sprite = GetImage(item);
            SetUpgradeIcon(itemIcon, item);

            GameObject description = itemIcon.transform.GetChild(2).gameObject;

            description.GetComponent<TextMeshProUGUI>().text = GetDescription(item);

            Button discardButton = itemIcon.transform.GetChild(4).GetComponent<Button>();
            int index = itemIcons.Count;
            discardButton.onClick.RemoveAllListeners();
            discardButton.onClick.AddListener(() => DiscardItem(index));

            // Event
            EventTrigger trigger = itemIcon.AddComponent<EventTrigger>();
            
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => {
                OnItemEnter(description);
                itemIcon.transform.DOScale(scale * hoverScale, hoverDuration).SetEase(Ease.OutQuad);
                itemIcon.transform.SetAsLastSibling();
            });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => {
                OnItemExit(description);
                itemIcon.transform.DOScale(scale, hoverDuration).SetEase(Ease.OutQuad);
                itemIcon.transform.SetSiblingIndex(itemIcons.IndexOf(itemIcon));
            });
            trigger.triggers.Add(exitEntry);

            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((data) => {
                OnItemClick(index);
            });
            trigger.triggers.Add(clickEntry);

            itemIcon.transform.SetSiblingIndex(itemIcons.Count);
            itemIcons.Add(itemIcon);

            slowShakeSequence = UIUtils.PlaySlowShakeAnimation(itemIcon.transform, rotateAmount: 5f, duration: 2f, delay: Random.Range(0f, 2f));
        }

        itemCountText.text = items.Count + "/" + maxItemCount;
    }

    void AutoSizing()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas.worldCamera ?? Camera.main;

        // UI ��ҵ��� ȭ��� ��ġ�� ũ�⸦ ��Ȯ�ϰ� ���
        Vector2 deskMatScreen = cam.WorldToScreenPoint(deskMatRect.position);
        Vector2 deckButtonUIScreen = cam.WorldToScreenPoint(deckButtonUIRect.position);

        // �� UI�� ���� ��� ��ġ ��� (ȭ�� ��ǥ��)
        float deskMatRight = deskMatScreen.x + ((deskMatRect.rect.width / 2) * canvas.scaleFactor);
        float deckButtonUILeft = deckButtonUIScreen.x - ((deckButtonUIRect.rect.width / 2) * canvas.scaleFactor);

        // UI ������ ��ȯ
        float availableWidth = (deckButtonUILeft - deskMatRight) / canvas.scaleFactor;
        float availableHeight = 160f / canvas.scaleFactor;

        // ���� ItemSetUI�� �ʿ� ũ��
        float itemSetUIWidth = 800f;
        float itemSetUIHeight = 160f;

        // ���� ������ �ణ �ּ� ��迡 �� ���� �ʵ��� ��
        float scale = Mathf.Min(availableWidth / itemSetUIWidth, availableHeight / itemSetUIHeight) * 0.99f;

        // �ּ� ũ�� ����
        scale = Mathf.Max(scale, 0.1f);

        // ũ�� ����
        rectTransform.localScale = Vector3.one * scale;

        // �� �𸣰ڴ�
        rectTransform.anchoredPosition = new Vector2(originalAnchoredPosition.x, (float)Screen.height / 1080 * originalAnchoredPosition.y);
    }
    
    public void SetUIColor(Color uiColor, Color textColor)
    {
        UIUtils.SetTextColorByScalar(itemCountText, textColor, 1f);
        UIUtils.SetImageColorByScalar(gameObject.GetComponent<Image>(), uiColor, 1f);
    }

    private Sprite GetImage(ItemData item)
    {
        string blockPresetPath = "Sprites/Block/Preset/";
        string itemPath = "Sprites/Item/Item/";

        if (item.type == ItemType.ADD_BLOCK)
        {
            return Resources.Load<Sprite>(blockPresetPath + item.block.type.ToString());
        }
        else if (item.type == ItemType.ITEM)
        {
            return Resources.Load<Sprite>(itemPath + item.id);
        }
        else if (item.type == ItemType.CONVERT_BLOCK || item.type == ItemType.UPGRADE)
        {
            return Resources.Load<Sprite>(blockPresetPath + item.block.type.ToString());
        }
        return null;
    }

    private void SetUpgradeIcon(GameObject itemUI, ItemData item)
    {
        string upgradePath = "Sprites/Item/Upgrade/";
        itemUI.transform.GetChild(1).gameObject.SetActive(false);

        if (item.type != ItemType.CONVERT_BLOCK && item.type != ItemType.UPGRADE)
            return;

        string IconPath = upgradePath;
        if (item.type == ItemType.CONVERT_BLOCK)
        {
            IconPath += "ConvertUpgrade";
        }
        else if (item.effects[0].type == EffectType.GOLD_MODIFIER)
        {
            IconPath += "GoldUpgrade";
        }
        else if (item.effects[0].type == EffectType.MULTIPLIER_MODIFIER)
        {
            IconPath += "MultiplierUpgrade";
        }
        else if (item.effects[0].type == EffectType.REROLL_MODIFIER)
        {
            IconPath += "RerollUpgrade";
        }
        else if (item.effects[0].type == EffectType.BLOCK_REUSE_MODIFIER)
        {
            IconPath += "ReuseUpgrade";
        }
        else if (item.effects[0].type == EffectType.SCORE_MODIFIER)
        {
            IconPath += "ScoreUpgrade";
        }

        Sprite IconSprite = Resources.Load<Sprite>(IconPath);
        itemUI.transform.GetChild(1).gameObject.SetActive(true);
        itemUI.transform.GetChild(1).GetComponent<Image>().sprite = IconSprite;
    }

    public void Clear(int discardIndex)
    {
        for (int i = 0; i < itemIcons.Count; i++)
        {
            if (itemIcons[i] != null)
            {
                if (i == discardIndex)
                {
                    PlayDiscardItemAnimation(itemIcons[i]);
                }
                else
                {
                    Destroy(itemIcons[i]);
                }
            }
        }
        itemIcons.Clear();
    }

    private void OnItemEnter(GameObject description)
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            description.SetActive(true);
        }
    }

    private void OnItemExit(GameObject description)
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            description.SetActive(false);
        }
    }

    private void OnItemClick(int index)
    {
        /*DisableAllDiscardButton(discardButton);
        if (discardButton.activeSelf)
        {
            discardButton.SetActive(false);
        }
        else
        {
            discardButton.SetActive(true);
            discardButton.transform.DOPunchScale(Vector3.one * 0.3f, duration: 0.1f, vibrato: 5)
                .SetUpdate(true);
        }*/
        AudioManager.instance.SFXSelectMenu();
        GameUIManager.instance.OnItemSetUIPressed(index);
    }

    public void DiscardItem(int index)
    {
        AudioManager.instance.SFXThrowItem();
        GameManager.instance.OnItemDiscard(index);
    }

    private void DisableAllDiscardButton(GameObject exception)
    {
        foreach (GameObject itemIcon in itemIcons)
        {
            GameObject discardButton = itemIcon.transform.GetChild(4).gameObject;

            if (discardButton == exception) continue;

            discardButton.SetActive(false);
        }
    }

    private void PlayDiscardItemAnimation(GameObject discardedObject)
    {
        discardedObject.transform.GetChild(2).gameObject.SetActive(false);

        discardedObject.transform.DOKill();

        discardedObject.GetComponent<EventTrigger>().triggers.Clear();
        
        Sequence sequence = DOTween.Sequence();

        float rotateAmount = 20f;
        // 좌우 흔들기
        sequence.Append(discardedObject.transform.DOScale(0f, discardAnimationDelay)
            .SetEase(Ease.InQuad));
        sequence.Join(discardedObject.transform.DORotate(new Vector3(0, 0, rotateAmount), discardAnimationDelay / 10)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.Linear));

        sequence.OnComplete(() => Destroy(discardedObject));
    }

    private string GetDescription(ItemData item)
    {
        string description = "";

        for (int i = 0; i < item.effects.Count; i++)
        {
            description += item.effects[i].effectName;
            if (i != item.effects.Count - 1)
            {
                description += "\n";
            }
        }

        EffectData triggerEffect = item.effects.FirstOrDefault(effect => effect.triggerMode == TriggerMode.Interval);

        if (triggerEffect != null)
        {
            description += "\n현재 횟수: " + triggerEffect.triggerCount;
        }

        return description;
    }

    public void UpdateTriggerCountDescription(int index, int count)
    {
        TextMeshProUGUI descriptionText = itemIcons[index].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        
        string[] words = descriptionText.text.Split(' ');
        if (words.Length == 0) return;

        words[words.Length - 1] = count.ToString();

        descriptionText.text = string.Join(" ", words);
    }

    public void StartShakeAnimation(int index, bool isBlockRelated)
    {
        GameObject currentItem = itemIcons[index];

        RectTransform itemRect = currentItem.GetComponent<RectTransform>();

        float originalPosX = itemRect.anchoredPosition.x;

        Tween currentTween = itemRect.DOPunchAnchorPos(Vector3.up * 8f, 0.5f,
            vibrato: 5, elasticity: 0.3f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutQuad)
            .OnKill(() => itemRect.anchoredPosition = new Vector2(originalPosX, 0f));

        if (isBlockRelated)
        {
            blockRelatedShakeTweens.Add(currentTween);
        }
        else
        {
            shakeTweens.Add(currentTween);
        }
    }

    public void StopShakeAnimation(bool isBlockRelated)
    {
        List<Tween> targetTwinList = new List<Tween>();
        if (isBlockRelated)
        {
            targetTwinList = blockRelatedShakeTweens;
        }
        else
        {
            targetTwinList = shakeTweens;
        }

        foreach (Tween tween in targetTwinList)
        {
            tween.Kill();
        }
    }

    public void PlayEffectAnimation(string effectDescription, int index, float delay)
    {
        float fadeDelay = 1f, scale;

        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            scale = windowsItemScale;
        }
        else
        {
            scale = mobileItemScale;
        }

        GameObject currentItem = itemIcons[index];

        TextMeshProUGUI currentText = currentItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        currentText.gameObject.SetActive(true);
        currentText.text = effectDescription;

        Vector2 originalPosition = currentText.rectTransform.anchoredPosition;
        Color originalColor = new Color(1f, 1f, 1f, 1f);
        float yOffset = 10f;

        currentItem.transform.DOKill();
        currentItem.transform.localScale = Vector3.one * scale;

        currentItem.transform.DOPunchScale(Vector2.one * scale * 1.2f, 0.3f, 10, 0.5f);
        
        Sequence sequence = DOTween.Sequence();

        sequence.Append(currentText.rectTransform.DOAnchorPosY(originalPosition.y + yOffset, 0.3f)
            .SetEase(Ease.InOutQuad));
        sequence.Join(currentText.DOFade(0f, delay + fadeDelay).SetEase(Ease.InOutQuad));

        sequence.OnComplete(() =>
        {
            currentItem.transform.DOScale(Vector2.one * scale, 0.2f);
            currentText.rectTransform.anchoredPosition = effectTextPos;
            currentText.color = originalColor;
            currentText.gameObject.SetActive(false);
        });
    }

    public void PlayItemFullAnimation()
    {
        if (showItemFullImage.gameObject.activeSelf)
        {
            return;
        }
        showItemFullImage.gameObject.SetActive(true);
        showItemFullImage.color = new Color(0f, 0f, 0f, 0.8f);

        float duration = 0.5f, endDelay = 0.2f;

        RectTransform textRect = showItemFullImage.transform.GetChild(0).GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(textRect.DOPunchScale(Vector3.one * 1.2f, duration,
            vibrato: 7, elasticity: 0.3f)
            .SetEase(Ease.OutQuad));

        sequence.AppendInterval(endDelay);

        sequence.OnComplete(() =>
        {
            textRect.DOScale(Vector3.one, duration);
            showItemFullImage.gameObject.SetActive(false);
        });
    }

    public void OpenItemSetUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseItemSetUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }
}