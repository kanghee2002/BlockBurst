# Block Burst - 구현 명세서

## 1. 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── BlockGame/           # BlockGame Layer - 블록 게임 로직
│   │   ├── Board.cs        # 보드 관리 및 매치 처리
│   │   ├── Cell.cs         # 개별 셀 상태 관리
│   │   ├── Block.cs        # 블록 정의 및 조작
│   │   ├── ScoreCalculator.cs  # 점수 계산
│   │   └── BlockGameData.cs    # 게임 진행 데이터
│   │
│   ├── Run/                 # Run Layer - 게임 진행
│   │   ├── DeckManager.cs   # 덱 관리
│   │   ├── EffectManager.cs # 효과 관리
│   │   ├── StageManager.cs  # 스테이지 진행
│   │   ├── ShopManager.cs   # 상점 관리
│   │   └── RunData.cs       # 런 진행 데이터
│   │
│   ├── Game/                # Game Layer - 게임 초기화
│   │   ├── GameManager.cs   # 게임 전체 관리
│   │   └── GameData.cs      # 게임 기본 데이터
│   │
│   └── ScriptableData/      # 프리셋 데이터
│       ├── BlockData.cs     # 블록 프리셋
│       ├── StageData.cs     # 스테이지 프리셋
│       ├── ItemData.cs      # 아이템 프리셋
│       └── EffectData.cs    # 효과 프리셋
│
└── ScriptableObjects/       # 프리셋 에셋
    ├── Blocks/              # 블록 프리셋
    ├── Stages/              # 스테이지 프리셋
    ├── Items/               # 아이템 프리셋
    └── Effects/             # 효과 프리셋
```

## 2. 핵심 데이터 구조

### 2.1 Layer별 데이터

```csharp
// BlockGame Layer 데이터
public class BlockGameData
{
    // 현재 게임 상태
    public Dictionary<BlockType, int> blockScores;      // 블록별 점수
    public Dictionary<MatchType, float> matchMultipliers;  // 매치 타입별 배율
    public int currentScore;                            // 현재 점수
    public int moveCount;                               // 이동 횟수
    public Dictionary<string, HashSet<Vector2Int>> activeBlockCells;  // 현재 활성화된 블록 cell 위치

    public void Initialize(RunData runData)
    {
        blockScores = new Dictionary<BlockType, int>(runData.baseBlockScores);
        matchMultipliers = new Dictionary<MatchType, float>(runData.baseMatchMultipliers);
        currentScore = 0;
        moveCount = 0;
        activeBlockCells = new Dictionary<string, HashSet<Vector2Int>>();
    }
}

// Run Layer 데이터
public class RunData
{
    // 스테이지 진행 데이터
    public Dictionary<BlockType, int> baseBlockScores;     // 기본 블록 점수
    public Dictionary<MatchType, float> baseMatchMultipliers;  // 기본 매치 배율
    public List<BlockData> availableBlocks;               // 사용 가능한 블록들
    public List<ItemData> activeItems;                    // 활성화된 아이템들
    public List<EffectData> activeEffects;                // 활성화된 효과들
    public StageData currentStage;                        // 현재 스테이지
    public Dictionary<string, int> blockReuses;           // 블록별 재사용 횟수
    public int gold;                                      // 현재 보유 골드
    public int baseRerollCount;                           // 기본 리롤 횟수
    public int currentRerollCount;                        // 현재 리롤 횟수
    public float baseMultiplier;                          // 기본 배율

    public void Initialize(GameData gameData)
    {
        baseBlockScores = new Dictionary<BlockType, int>(gameData.defaultBlockScores);
        baseMatchMultipliers = new Dictionary<MatchType, float>(gameData.defaultMatchMultipliers);
        availableBlocks = new List<BlockData>(gameData.defaultBlocks);
        activeEffects = new List<EffectData>();
        blockReuses = new Dictionary<string, int>();
        gold = gameData.startingGold;
        baseRerollCount = gameData.defaultRerollCount;
        currentRerollCount = baseRerollCount;
        baseMultiplier = 1.0f;
    }
}

// Game Layer 데이터
public class GameData
{
    // 기본 게임 데이터
    public Dictionary<BlockType, int> defaultBlockScores;     // 기본 블록 점수
    public Dictionary<MatchType, float> defaultMatchMultipliers;  // 기본 매치 배율
    public List<BlockData> defaultBlocks;                     // 기본 블록 목록
    public List<StageData> stagePool;                        // 스테이지 풀
    public int startingGold;                                 // 시작 골드
    public int defaultRerollCount;                           // 기본 리롤 횟수

    public void Initialize()
    {
        // 기본값 설정
        defaultBlockScores = new Dictionary<BlockType, int>();
        defaultMatchMultipliers = new Dictionary<MatchType, float>();
        defaultBlocks = new List<BlockData>();
        stagePool = new List<StageData>();
        startingGold = 10;
        defaultRerollCount = 3;

        // 기본 블록 점수 설정
        defaultBlockScores[BlockType.I] = 100;
        defaultBlockScores[BlockType.O] = 100;
        defaultBlockScores[BlockType.T] = 150;
        defaultBlockScores[BlockType.L] = 120;
        defaultBlockScores[BlockType.J] = 120;
        defaultBlockScores[BlockType.S] = 130;
        defaultBlockScores[BlockType.Z] = 130;

        // 기본 매치 배율 설정
        defaultMatchMultipliers[MatchType.LINE] = 1.0f;
        defaultMatchMultipliers[MatchType.SQUARE] = 1.5f;
    }
}
```

### 2.2 ScriptableObject 데이터

```csharp
// 블록 프리셋
[CreateAssetMenu(fileName = "Block", menuName = "BlockBurst/Block")]
public class BlockData : ScriptableObject
{
    public string id;                      // 블록 고유 ID
    public BlockType type;                 // 블록 타입
    public bool[,] shape;                  // 블록 모양
    public List<EffectData> onPlaceEffects;  // 배치 시 효과
    public List<EffectData> onClearEffects;  // 제거 시 효과
    public int maxReuses;                  // 최대 재사용 횟수
}

// 스테이지 프리셋
[CreateAssetMenu(fileName = "Stage", menuName = "BlockBurst/Stage")]
public class StageData : ScriptableObject
{
    public string id;                        // 스테이지 ID
    public StageType type;                   // Normal1, Normal2, Boss
    public Vector2Int boardSize;             // 보드 크기
    public List<Vector2Int> blockedCells;    // 사용 불가능한 셀
    public List<EffectData> constraints;     // 스테이지 제한사항
    public StageRequirement clearRequirement; // 클리어 조건
    public int goldReward;                   // 클리어 보상
}

public class StageRequirement
{
    public int targetScore;      // 목표 점수
    public int maxMoves;         // 최대 이동 수
    public int minLineClears;    // 최소 라인 클리어 수
}

// 효과 프리셋
[CreateAssetMenu(fileName = "Effect", menuName = "BlockBurst/Effect")]
public class EffectData : ScriptableObject
{
    public string id;                                // 효과 ID
    public EffectType type;                         // 효과 타입
    public TriggerType trigger;                     // 발동 조건
    public Dictionary<string, float> modifiers;      // 데이터 수정자
}

public enum BlockType
{
    I, O, T, L, J, S, Z, SPECIAL
}

public enum StageType
{
    NORMAL1, NORMAL2, BOSS
}

public enum EffectType
{
    SCORE_MODIFIER,      // 점수 수정
    MULTIPLIER_MODIFIER, // 배율 수정
    REROLL_MODIFIER,     // 리롤 수정
    GOLD_MODIFIER,       // 골드 수정
    BOARD_MODIFIER,      // 보드 수정
    DECK_MODIFIER,       // 덱 수정
    BLOCK_REUSE,        // 블록 재사용
    SPECIAL_CLEAR       // 특수 클리어
}

public enum TriggerType
{
    ON_BLOCK_PLACE,          // 블록 배치 시
    ON_BLOCK_CLEAR,          // 블록 제거 시
    ON_LINE_CLEAR,           // 라인 클리어 시
    ON_LINE_CLEAR_WITH_BLOCK, // 특정 블록으로 라인 클리어 시
    ON_DECK_EMPTY,           // 덱이 비었을 때
    ON_REROLL,              // 리롤 시
    ON_BOARD_STATE,         // 보드 상태 조건 충족 시
}

// 아이템 프리셋
[CreateAssetMenu(fileName = "Item", menuName = "BlockBurst/Item")]
public class ItemData : ScriptableObject
{
    public string id;                  // 아이템 ID
    public List<EffectData> effects;   // 아이템 효과
    public int cost;                   // 구매 비용
    public string targetBlockId;       // 대상 블록 ID (필요한 경우)
}

public enum MatchType
{
    ROW,    // 행 매치
    COLUMN, // 열 매치
    SQUARE  // 정사각형 매치
}
```

## 3. 핵심 시스템 구현

### 3.1 BlockGame Layer

```csharp
public class Board : MonoBehaviour
{
    private Cell[,] cells;
    private BlockGameData gameData;

    // 블록 배치 처리
    public bool PlaceBlock(Block block, Vector2Int pos);

    // 블록 배치 가능 여부 확인
    private bool CanPlace(Block block, Vector2Int pos);

    // 매치 처리
    public void ProcessMatches();

    // 매치 확인
    private List<Match> CheckMatches();

    // 행 매치 확인
    private Match CheckRowMatch(Vector2Int pos);

    // 열 매치 확인
    private Match CheckColumnMatch(Vector2Int pos);

    // 정사각형 매치 확인
    private Match CheckSquareMatch(Vector2Int pos);
}

public class ScoreCalculator
{
    public static ScoreCalculator Instance { get; private set; }

    // 점수 계산
    public int Calculate(Match match, BlockGameData data);

    // 블록별 점수 계산
    private int CalculateBlockScore(BlockType type, BlockGameData data);
}

public class Cell
{
    public BlockType? Type { get; private set; }
    public string BlockId { get; private set; }
    public bool IsBlocked { get; private set; }

    // 블록 설정
    public void SetBlock(BlockType type, string blockId);

    // 블록 제거
    public void ClearBlock();

    // 셀 차단 설정
    public void SetBlocked(bool blocked);
}

public class Block
{
    public string Id { get; private set; }
    public BlockType Type { get; private set; }
    public bool[,] Shape { get; private set; }
    public List<EffectData> OnPlaceEffects { get; private set; }
    public List<EffectData> OnClearEffects { get; private set; }
    public int MaxReuses { get; private set; }

    // 블록 위치 계산
    public List<Vector2Int> GetPositions(Vector2Int origin);
}
```

### 3.2 Run Layer

```csharp
public class EffectManager : MonoBehaviour
{
    private RunData runData;
    private BlockGameData gameData;
    private Dictionary<TriggerType, List<EffectData>> triggerEffects;

    // 효과 추가
    public void AddEffect(EffectData effect);

    // 효과 제거
    public void RemoveEffect(EffectData effect);

    // 효과 트리거
    public void TriggerEffects(TriggerType trigger);

    // 효과 적용
    private void ApplyEffect(EffectData effect);
}

public class DeckManager : MonoBehaviour
{
    private List<BlockData> deck;
    private List<BlockData> discardPile;
    private RunData runData;

    // 덱 초기화
    public void Initialize(RunData data);

    // 블록 뽑기
    public BlockData DrawBlock();

    // 덱 셔플
    private void ShuffleDeck();

    // 덱 리롤
    public bool RerollDeck();

    // 블록 재사용 처리
    public void ProcessBlockReuse(string blockId);

    // 블록 추가
    public void AddBlock(BlockData block);

    // 블록 제거
    public void RemoveBlock(BlockData block);
}

public class StageManager : MonoBehaviour
{
    private RunData runData;
    private StageData currentStage;
    private BlockGameData gameData;

    // 스테이지 시작
    public void StartStage(StageData stage);

    // 스테이지 클리어 체크
    public bool CheckStageClear();

    // 스테이지 제한사항 적용
    private void ApplyConstraints();

    // 클리어 보상 지급
    private void GrantReward();
}

public class ShopManager : MonoBehaviour
{
    private RunData runData;
    private List<ItemData> currentItems;

    // 상점 초기화
    public void InitializeShop();

    // 아이템 구매
    public bool PurchaseItem(string itemId);

    // 아이템 적용
    private void ApplyItem(ItemData item);

    // 블록 강화
    public void UpgradeBlock(string blockId);
}
```

### 3.3 Game Layer

```csharp
public class GameManager : MonoBehaviour
{
    private GameData gameData;
    private RunData currentRun;
    private BlockGameData blockGame;

    // 새 게임 시작
    public void StartNewGame();

    // 새로운 런 시작
    public void StartNewRun();

    // 스테이지 시작
    public void StartStage(StageData stage);

    // 스테이지 종료
    public void EndStage(bool cleared);

    // 게임 종료
    public void EndGame();
}
```

### 3.4 시스템 흐름도

#### 3.4.1 초기화 흐름

```
GameManager.StartNewGame()
└── GameData 초기화
    ├── 기본 블록 설정
    ├── 기본 점수/배율 설정
    └── 스테이지 풀 설정

GameManager.StartNewRun()
└── RunData 초기화
    ├── 덱 초기화
    │   ├── 기본 블록 추가
    │   └── 덱 셔플
    ├── 점수/배율 초기화
    └── 골드/리롤 초기화

StageManager.StartStage()
└── BlockGameData 초기화
    ├── 보드 초기화
    ├── 점수/배율 초기화
    └── 제한사항 적용
```

#### 3.4.2 게임플레이 흐름

```
1. 블록 드로우
DeckManager.DrawBlock()
├── 재사용 가능한 경우
│   ├── 재사용 카운트 감소
│   └── 덱에 다시 추가
└── 블록 소멸

2. 블록 배치
Board.PlaceBlock()
├── 배치 가능 여부 확인
├── 블록 배치 효과 적용
└── 매치 확인 및 처리

3. 매치 처리
Board.ProcessMatches()
├── 매치 확인
├── 점수 계산
├── 블록 제거 효과 적용
└── 중력 적용
```

#### 3.4.3 스테이지 진행 흐름

```
1. 스테이지 선택 및 시작
├── 스테이지 데이터 로드
├── 제한사항 적용
└── 보드 초기화

2. 게임플레이 루프
├── 블록 드로우/배치
├── 매치 처리
└── 클리어 조건 확인

3. 스테이지 종료
├── 보상 획득
├── 상점 오픈
└── 다음 스테이지 선택
```

## 4. UI 시스템

### 4.1 UI 구조

```
Assets/
├── Scenes/
│   ├── GameScene           # 메인 게임 화면
│   └── StageScene          # 스테이지 선택 화면
│
└── Scripts/
    └── UI/
        ├── GameUIManager.cs    # UI 총괄 관리
        ├── UIEvents.cs         # UI 이벤트 정의
        └── InGame/
            ├── BoardUI.cs      # 보드 UI (블록 조작, 매치 효과 포함)
            ├── DeckUI.cs       # 덱 UI
            ├── ShopUI.cs       # 상점 UI
            └── InfoUI.cs       # 정보 UI (점수, 진행상태 포함)
```

### 4.2 UI 시스템 구현

```csharp
public class BoardUI : MonoBehaviour
{
    private Cell[,] boardCells;
    private Block currentBlock;
    private Vector2Int currentPosition;

    public void Initialize(Vector2Int size);
    public void UpdateBoard(Cell[,] cells);
    public void ShowBlockPreview(Block block, Vector2Int pos);
    public void ShowMatchEffect(List<Vector2Int> positions);
    public void ShowPlacementGuide(bool canPlace);
    public void UpdateBlockControl(Vector2Int pos);
    public void ShowMatchType(MatchType type, List<Vector2Int> positions);
    private void HandleMovementInput();
}

public class DeckUI : MonoBehaviour
{
    public void UpdateDeck(List<BlockData> deck);
    public void UpdateRerollCount(int count);
    public void ShowCurrentBlock(BlockData block);
    public void ShowBlockDetail(BlockData block);
    public void UpdateBlockReuses(Dictionary<string, int> reuseCounts);
}

public class ShopUI : MonoBehaviour
{
    public void DisplayItems(List<ItemData> items);
    public void UpdateGold(int gold);
    public void ShowItemDetail(ItemData item);
    public void ShowUpgradeOptions(BlockData block);
    public void UpdatePurchaseState(bool canPurchase);
}

public class InfoUI : MonoBehaviour
{
    public void UpdateScore(int currentScore, int targetScore);
    public void UpdateMoves(int remainingMoves, int totalMoves);
    public void UpdateMultiplier(float multiplier);
    public void ShowBlockScores(Dictionary<BlockType, int> scores);
    public void ShowStageProgress(StageRequirement requirement, StageProgress progress);
    public void ShowScorePopup(int amount, Vector2 position);
    public void UpdateLineClears(int current, int required);
}
```

### 4.3 UI 이벤트 시스템

```csharp
public static class UIEvents
{
    // 보드 관련
    public static event Action<Cell[,]> OnBoardChanged;
    public static event Action<Block, Vector2Int> OnBlockMoved;
    public static event Action<BlockData, Vector2Int> OnBlockPlaced;
    public static event Action<MatchType, List<Vector2Int>> OnMatchCompleted;

    // 덱 관련
    public static event Action<List<BlockData>> OnDeckChanged;
    public static event Action OnRerollRequested;

    // 점수/진행 관련
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnGoldChanged;
    public static event Action<StageProgress> OnProgressUpdated;

    // 상점 관련
    public static event Action<ItemData> OnItemSelected;
    public static event Action<BlockData> OnUpgradeSelected;
}
```

### 4.4 화면별 구성

```
1. 게임 화면

   - 좌측 영역
     - 블록별 점수
     - 진행 점수
     - 스테이지 진행 상태
   - 중앙 영역
     - 8x8 보드
     - 블록 회전/이동 컨트롤
     - 매치 효과
   - 우측 영역
     - 덱 목록
     - 리롤 버튼
     - 블록 재사용 횟수
   - 하단 영역
     - 현재 블록 정보
     - 배치 가능 영역 표시
   - 모달
     - 상점: 아이템 구매/블록 강화
     - 결과: 스테이지 결과

2. 스테이지 선택 화면
   - 스테이지 선택 카드 (2개)
   - 스테이지 정보
     - 목표 점수/이동 제한
     - 현재 제한사항
     - 보상 정보
   - 현재 덱 정보
     - 보유 블록 목록
     - 블록별 재사용 횟수
```
