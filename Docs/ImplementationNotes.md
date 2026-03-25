# 구현 시 주의사항

BlockBurst 코드·데이터를 설계·수정할 때 지키면 좋은 규칙과 함정을 이 문서에 모은다. 항목은 필요에 따라 추가한다.

## EffectData 참조 유일성 (디자인 규칙)

런타임에서 `EffectState.template`만으로 **어느 `ItemData` 슬롯인지** 찾거나, 판매 시 **해당 아이템에서 등록한 효과만** 제거하려면, 아래 범위에서 **같은 `EffectData` 에셋을 여러 “소스”가 동시에 쓰지 않는 것**을 권장한다.

### 규칙 (요약)

**동시에 `RunData.activeEffects`(또는 이를 대체하는 목록)에 올라갈 수 있는 출처끼리는, 같은 `EffectData` 인스턴스(에셋)를 공유하지 않는다.**

출처 예:

- `ItemData.effects` — 타입 `ITEM`, `BOOST` (구매 시 `AddEffect` 되는 것)
- `ItemData.effects` — 타입 `UPGRADE` (구매 시 등록)
- `DeckData.effects` / `GameData.defaultEffects` (런 시작 시 등록)
- `StageData.constraints` (스테이지 진입 시 등록)
- `BlockData.effects` (특수 블록 등, 해당 경로에서 등록되는 것)

즉, **한 `EffectData` 에셋은 위 카테고리 중 하나의 “논리적 소유”에만 속한다**고 보면 구현이 단순해진다.

### 코드에서 허용하는 가정 (규칙 준수 시)

1. **트리거 UI (`UpdateItemTriggerCount` 등)**  
   `EffectState.template`에 대해 `activeItems`에서 `item.effects.Contains(template)`로 **유일한 슬롯**을 찾을 수 있다.

2. **아이템 판매 (`OnItemDiscard`)**  
   `soldItem.effects`에 포함된 `template`과 일치하는 `EffectState`만 제거하면, **다른 출처(덱 기본, 제약 등)의 state를 잘못 지우지 않는다.**

### 예외 / 주의

- **서로 다른 `ItemData`가 같은 `EffectData`를 가리키되, 둘 다 `activeEffects`에 올라오지 않는 경우**는 규칙 위반이 아니다.  
  예: 현재 `ConvertBlock1` / `ConvertBlock2`가 동일 변환 효과 에셋을 쓰지만, `CONVERT_BLOCK` 경로에서는 해당 효과를 `activeEffects`에 넣지 않는다.
- 규칙을 나중에 깨뜨리면(예: 덱 기본 효과와 아이템 효과가 동일 에셋) **템플릿 기반 제거가 덱 쪽 state까지 지울 수 있으니**, 그때는 `EffectSource` 등으로 출처를 명시하거나 에셋을 분리한다.
- **한 `ItemData`의 `effects` 리스트 안에서 동일 에셋을 두 번 넣지 않는다** (한 아이템 = 효과 템플릿당 최대 하나의 런타임 인스턴스와 대응하기 쉬움).

### 검증

에셋 추가 시 `ITEM`/`BOOST` 간 EffectData guid 중복, 덱·제약·아이템 간 교집합 등은 스크립트로 주기적으로 확인할 수 있다. (과거에 리포지토리 기준으로 `ITEM`/`BOOST`끼리 공유 없음, 덱↔아이템·제약↔아이템 교집합 없음이 확인된 바 있음.)
