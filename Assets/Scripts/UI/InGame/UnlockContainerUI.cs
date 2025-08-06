using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockInfoContainer;
    [SerializeField] private Transform containers;
    [SerializeField] private TextMeshProUGUI pageText;

    private const int itemsPerPage = 4;
    private const int itemsPerRow = 2;
    private const float xOffset = 110f;
    private const float yOffset = 150f;
    private const float itemSpacingX = 200f;
    private const float itemSpacingY = -230f;

    private List<GameObject> unlockInfoList = null;
    private int currentPage;
    private int maxPage;

    public void Initialize(UnlockInfo[] unlockInfoTemplates, List<string> unlockedItems)
    {
        // 이미 한 번 생성되었다면 정보만 초기화
        if (containers.childCount > 0)
        {
            for (int i = 0; i < unlockInfoTemplates.Length; i++)
            {
                GameObject container = containers.GetChild(i).gameObject;
                UnlockInfo unlockInfo = unlockInfoTemplates[i];
                SetContainer(unlockedItems.Contains(unlockInfo.targetName), container, unlockInfo);
            }
            return;
        }

        unlockInfoList = new List<GameObject>();

        currentPage = 1;
        maxPage = unlockInfoTemplates.Length / itemsPerPage;
        if (unlockInfoTemplates.Length % itemsPerPage != 0)
        {
            maxPage++;
        }

        SetPageText();

        for (int i = 0; i < unlockInfoTemplates.Length; i++)
        {
            GameObject container = Instantiate(unlockInfoContainer, containers);

            UnlockInfo unlockInfo = unlockInfoTemplates[i];

            SetContainer(unlockedItems.Contains(unlockInfo.targetName), container, unlockInfo);

            float posX = xOffset + itemSpacingX * (i % itemsPerPage % itemsPerRow);
            float posY = yOffset + itemSpacingY * (i % itemsPerPage / itemsPerRow);
            container.transform.localPosition = new Vector2(posX, posY);

            // 첫 페이지가 아니면 비활성화
            if (i >= itemsPerPage)
            {
                container.SetActive(false);
            }

            unlockInfoList.Add(container);
        }
    }

    private void SetContainer(bool isUnlocked, GameObject container, UnlockInfo unlockInfo)
    {
        container.transform.GetChild(1).GetComponent<Image>().sprite = GetImage(isUnlocked, unlockInfo.targetName);
        container.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = unlockInfo.GetDescription();
    }

    private Sprite GetImage(bool isUnlocked, string itemID)
    {
        string path;
        Sprite sprite;
        if (isUnlocked)
        {
            path = "Sprites/Item/Item/" + itemID;
        }
        else
        {
            path = "Sprites/UI/LockItem";
        }
        sprite = Resources.Load<Sprite>(path);
        return sprite;
    }

    public void OnNextButtonUIPressed()
    {
        if (currentPage >= maxPage)
        {
            return;
        }

        int index = (currentPage - 1) * itemsPerPage;

        for (int i = 0; i < itemsPerPage; i++)
        {
            unlockInfoList[index + i].SetActive(false);
        }

        index += itemsPerPage;

        for (int i = 0; i < itemsPerPage; i++)
        {
            if (index + i >= unlockInfoList.Count) break;
            unlockInfoList[index + i].SetActive(true);
        }

        currentPage++;

        SetPageText();
    }

    public void OnPreviousButtonUIPressed()
    {
        if (currentPage <= 1)
        {
            return;
        }

        int index = (currentPage - 1) * itemsPerPage;

        for (int i = 0; i < itemsPerPage; i++)
        {
            if (index + i >= unlockInfoList.Count) break;
            unlockInfoList[index + i].SetActive(false);
        }

        index -= itemsPerPage;

        for (int i = 0; i < itemsPerPage; i++)
        {
            unlockInfoList[index + i].SetActive(true);
        }

        currentPage--;

        SetPageText();
    }

    private void SetPageText()
    {
        pageText.text = currentPage + " / " + maxPage;
    }
}
