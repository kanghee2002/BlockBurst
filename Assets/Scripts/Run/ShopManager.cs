using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("DEBUG")]
    // 첫 상점에서 나올 아이템 지정
    [SerializeField] private List<string> firstShopItemList;

    private RunData runData;
    [SerializeField] private List<ItemData> currentItems;

    private DeckManager deckManager;

    [HideInInspector] public int currentRerollCost;
    [HideInInspector] public int baseRerollCost;

    private Dictionary<ItemType, List<ItemType>> shopItemDictionary = new Dictionary<ItemType, List<ItemType>>()
        {
            { ItemType.ITEM, new List<ItemType>() { ItemType.ITEM } },
            { ItemType.BOOST, new List<ItemType>() { ItemType.BOOST } },
            { ItemType.ADD_BLOCK, new List<ItemType>() { ItemType.ADD_BLOCK, ItemType.CONVERT_BLOCK, ItemType.UPGRADE } },
        };

    public void Initialize(ref RunData data, ItemData[] items)
    {
        baseRerollCost = data.shopBaseRerollCost;
        currentRerollCost = baseRerollCost;

        runData = data;
        currentItems = new List<ItemData>();
        foreach (ItemData item in items)
        {
            AddItem(item);

            // 아이템 초기 값 초기화
            item.effects.ForEach(effect =>
            {
                effect.effectValue = effect.baseEffectValue;
                effect.triggerCount = 0;
            });
        }
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
    }

    // 튜토리얼 용 함수
    public void AddFirstItem(List<string> items)
    {
        foreach (string item in items)
        {
            firstShopItemList.Add(item);
        }
    }

    public int PurchaseItem(ItemData item)
    {
        if (item == null) return -1;

        if (item.type == ItemType.ITEM && runData.activeItems.Count >= runData.maxItemCount)
        {
            GameManager.instance.PlayItemFullAnimation();
            return -1;
        }

        if (runData.gold < item.cost)
        {
            GameManager.instance.PlayNotEnoughGoldAnimation();
            return -1;
        } 
        else
        {
            if (item.isPurchasableAgain)
            {
                AddItem(item);
            }
            GameManager.instance.UpdateGold(-item.cost);
            ApplyItem(item);
            return runData.gold;
        }
    }

    public ItemData PopItem(ItemType itemType)
    {
        if (currentItems.Count == 0)
        {
            return null;
        }

        // DEBUG or TUTORIAL /////////////////////////////////////////
        if (firstShopItemList.Count > 0)
        {
            int index = currentItems.FindIndex(x => x.id == firstShopItemList[0]);
            ItemData itemData = currentItems[index];
            if (shopItemDictionary[itemType].Contains(itemData.type))
            {
                currentItems.RemoveAt(index);
                firstShopItemList.RemoveAt(0);

                return itemData;
            }
        }
        ///////////////////////////////////////////////////////////////

        // 타입에 해당하는 아이템 선별
        List<ItemData> filteredItems = new List<ItemData>();
        List<ItemType> itemTypes = shopItemDictionary[itemType];
        filteredItems = currentItems.Where(item => itemTypes.Contains(item.type)).ToList();

        // 희귀도에 따른 아이템 선별
        List<ItemData> selectedItems = new List<ItemData>();
        for (int i = 0; i < 10000; i++)
        {
            ItemRarity selectedItemRarity = SelectByWeight(runData.itemRarityWeights);
            selectedItems = filteredItems.Where(item => item.rarity == selectedItemRarity).ToList();
            if (selectedItems.Count > 0) break;
        }

        int idx = Random.Range(0, selectedItems.Count);

        ItemData item = selectedItems[idx];
        currentItems.Remove(item);

        return item;
    }

    private void ApplyItem(ItemData item)
    {
        if (item.type == ItemType.ADD_BLOCK)
        {
            int addCount = 1;

            for (int i = 0; i < addCount; i++)
            {
                deckManager.AddBlockToRunDeck(item.block);
            }

            // 특수 블록은 첫 구매에만 효과 등록
            if (Enums.IsSpecialBlockType(item.block.type))
            {
                foreach (EffectData effect in item.block.effects)
                {
                    if (EffectManager.instance.ContainsEffect(effect))
                    {
                        continue;
                    }
                    EffectManager.instance.AddEffect(effect);
                }
            }
        }
        else if (item.type == ItemType.CONVERT_BLOCK)
        {
            int convertCount = 1;

            List<BlockType> highestScoreBlocks = runData.baseBlockScores
                            .Where(kv => kv.Value == runData.baseBlockScores.Values.Max())
                            .Select(kv => kv.Key)
                            .ToList();

            int removeCount = 0;
            for (int i = 0; i < convertCount; i++)
            {
                if (deckManager.RemoveRandomBlockFromRunDeck(highestScoreBlocks))
                {
                    removeCount++;
                }
            
            }

            BlockType convertBlockType = highestScoreBlocks[Random.Range(0, highestScoreBlocks.Count)];
            BlockData convertBlock = Resources.Load<BlockData>("ScriptableObjects/Block/Block" + convertBlockType + "Data");

            for (int i = 0; i < removeCount; i++)
            {
                deckManager.AddBlockToRunDeck(convertBlock);
            } 
        }
        else if (item.type == ItemType.ITEM)
        {
            runData.activeItems.Add(item);
            foreach (EffectData effect in item.effects)
            {
                EffectManager.instance.AddEffect(effect);
            }
        }
        else if (item.type == ItemType.BOOST)
        {
            // 진행정보 창에 추가 필요
            runData.activeBoosts.Add(item);
            foreach (EffectData effect in item.effects)
            {
                EffectManager.instance.AddEffect(effect);
            }
        }
        else if (item.type == ItemType.UPGRADE) 
        {
            foreach (EffectData effect in item.effects)
            {
                // 아이템처럼 취급
                // 구매 시 등록 (BlockData의 effects로 등록 X)
                EffectManager.instance.AddEffect(effect);
                CellEffectManager.instance.AddEffect(effect.blockTypes, effect.type.ToString());
            }
        }

        if (item.type == ItemType.ADD_BLOCK ||
            item.type == ItemType.CONVERT_BLOCK)
        {
            GameManager.instance.UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);
        }
    }

    public void UpgradeBlock(BlockData block, EffectData effect)
    {
        block.effects.Add(effect);
    }

    public void RerollShop(List<ItemData> items)
    {
        foreach (ItemData item in items)
        {
            if (item != null)
            {
                AddItem(item);
            }
        }
        currentRerollCost += runData.shopRerollCostGrowth;
    }

    public void AddItem(ItemData item)
    {
        // append
        currentItems.Add(item);
    }

    public void InitializeRerollCost()
    {
        baseRerollCost = runData.shopBaseRerollCost;
        currentRerollCost = baseRerollCost;
    }

    private int GetTotalWeights<T>(Dictionary<T, int> weights)
    {
        return weights.Sum(x => x.Value);
    }

    private T SelectByWeight<T>(Dictionary<T, int> weights)
    {
        // 누적 가중치 만들기
        List<(T, int)> weightThresholds = new List<(T, int)>();

        int sum = 0;
        foreach (var itemWeight in weights)
        {
            sum += itemWeight.Value;
            weightThresholds.Add((itemWeight.Key, sum));
        }

        // 뽑기
        int randomWeight = Random.Range(0, sum + 1);

        for (int i = 0; i < weightThresholds.Count; i++)
        {
            if (randomWeight <= weightThresholds[i].Item2)
            {
                return weightThresholds[i].Item1;
            }
        }

        Debug.Log("Error: ShopManager Weighted Random");
        return default(T);
    }
}