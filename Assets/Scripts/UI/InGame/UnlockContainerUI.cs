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
    private const float offset = 150f;
    private const float itemSpacing = -90f;

    private List<GameObject> unlockInfoList = null;
    private int currentPage;
    private int maxPage;

    public void Initialize(UnlockInfo[] unlockInfoTemplates, List<string> unlockedItems)
    {
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

            float posY = offset + itemSpacing * (i % itemsPerPage);
            container.transform.localPosition = new Vector2(0, posY);

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
        if (isUnlocked)
        {
            container.transform.GetChild(0).GetComponent<Image>().sprite = GetImage(isUnlocked, unlockInfo.targetName);
            container.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDescription(isUnlocked, unlockInfo);
        }
        else
        {
            container.transform.GetChild(0).GetComponent<Image>().sprite = GetImage(isUnlocked, unlockInfo.targetName);
            container.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDescription(isUnlocked, unlockInfo);
        }
    }

    private Sprite GetImage(bool isUnlocked, string itemID)
    {
        string path = "Sprites/Item/Item/" + itemID;
        Sprite sprite = Resources.Load<Sprite>(path);

        return sprite;
    }

    private string GetDescription(bool isUnlocked, UnlockInfo unlockInfo)
    {
        string result = unlockInfo.description;

        result = result.Replace("Requirement", unlockInfo.requirement.ToString());

        result = UIUtils.SetBlockNameToIcon(result);

        return result;
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
