using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Serializable]
    private struct DictionaryData<TKey, TValue>
    {
        public List<TKey> keys;
        public List<TValue> values;
    }

    public static DataManager instance = null;

    private string path;
    private int dictionaryCount;

    private PlayerData playerData;

    private void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        path = Application.dataPath + "/Data/";

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath;
        }
    }

    // 해금, 통계
    public void SavePlayerData(PlayerData playerData)
    {
        this.playerData = playerData;

        string savedJson = JsonUtility.ToJson(playerData, true);
        string dataPath = Path.Combine(path, "PlayerData.json");
        File.WriteAllText(dataPath, savedJson);
    }

    public void OnStatisticsPlayerDataRequested()
    {
        GameUIManager.instance.OnStatisticsPlayerDataCallback(playerData);
    }

    public void OnDeckSelectionPlayerDataRequested()
    {
        GameUIManager.instance.OnDeckSelectionPlayerDataCallback(playerData);
    }

    // 이어하기
    public void SaveRunData(RunData runData)
    {
        dictionaryCount = 0;

        FieldInfo[] fields = runData.GetType().GetFields();

        foreach (FieldInfo field in fields)
        {
            if (typeof(IDictionary).IsAssignableFrom(field.FieldType))
            {
                object fieldValue = field.GetValue(runData);
                IDictionary dictionary = fieldValue as IDictionary;

                MatchDictionaryType(dictionary);
            }
        }

        string jsonData = JsonUtility.ToJson(runData, true);
        string dataPath = Path.Combine(path, "RunData/");
        dataPath = Path.Combine(dataPath, "RunData.json");
        File.WriteAllText(dataPath, jsonData);

        Debug.Log("Finish Saving RunData");
    }

    public PlayerData LoadPlayerData()
    {
        string dataPath = Path.Combine(path, "PlayerData.json");
        if (!File.Exists(dataPath))
        {
            Debug.Log("There doesn't exist Player Data");
            return null;
        }

        string loadedJson = File.ReadAllText(dataPath);
        playerData = JsonUtility.FromJson<PlayerData>(loadedJson);

        // TEST ---------------------------------------------------------------
        GameManager.instance.TEST_TEXT("Player Data Exist!\n");
        if (playerData == null) GameManager.instance.TEST_TEXT("But NULL\n");

        GameManager.instance.TEST_TEXT("CONTAINING TUTO VALUE? : " + loadedJson.Contains("tutorialValue") + "\n");
        GameManager.instance.TEST_TEXT("MAX CHAPTER : " + playerData.maxChapter + "\n");
        GameManager.instance.TEST_TEXT("Place I : " + playerData.placeCountI + "\n");
        // --------------------------------------------------------------------

        return playerData;
    }

    public RunData LoadRunData(GameData gameData)
    {
        string dataPath = Path.Combine(path, "RunData/");
        dataPath = Path.Combine(dataPath, "RunData.json");
        if (!File.Exists(dataPath))
        {
            Debug.Log("There doesn't exist Run Data");
            //return null;
        }

        //string loadedJson = File.ReadAllText(dataPath);
        //RunData runData = JsonUtility.FromJson<RunData>(loadedJson);

        // ---------------------------------
        // TEST

        RunData runData = new RunData();
        runData.Initialize(gameData);

        // ---------------------------------

        return runData;
    }
    
    private void MatchDictionaryType(IDictionary dictionary)
    {
        Type keyType = dictionary.GetType().GetGenericArguments()[0];
        Type valueType = dictionary.GetType().GetGenericArguments()[1];

        if (valueType == typeof(int))
        {
            if (keyType == typeof(BlockType))
            {
                SaveDictionaryData(dictionary as Dictionary<BlockType, int>, "RunData/");
            }
            else if (keyType == typeof(MatchType))
            {
                SaveDictionaryData(dictionary as Dictionary<MatchType, int>, "RunData/");
            }
            else if (keyType == typeof(ItemType))
            {
                SaveDictionaryData(dictionary as Dictionary<ItemType, int>, "RunData/");
            }
            else if (keyType == typeof(ItemRarity))
            {
                SaveDictionaryData(dictionary as Dictionary<ItemRarity, int>, "RunData/");
            }
            else
            {
                Debug.LogError("Dictionary 저장 에러: keyType이 정의되지 않음");
            }
        }
        else
        {
            Debug.LogError("Dictionary 저장 에러: keyType이 정의되지 않음");
        }
    }

    private DictionaryData<TKey,TValue> ConvertToDictionaryData<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        DictionaryData<TKey, TValue> dictionaryData = new();
        
        List<TKey> keys = new();
        List<TValue> values = new();

        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }

        dictionaryData.keys = keys;
        dictionaryData.values = values;

        return dictionaryData;
    }

    private void SaveDictionaryData<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string additionalPath = "")
    {
        DictionaryData<TKey, TValue> data = ConvertToDictionaryData(dictionary);
        string jsonData = JsonUtility.ToJson(data, true);
        string dataPath = Path.Combine(path, additionalPath);
        dataPath = Path.Combine(dataPath, "dictionary" + dictionaryCount + ".json");
        File.WriteAllText(dataPath, jsonData);

        dictionaryCount++;
    }

    private Dictionary<TKey, TValue> ConvertToDictionary<TKey, TValue>(DictionaryData<TKey, TValue> dictionaryData)
    {
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        if (dictionaryData.keys.Count != dictionaryData.values.Count)
        {
            Debug.LogError("Dictionary 불러오기 에러: key와 value의 개수가 맞지 않음");
            return null;
        }

        for (int i = 0; i < dictionaryData.keys.Count; i++)
        {
            dictionary.Add(dictionaryData.keys[i], dictionaryData.values[i]);
        }

        return dictionary;
    }

    // 덱 해금 추가
    public void AddUnlockedDeck(string deckName)
    {
        int index = playerData.unlockedDecks.FindIndex(x => x.deckName == deckName);

        if (index != -1)
        {
            Debug.LogError("해금하려는 덱이 이미 존재함: " + deckName); ;
            return;
        }

        DeckType deckType = Enums.GetEnumByString<DeckType>(deckName);
        DeckInfo deckInfo = new DeckInfo(deckType, 0);
        playerData.unlockedDecks.Add(deckInfo);
    }

    // 덱 레벨 업데이트
    public void UpdateDeckLevel(DeckType deckType, int clearedLevel)
    {
        int index = playerData.unlockedDecks.FindIndex(x => x.deckType == deckType);

        if (index == -1)
        {
            Debug.LogError("레벨 업데이트하려는 덱이 존재하지 않음" + deckType);
            return;
        }

        DeckInfo deckInfo = playerData.unlockedDecks[index];

        if (deckInfo.level == clearedLevel && deckInfo.level < DeckInfo.MAX_LEVEL)
        {
            deckInfo.level++;
            playerData.unlockedDecks[index] = deckInfo;
        }
    }

    // 아이템 해금 추가
    public void AddUnlockedItem(string itemID)
    {
        playerData.unlockedItems.Add(itemID);
    }

    // 데이터 업데이트 함수 ------------------------------------------------
    public void SetTutorialValue(bool value)
    {
        playerData.tutorialValue = value;
    }
    
    public void UpdateBlockPlaceCount(Block block)
    {
        switch (block.Type)
        {
            case BlockType.I:
                playerData.placeCountI++;
                UnlockManager.instance.onPlaceCountIUpdate?.Invoke(playerData.placeCountI);
                break;
            case BlockType.O:
                playerData.placeCountO++;
                UnlockManager.instance.onPlaceCountOUpdate?.Invoke(playerData.placeCountO);
                break;
            case BlockType.Z:
                playerData.placeCountZ++;
                UnlockManager.instance.onPlaceCountZUpdate?.Invoke(playerData.placeCountZ);
                break;
            case BlockType.S:
                playerData.placeCountS++;
                UnlockManager.instance.onPlaceCountSUpdate?.Invoke(playerData.placeCountS);
                break;
            case BlockType.J:
                playerData.placeCountJ++;
                UnlockManager.instance.onPlaceCountJUpdate?.Invoke(playerData.placeCountJ);
                break;
            case BlockType.L:
                playerData.placeCountL++;
                UnlockManager.instance.onPlaceCountLUpdate?.Invoke(playerData.placeCountL);
                break;
            case BlockType.T:
                playerData.placeCountT++;
                UnlockManager.instance.onPlaceCountTUpdate?.Invoke(playerData.placeCountT);
                break;
        }

        switch (block.Type)
        {
            case BlockType.I:
            case BlockType.O:
                UnlockManager.instance.onPlaceCountIOUpdate?.Invoke(playerData.placeCountI + playerData.placeCountO);
                break;
            case BlockType.Z:
            case BlockType.S:
                UnlockManager.instance.onPlaceCountZSUpdate?.Invoke(playerData.placeCountZ + playerData.placeCountS);
                break;
            case BlockType.J:
            case BlockType.L:
            case BlockType.T:
                UnlockManager.instance.onPlaceCountJLUpdate?.Invoke(playerData.placeCountJ + playerData.placeCountL);
                break;
        }
    }
    public void UpdateRerollCount()
    {
        playerData.rerollCount++;
        UnlockManager.instance.onRerollCountUpdate?.Invoke(playerData.rerollCount);
    }
    public void UpdateItemPurchaseCount()
    {
        playerData.itemPurchaseCount++;
        UnlockManager.instance.onItemPurchaseCountUpdate?.Invoke(playerData.itemPurchaseCount);
    }
    public void UpdateShopRerollCount()
    {
        playerData.shopRerollCount++;
        UnlockManager.instance.onShopRerollCountUpdate?.Invoke(playerData.shopRerollCount);
    }
    public void UpdateMaxScore(int score)
    {
        if (score > playerData.maxScore)
        {

            playerData.maxScore = score;
            UnlockManager.instance.onMaxScoreUpdate?.Invoke(playerData.maxScore);
        }
    }
    public void UpdateMaxChapterStage(int chapter, int stage)
    {
        if ((chapter > playerData.maxChapter) ||
            (chapter == playerData.maxChapter &&
             stage > playerData.maxStage))
        {
            playerData.maxChapter = chapter;
            playerData.maxStage = stage;
            UnlockManager.instance.onMaxChapterUpdate?.Invoke(chapter);
        }
    }
    public void UpdateWinCount()
    {
        playerData.winCount++;
        UnlockManager.instance.onWinCountUpdate?.Invoke(playerData.winCount);
    }
    public void UpdateBlockRerollCount(BlockData block)
    {
        switch (block.type)
        {
            case BlockType.I:
            case BlockType.O:
                playerData.rerollCountIO++;
                UnlockManager.instance.onRerollCountIOUpdate?.Invoke(playerData.rerollCountIO);
                break;
            case BlockType.Z:
            case BlockType.S:
                playerData.rerollCountZS++;
                UnlockManager.instance.onRerollCountZSUpdate?.Invoke(playerData.rerollCountZS);
                break;
            case BlockType.J:
            case BlockType.L:
            case BlockType.T:
                playerData.rerollCountJLT++;
                UnlockManager.instance.onRerollCountJLTUpdate?.Invoke(playerData.rerollCountJLT);
                break;
        }
    }
    public void UpdateMaxBaseMultiplier(int value)
    {
        if (value > playerData.maxBaseMultiplier)
        {
            playerData.maxBaseMultiplier = value;
            UnlockManager.instance.onMaxBaseMultiplierUpdate?.Invoke(playerData.maxBaseMultiplier);
        }
    }
    public void UpdateMaxMultiplier(int value)
    {
        if (value > playerData.maxMultiplier)
        {
            playerData.maxMultiplier = value;
            UnlockManager.instance.onMaxMultiplierUpdate?.Invoke(playerData.maxMultiplier);
        }
    }
    public void UpdateMaxBaseRerollCount(int value)
    {
        if (value > playerData.maxBaseRerollCount)
        {
            playerData.maxBaseRerollCount = value;
            UnlockManager.instance.onMaxBaseRerollCountUpdate?.Invoke(playerData.maxBaseRerollCount);
        }
    }
    public void UpdateMaxGold(int value)
    {
        if (value > playerData.maxGold)
        {
            playerData.maxGold = value;
            UnlockManager.instance.onMaxGoldUpdate?.Invoke(playerData.maxGold);
        }
    }
    public void UpdateHasOnlySpecificBlock(List<BlockData> deck)
    {
        if (deck.All(block => block.type == BlockType.I || block.type == BlockType.O))
        {
            playerData.hasOnlyIO++;
            UnlockManager.instance.onHasOnlyIOUpdate?.Invoke(playerData.hasOnlyIO);
        }
        else if (deck.All(block => block.type == BlockType.Z || block.type == BlockType.S))
        {
            playerData.hasOnlyZS++;
            UnlockManager.instance.onHasOnlyZSUpdate?.Invoke(playerData.hasOnlyZS);
        }
        else if (deck.All(block => block.type == BlockType.J || block.type == BlockType.L))
        {
            playerData.hasOnlyJL++;
            UnlockManager.instance.onHasOnlyJLUpdate?.Invoke(playerData.hasOnlyJL);
        }
    }

    public void UpdateDeckWinCount(DeckType deckType)
    {
        switch (deckType)
        {
            case DeckType.Default:
                playerData.defaultDeckWinCount++;
                UnlockManager.instance.onDefaultDeckWinCountUpdate?.Invoke(playerData.defaultDeckWinCount);
                break;
            case DeckType.YoYo:
                playerData.yoyoDeckWinCount++;
                UnlockManager.instance.onYoYoDeckWinCountUpdate?.Invoke(playerData.yoyoDeckWinCount);
                break;
            case DeckType.Dice:
                playerData.diceDeckWinCount++;
                UnlockManager.instance.onDiceDeckWinCountUpdate?.Invoke(playerData.diceDeckWinCount);
                break;
            case DeckType.Telescope:
                playerData.telescopeDeckWinCount++;
                UnlockManager.instance.onTelescopeDeckWinCountUpdate?.Invoke(playerData.telescopeDeckWinCount);
                break;
            case DeckType.Mirror:
                playerData.mirrorDeckWinCount++;
                UnlockManager.instance.onMirrorDeckWinCountUpdate?.Invoke(playerData.mirrorDeckWinCount);
                break;
            case DeckType.Bomb:
                playerData.bombDeckWinCount++;
                UnlockManager.instance.onBombDeckWinCountUpdate?.Invoke(playerData.bombDeckWinCount);
                break;
        }
    }

    public void UpdateClearedMaxLevel(int value)
    {
        if (value > playerData.clearedMaxLevel)
        {
            playerData.clearedMaxLevel = value;
            UnlockManager.instance.onClearedMaxLevelUpdate?.Invoke(playerData.clearedMaxLevel);
        }
    }
    // ---------------------------------------------------------------------
}
