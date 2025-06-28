using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System;

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
            path = Application.persistentDataPath + "/";
        }
    }

    // 해금, 통계
    public void SavePlayerData(PlayerData playerData)
    {
        string savedJson = JsonUtility.ToJson(playerData, true);
        string dataPath = Path.Combine(path, "PlayerData.json");
        File.WriteAllText(dataPath, savedJson);
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
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(loadedJson);

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
}
