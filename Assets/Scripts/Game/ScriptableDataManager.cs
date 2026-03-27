using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources에서 로드한 ScriptableObject 배열과 id→SO 딕셔너리를 보관한다. <see cref="Initialize"/>는 <see cref="GameManager"/>의 씬 로드 시 호출된된다.
/// </summary>
public class ScriptableDataManager : MonoBehaviour
{
    public static ScriptableDataManager instance = null;

    private bool _initialized;

    public DeckData[] deckTemplates { get; private set; }
    public LevelData[] levelTemplates { get; private set; }
    public StageData[] stageTemplates { get; private set; }
    public ItemData[] itemTemplates { get; private set; }
    public BlockData[] blockTemplates { get; private set; }
    public EffectData[] effectTemplates { get; private set; }

    private readonly Dictionary<string, DeckData> _deckById = new();
    private readonly Dictionary<string, LevelData> _levelById = new();
    private readonly Dictionary<string, StageData> _stageById = new();
    private readonly Dictionary<string, ItemData> _itemById = new();
    private readonly Dictionary<string, BlockData> _blockById = new();
    private readonly Dictionary<string, EffectData> _effectById = new();

    // 싱글톤 등록 및 DontDestroyOnLoad (중복 컴포넌트는 제거).
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Resources에서 SO를 로드하고 배열·id 딕셔너리를 채운다.
    public void Initialize()
    {
        if (_initialized)
            return;

        deckTemplates = Resources.LoadAll<DeckData>("ScriptableObjects/Deck");
        levelTemplates = Resources.LoadAll<LevelData>("ScriptableObjects/Level");
        stageTemplates = Resources.LoadAll<StageData>("ScriptableObjects/Stage");
        itemTemplates = Resources.LoadAll<ItemData>("ScriptableObjects/Item");
        blockTemplates = Resources.LoadAll<BlockData>("ScriptableObjects/Block");
        effectTemplates = Resources.LoadAll<EffectData>("ScriptableObjects/Effect");

        BuildDictionary(_deckById, deckTemplates, nameof(DeckData));
        BuildDictionary(_levelById, levelTemplates, nameof(LevelData));
        BuildDictionary(_stageById, stageTemplates, nameof(StageData));
        BuildDictionary(_itemById, itemTemplates, nameof(ItemData));
        BuildDictionary(_blockById, blockTemplates, nameof(BlockData));
        BuildDictionary(_effectById, effectTemplates, nameof(EffectData));

        _initialized = true;
    }

    // 배열을 비우고 각 원소를 id→SO 딕셔너리에 등록한다.
    private static void BuildDictionary<T>(Dictionary<string, T> map, T[] items, string context) where T : BaseData
    {
        map.Clear();
        if (items == null)
            return;

        foreach (T item in items)
            TryRegister(map, item, context);
    }

    // 한 개 SO를 딕셔너리에 넣는다. 빈 id·중복 id(다른 인스턴스)는 경고만 남긴다.
    private static void TryRegister<T>(Dictionary<string, T> map, T item, string context) where T : BaseData
    {
        if (item == null)
            return;

        if (string.IsNullOrEmpty(item.id))
        {
            Debug.LogWarning($"{context}: empty id on asset '{item.name}'");
            return;
        }

        if (map.TryGetValue(item.id, out T existing))
        {
            if (!ReferenceEquals(existing, item))
                Debug.LogWarning($"{context}: duplicate id '{item.id}' — different instances ('{existing.name}' vs '{item.name}')");
            return;
        }

        map[item.id] = item;
    }

    // 조회 API 호출 전 초기화 여부를 검사한다.
    private bool EnsureInitialized(string caller)
    {
        if (_initialized)
            return true;

        Debug.LogError($"ScriptableDataManager: {caller} called before Initialize().");
        return false;
    }

    // EffectData를 id로 조회한다. 없으면 false.
    public bool TryGetEffect(string id, out EffectData data)
    {
        data = null;
        if (!EnsureInitialized(nameof(TryGetEffect)))
            return false;
        if (string.IsNullOrEmpty(id))
            return false;
        return _effectById.TryGetValue(id, out data);
    }

    // EffectData를 id로 가져온다. 없으면 null과 에러 로그.
    public EffectData GetEffect(string id)
    {
        if (!TryGetEffect(id, out EffectData data))
        {
            if (_initialized && !string.IsNullOrEmpty(id))
                Debug.LogError($"ScriptableDataManager.GetEffect: no EffectData for id '{id}'.");
            return null;
        }

        return data;
    }

    // DeckData를 id로 조회한다. 없으면 false.
    public bool TryGetDeck(string id, out DeckData data) => TryGet(_deckById, id, out data, nameof(TryGetDeck));
    // DeckData를 id로 가져온다. 없으면 null과 에러 로그.
    public DeckData GetDeck(string id) => Get(_deckById, id, nameof(DeckData));

    // LevelData를 id로 조회한다. 없으면 false.
    public bool TryGetLevel(string id, out LevelData data) => TryGet(_levelById, id, out data, nameof(TryGetLevel));
    // LevelData를 id로 가져온다. 없으면 null과 에러 로그.
    public LevelData GetLevel(string id) => Get(_levelById, id, nameof(LevelData));

    // StageData를 id로 조회한다. 없으면 false.
    public bool TryGetStage(string id, out StageData data) => TryGet(_stageById, id, out data, nameof(TryGetStage));
    // StageData를 id로 가져온다. 없으면 null과 에러 로그.
    public StageData GetStage(string id) => Get(_stageById, id, nameof(StageData));

    // ItemData를 id로 조회한다. 없으면 false.
    public bool TryGetItem(string id, out ItemData data) => TryGet(_itemById, id, out data, nameof(TryGetItem));
    // ItemData를 id로 가져온다. 없으면 null과 에러 로그.
    public ItemData GetItem(string id) => Get(_itemById, id, nameof(ItemData));

    // BlockData를 id로 조회한다. 없으면 false.
    public bool TryGetBlock(string id, out BlockData data) => TryGet(_blockById, id, out data, nameof(TryGetBlock));
    // BlockData를 id로 가져온다. 없으면 null과 에러 로그.
    public BlockData GetBlock(string id) => Get(_blockById, id, nameof(BlockData));

    // 공통: 딕셔너리에서 id로 SO를 찾는다(Try 패턴).
    private bool TryGet<T>(Dictionary<string, T> map, string id, out T data, string caller) where T : BaseData
    {
        data = null;
        if (!EnsureInitialized(caller))
            return false;
        if (string.IsNullOrEmpty(id))
            return false;
        return map.TryGetValue(id, out data);
    }

    // 공통: id로 SO를 반환한다. 없으면 null과 에러 로그.
    private T Get<T>(Dictionary<string, T> map, string id, string typeName) where T : BaseData
    {
        if (!TryGet(map, id, out T data, nameof(Get)))
        {
            if (_initialized && !string.IsNullOrEmpty(id))
                Debug.LogError($"ScriptableDataManager: no {typeName} for id '{id}'.");
            return null;
        }

        return data;
    }
}
