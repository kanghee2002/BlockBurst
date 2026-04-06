using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    [NonSerialized] private Dictionary<TKey, TValue> dictionary;

    public SerializableDictionary()
    {
    }

    public SerializableDictionary(Dictionary<TKey, TValue> source)
    {
        if (source == null)
        {
            dictionary = null;
            return;
        }

        dictionary = new Dictionary<TKey, TValue>(source);
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        EnsureDictionary();
        return new Dictionary<TKey, TValue>(dictionary);
    }

    public void FromDictionary(Dictionary<TKey, TValue> source)
    {
        if (source == null)
        {
            dictionary = null;
            keys.Clear();
            values.Clear();
            return;
        }

        dictionary = new Dictionary<TKey, TValue>(source);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        EnsureDictionary();
        return dictionary.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            EnsureDictionary();
            return dictionary[key];
        }
        set
        {
            EnsureDictionary();
            dictionary[key] = value;
        }
    }

    public int Count
    {
        get
        {
            EnsureDictionary();
            return dictionary.Count;
        }
    }

    public void Clear()
    {
        EnsureDictionary();
        dictionary.Clear();
    }

    public void Add(TKey key, TValue value)
    {
        EnsureDictionary();
        dictionary.Add(key, value);
    }

    public bool Remove(TKey key)
    {
        EnsureDictionary();
        return dictionary.Remove(key);
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        if (dictionary == null || dictionary.Count == 0)
        {
            return;
        }

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        if (keys == null || values == null)
        {
            dictionary = new Dictionary<TKey, TValue>();
            return;
        }

        if (keys.Count != values.Count)
        {
            Debug.LogError(
                $"SerializableDictionary deserialization failed: keys({keys.Count}) != values({values.Count}) for {GetType().Name}");
            dictionary = new Dictionary<TKey, TValue>();
            return;
        }

        dictionary = new Dictionary<TKey, TValue>(keys.Count);
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    private void EnsureDictionary()
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<TKey, TValue>();
        }
    }
}

[Serializable]
public sealed class BlockTypeIntDictionary : SerializableDictionary<BlockType, int>
{
    public BlockTypeIntDictionary()
    {
    }

    public BlockTypeIntDictionary(Dictionary<BlockType, int> source)
        : base(source)
    {
    }
}

[Serializable]
public sealed class MatchTypeIntDictionary : SerializableDictionary<MatchType, int>
{
    public MatchTypeIntDictionary()
    {
    }

    public MatchTypeIntDictionary(Dictionary<MatchType, int> source)
        : base(source)
    {
    }
}

[Serializable]
public sealed class ItemTypeIntDictionary : SerializableDictionary<ItemType, int>
{
    public ItemTypeIntDictionary()
    {
    }

    public ItemTypeIntDictionary(Dictionary<ItemType, int> source)
        : base(source)
    {
    }
}

[Serializable]
public sealed class ItemRarityIntDictionary : SerializableDictionary<ItemRarity, int>
{
    public ItemRarityIntDictionary()
    {
    }

    public ItemRarityIntDictionary(Dictionary<ItemRarity, int> source)
        : base(source)
    {
    }
}

