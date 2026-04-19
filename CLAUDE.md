# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

**BlockBlock**은 Unity 2022.3.62f2 기반의 덱빌딩 로그라이크 블록 퍼즐 모바일 게임입니다 (한국 팀 프로젝트, 4인). 이전 이름이 **BlockBurst**라 `BlockBlock.slnx`와 `BlockBurst.slnx`가 공존하고, 코드·문서에도 두 이름이 섞여 있습니다.

**빌드 타겟은 Android 중심이며 iOS가 서브입니다.** Windows용 씬·분기 코드 (`ApplicationType.Windows`, `NewLogoScene` 등)가 남아 있지만 현재는 거의 사용되지 않습니다 — 새 기능을 설계할 때 Windows 경로를 우선 고려할 필요는 없습니다.

빌드·테스트 스크립트는 없고 모든 빌드·플레이·검증은 Unity Editor에서 수행합니다. 자동화된 테스트 스위트가 존재하지 않습니다.

## Workflow Principles

### 1. Think Before Coding — 먼저 생각하고 코딩하라

- 작업 전에 **관련 파일을 읽고 TodoWrite로 계획**을 세울 것
- **사용자와 계획을 확인**한 뒤 구현 시작
- 가정이 있으면 **명시적으로 밝히고**, 불확실하면 코딩 전에 물어볼 것
- 여러 접근법이 가능하면 **트레이드오프와 함께 제시**하고 사용자가 선택하게 할 것
- 더 단순한 방법이 있으면 복잡한 방법 대신 **먼저 제안**할 것

### 2. Simplicity First — 단순함을 우선하라

- 요청받은 것 **이상의 기능을 추가하지 말 것** (불필요한 추상화, 설정 옵션, 에러 핸들링 금지)
- 한 번만 쓰이는 코드에 헬퍼/유틸리티/추상화를 만들지 말 것
- 200줄로 작성한 코드가 50줄로 가능하면 **다시 작성**할 것
- 자기 점검: "시니어 엔지니어가 이걸 보고 과하다고 할까?"

### 3. Surgical Changes — 외과적으로 변경하라

- **최소 변경 원칙** — 대규모 리팩토링 금지, 영향 범위 최소화
- 요청과 **무관한 코드·주석·포맷팅을 건드리지 말 것**
- 기존 코드 스타일을 따를 것 (본인 선호와 달라도)
- 자기 점검: "변경된 모든 줄이 사용자 요청으로 추적 가능한가?"

### 4. Goal-Driven Execution — 목표 중심으로 실행하라

- 각 변경에 대해 **간단한 고수준 설명** 제공
- 모호한 요청은 **검증 가능한 목표로 변환**하여 확인받을 것
- 다단계 작업에는 **단계별 검증 계획**을 제시할 것
- 작업 종료 시 변경 사항 요약

## 응답 언어·톤

- 항상 **한국어 존댓말**로 답변합니다.
- 코드 주석은 한국어, 식별자는 영어입니다. 기존 톤을 따릅니다.

## 코드 스타일 (사용자 선호)

- 사용자가 `??=` 문법에 익숙하지 않으므로, 코드에서 **`??=` 연산자를 사용하지 말고** 명시적 `if (x == null) { x = ...; }` 형태를 사용합니다.
- `if`, `foreach`, `while` 등 제어문 본문이 한 줄뿐이어도 **항상 중괄호 `{ }`로 블록을 감쌉니다.**
- `foreach` 등 반복 변수는 `s`, `e` 같은 한 글자 대신 **`StageData`면 `stage`**, 요소 의미에 맞는 이름 (`item`, `constraint` 등)을 씁니다.
- **함수 오버로딩**은 가독성을 고려하여 최대한 쓰지 말고, 정말 필요한 경우에만 사용합니다.
- **null 체크**는 실제 제어 흐름을 읽고 판단합니다. 앞 단계에서 이미 null이 아님이 보장되면 (가드·초기화·계약 등) 이후에 동일 값에 대해 반복해서 `null` 검사를 붙이지 말고, 방어 코드를 남발하지 않습니다. 반대로 흐름상 null일 수 있는 지점에서는 필요한 만큼만 명확히 검사합니다.

## 구현 시 주의사항

[Docs/ImplementationNotes.md](Docs/ImplementationNotes.md) — 효과(Effect) 관련 작업 전 반드시 읽어야 합니다.

**EffectData 참조 유일성 규칙:** 하나의 `EffectData` 에셋이 `RunData.activeEffects`에 동시에 올라갈 수 있는 여러 "소스"에 동시에 속하면 안 됩니다. 소스는 다음과 같습니다.

- `ItemData.effects` (ITEM/BOOST — 구매 시 `AddEffect`)
- `ItemData.effects` (UPGRADE — 구매 시 등록)
- `DeckData.effects` / `GameData.defaultEffects` (런 시작 시 등록)
- `StageData.constraints` (스테이지 진입 시 등록)
- `BlockData.effects` (특수 블록 경로)

규칙을 어기면 `OnItemDiscard`의 template 기반 `EffectState` 제거가 다른 출처 state까지 지워 버립니다. 공유가 꼭 필요하면 에셋을 분리하거나 출처 태그를 도입합니다.

## 아키텍처

### 데이터 수명 레이어 (4단계)

README의 설계 의도와 [GameManager.cs](Assets/Scripts/Game/GameManager.cs) 구현이 일치합니다.

| 레이어 | 수명 | 생성 시점 | 비고 |
|---|---|---|---|
| `PlayerData` | 런을 넘어 영구 | `LoadPlayerData()` — 씬 로드 시 디스크에서 | 해금, 통계, 덱 레벨. `PlayerData.json`. |
| `GameData` | 한 게임 전체 | `StartNewGame(deck, level)` | 템플릿 + 점수표 + 보드 크기. `ContinueGame`에서는 저장된 덱·레벨 id로 재구성. |
| `RunData` | 한 번의 플레이 (챕터/스테이지 진행) | `StartNewRun()` | 골드, `activeItems`/`activeBoosts`/`activeEffects`, 진행 상태, history. 이어하기를 위해 `RunData.json`으로 저장. |
| `BlockGameData` | 스테이지 1회 | `StartStage(stage)` | 보드 상태, 손패, 매치, 덱 잔여. 메모리 전용. |

진입 흐름: `StartNewGame` → `StartNewRun` → `StartStageSelection` → `OnStageSelection` → `StartStage` → (플레이) → `EndStage` → `StartShop` → 다음 스테이지 혹은 `EndGame`.
이어하기 흐름: `ContinueGame` — `RunData.json` 로드, 저장된 id로 `GameData` 재구성, 스테이지 선택 건너뜀.
부활 흐름: `ResumeGame` — 보상형 광고 성공 후 `TryReviveWithAd`에서 호출, 현재 메모리의 `RunData`로 같은 스테이지 재시도.

### 싱글톤 (모두 `public static ... instance`)

- **`GameManager`** — 게임·런·스테이지 레이어를 조율하고 네 데이터 객체를 보유. 대부분의 UI 콜백 진입점 (`OnStageSelection`, `OnItemPurchased`, `OnShopReroll`, `TryPlaceBlock` 등).
- **`DataManager`** — `PlayerData` / `RunData` JSON 저장·로드. 통계 누적 (승수, 블록 배치 수, 리롤 수, 최고 점수 등). `RunData` 저장은 generation 번호 + 임시 파일 swap을 사용한 백그라운드 워커, `PlayerData` 저장은 동기.
- **`ScriptableDataManager`** — `Resources.LoadAll`로 `Assets/Resources/ScriptableObjects/{Deck,Level,Stage,Item,Block,Effect}` 아래 모든 SO를 로드, `id`→SO 조회 API 제공 (`TryGetStage`, `GetEffect` 등). 씬 로드 시 인스턴스가 없으면 `GameManager`에 컴포넌트로 재추가.
- **`UnlockManager`** — `PlayerData` 기준으로 해금 조건을 판정하고 해금 이벤트를 발생.
- **`EffectManager`** — 런타임 효과 상태 소유. `TriggerEffects(TriggerType, ...)`가 `runData.activeEffects`를 순회하며 trigger·블록·카운트 매칭 후 적용·애니메이션 enqueue. `EndTriggerEffect()` / `EndTriggerEffectOnPlace(matches)`로 배치 종료.
- **`CellEffectManager`** — 보드 셀 단위 시각 효과.
- **`AnimationManager`** — Queue 기반 애니메이션 파이프라인 (라인 클리어, 점수·칩·배수 갱신, 아이템 효과, 딜레이). DOTween + Coroutine 사용.
- **`GameUIManager`** — [Assets/Scripts/UI/InGame/](Assets/Scripts/UI/InGame/) 아래 약 30개 UI 컴포넌트의 파사드. README가 말하는 MVP에서 Presenter 역할을 `GameUIManager`가 겸함 (별도 Presenter 레이어 없음).
- **`SceneTransitionManager`** — 와이프 방식 씬 전환. `SceneManager.LoadScene` 대신 `TransitionToScene("...")`을 사용합니다.
- **`AudioManager`** — FMOD 기반. `isInitialized` 플래그, `BeginBackgroundMusic`, `SFX*` 메서드. `GameManager.OnSceneLoaded`는 초기화를 기다린 뒤 음악을 시작합니다.
- **`AdManager`** — Google Mobile Ads (보상형). `ShowReviveAd(onRewarded, onFailed)`, `ShowDeckUnlockAd(...)`. `GameManager.TryReviveWithAd` / `TryDeckUnlockWithAd`를 통해 호출됩니다.

### ScriptableObject 데이터

모든 SO는 [`BaseData`](Assets/Scripts/ScriptableData/BaseData.cs)를 상속하며 자동 생성 GUID `id`와 저자 지정 `resourceKey`를 모두 가집니다. 최근 리팩토링 (`refactor: Rename ScriptableObject id field to resourceKey`)으로 사람이 읽는 키가 `resourceKey`로 옮겨졌습니다. 저자 대상 조회 (firstStageList, 해금 키 등)에는 `resourceKey`, 저장 파일 참조에는 `id`를 씁니다.

타입: `BlockData`, `DeckData`, `EffectData`, `ItemData`, `LevelData`, `StageData`, `UnlockInfo`. 런타임에 추가된 필드 (예: `DeckData.Initialize` 경로)는 에셋에 역기록되지 않도록 합니다 — 저장 경로는 SO가 아니라 `RunData`를 거칩니다.

### 저장 시스템

- 경로: Android/WebGL에서는 `Application.persistentDataPath` (실배포 경로), Editor·그 외 플랫폼에서는 `Application.dataPath/Data/`. 파일: `PlayerData.json`, `RunData.json`.
- `RunData`는 [`RunSaveMapper`](Assets/Scripts/Data/RunSaveMapper.cs)를 통해 [`RunSaveData`](Assets/Scripts/Data/RunSaveData.cs)로 매핑되면서 SO 참조가 문자열 `id`로 평탄화되고, generation 번호 태그 + 임시 파일 → 최종 파일 원자적 swap으로 기록됩니다 (재시도 포함).
- Dictionary 직렬화: [`SerializableDictionary`](Assets/Scripts/Data/SerializableDictionary.cs)가 `ISerializationCallbackReceiver`로 병렬 key/value 리스트를 관리합니다. 구체 서브클래스 (`BlockTypeIntDictionary`, `MatchTypeIntDictionary`, `ItemTypeIntDictionary`, `ItemRarityIntDictionary`)를 씁니다. README는 리플렉션 기반 자동 직렬화를 언급하지만, 현재 사용 중인 것은 이 콜백 기반 Wrapper입니다 — 새 Dictionary가 필요하면 리플렉션 도입 대신 타입별 서브클래스를 추가합니다.

### 효과·트리거 시스템

`RunData.activeEffects`의 `EffectState`는 `effectDataId` (SO 역참조용), `effectValue`, `triggerCount`, 연쇄 효과용 `modifyingTargetStateId`를 보유합니다. SO는 항상 `ScriptableDataManager.instance.GetEffect(state.effectDataId)`로 해석하며, state에 참조를 캐시하지 않습니다.

`EffectScope.Stage` 효과는 스테이지 종료 시 제거되고, `Run` 효과는 남습니다. `TriggerMode.Interval`은 `triggerCount` vs `triggerValue` 비교, `Exact`는 동등할 때 발동합니다.

### 씬

[Assets/Scenes/](Assets/Scenes/)에 여러 씬이 있지만 **실제 사용되는 건 `VerticalGameScene` (메인 게임플레이) 사실상 하나**입니다. `LogoVerti`, `HorizontalGameScene`, `NewLogoScene`, `AudioTestScene`, `A3Scene` 등은 레거시·테스트·플랫폼 분기용으로 남아 있을 뿐 현재 개발은 `VerticalGameScene` 기준입니다. `GameManager.SetApplicationType`의 플랫폼 분기도 실질적으로는 모바일 경로만 살아 있다고 보면 됩니다.

## 에디터 전용 디버그 키

[GameManager.Update](Assets/Scripts/Game/GameManager.cs) 내 `#if UNITY_EDITOR` 블록:

| 키 | 동작 |
|---|---|
| `R` | `SaveRunData(runData)` |
| `T` | `SavePlayerData(playerData)` |
| `S` | `EndStage()` (현재 스테이지 즉시 종료) |
| `D` | 강제 패배 (`runData.isDefeated = true` + 저장 + `EndGame(false)`) |
| `G` | `DebugAddGold()` — 골드 +5000 |
| `Z` | `TryReviveWithAd(...)` — 부활 광고 흐름 점검 |
| `X` | `TryDeckUnlockWithAd("Dice", ...)` |

## 주의할 점

- `.slnx` 파일이 두 개 (`BlockBlock.slnx`, `BlockBurst.slnx`) 있고 같은 프로젝트를 가리킵니다. 최근 chore로 기본 솔루션이 `BlockBlock.slnx`로 전환되었으니 그 쪽을 엽니다.
- `GameManager`는 `DontDestroyOnLoad`이며, 씬별 매니저 (`DeckManager`, `ShopManager`, `StageManager`, `AnimationManager`, `TutorialManager`)는 `OnSceneLoaded`에서 `VerticalGameScene` / `HorizontalGameScene`일 때만 `FindObjectOfType`으로 다시 잡습니다. 실질 경로는 `VerticalGameScene`입니다.
- 코드로 ScriptableObject 값을 바꾸고 Git에 반영하려면 `GameManager.SaveScriptableObjectChanges(so)` (에디터 전용 헬퍼)를 호출해야 합니다. 디버그 경로 밖에서는 쓰지 않는 게 좋습니다 — Git diff를 오염시킵니다.
- 실배포(Android/WebGL)는 `persistentDataPath`, Editor는 `Assets/Data/`에 쓰며 이 경로는 `.gitignore` 대상입니다.
- `EffectData` 에셋이 여러 `activeEffects` 소스에 동시에 속하는 문제는 런타임 예외가 아니라 **설계 레벨 버그**입니다 — 위 "구현 시 주의사항" 참고.