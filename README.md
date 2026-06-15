<img width="400" alt="Block_Block_썸네일" src="https://github.com/user-attachments/assets/864577f1-99dd-49d4-b0dc-163845cee59d" />
<p align="left">
  <img src="https://github.com/user-attachments/assets/14d9bfcc-3a5e-4946-a31c-3fb918072b19" width="30%" alt="GIF1" />
  <img src="https://github.com/user-attachments/assets/00ddefe2-af1f-4ff1-8f6d-5071ad8c15a3" width="30%" alt="GIF2" />
</p>

## 덱 빌딩 로그라이크 퍼즐
- Mobile, 팀 프로젝트 (4인, **팀장**)
- Unity, C#
- **장려상** (2025 **넥슨** 드림 멤버스), **우수상** (2025 **고연전** GameDev), **루키 부문 전시** (BIC 2025)
- ScriptableObject, JsonUtility, DOTween

## 주요 기능
### 수명 기반 데이터 관리 설계

- 이유
    - 단일 저장 구조로 데이터 관리 시 **초기화 범위 불명확**, 버그 원인 추적 어려움
    - 데이터의 수명과 역할 기준으로 구분 필요
- 방법 — 데이터를 **수명 기준 4계층**으로 분리
    - **PlayerData** : 영구 데이터 (해금, 통계)
    - **GameData** : 한 게임 전체에서 유지되는 공통 데이터
    - **RunData** : 한 번의 런 동안 유지되는 진행 데이터
    - **BlockGameData** : 스테이지별로 관리되는 세부 진행 데이터
- 효과
    - 데이터 버그 발생 시 **발생 시점 특정 용이**
    - 신규 데이터 추가 시 적절한 계층에 배치 → **확장성 확보**
    - **이어하기 / 부활 기능을 동일 구조로 지원**

#### RunData 저장 / 로드 흐름 (이어하기 기능)

- **저장**: `RunData` (SO 직접 참조) → `RunSaveMapper`가 SO를 ID로 변환 → `RunSaveData` (SO id로 평탄화) → `DataManager`가 JSON으로 기록
- **로드**: JSON → `DataManager` → `RunSaveData` → `RunSaveMapper`가 ID를 SO로 역참조 (`ScriptableDataManager` 경유) → `RunData` 복원
- **저장 무결성**: ScriptableObject 직접 참조는 instance ID 변경 시 저장 파일이 조용히 다른 SO를 가리키는 문제 발생
    - 저장용 데이터 클래스(`RunSaveData`) 및 `RunSaveMapper` 도입 → SO 참조를 **GUID로 저장**, 로드 시 `ScriptableDataManager`로 GUID를 SO로 역참조

### Dictionary 직렬화 리팩토링 (Reflection → Wrapper)

- 이유
    - Unity `JsonUtility`가 Dictionary 직렬화를 지원하지 않아 데이터 저장에 어려움 발생

- **1차 시도 — Reflection 기반 자동 직렬화**
    1. 클래스 필드를 Reflection으로 탐색
    2. `IDictionary.IsAssignableFrom`으로 타입을 선별
    3. 제네릭 메서드로 일괄 직렬화
    - 장점: 신규 Dictionary 추가 시 **저장 로직 수정 불필요**
    - 한계
        - **성능**: 매 저장마다 Reflection 호출 비용 발생
        - **타입 안정성**: 컴파일 타임 검증 불가 → 런타임 오류 가능성
        - **캡슐화**: private 필드를 외부에서 접근하게 됨

- **2차 시도 — SerializableDictionary Wrapper**
    1. 클래스 Dictionary를 Mapper로 `SerializableDictionary`에 매핑
    2. `ISerializationCallbackReceiver`가 key / value List를 동기화
    3. JSON 직렬화

- 효과 및 학습
    - Reflection 호출 제거
    - **타입 안정성과 캡슐화 확보**
    - **편의성을 위한 추상화가 항상 옳은 선택은 아니라는 점을 배움**

### 연출 시퀀싱 시스템 (명령 큐 + 단일 디스패처)

- 이유
    - 처음엔 각 UI가 애니메이션을 **직접 관리하는 분산 방식**
    - 상황에 따라 **연출 순서·횟수가 데이터와 어긋나는** 문제 발생

- 방법
    1. **명령화**: 애니메이션 1건을 `AnimationData`(struct)로 표현 (**Command 패턴**)
    2. **직렬화**: `Queue` + 코루틴으로 1건씩 순차 처리
    3. **스케줄링**: 코루틴이 Dequeue → 처리 → `WaitForSeconds` 반복
    4. **라우팅**: `UIManager`가 Animation Type에 따라 해당 UI 메서드 호출 (DOTween)
    - **콤보 가속**: 처리할수록 `speed *= 1.05` → 콤보가 길수록 연출 가속, `bool resetSpeed`로 콤보 구분

- 효과
    - 빠르게 연속 점수 획득 시 **연출은 1번, 점수는 합산되던 버그 해결**
    - 새 연출은 **enum + UI 메서드 + 트윈 구현**만으로 추가 → 확장성 확보
    - **연출 로직과 렌더링 분리**로 디버깅 용이

#### 처리 흐름

- `Animation Data` → Enqueue → `Animation Manager` (Queue + Coroutine) → 타입별 호출 → `UIManager` (타입 → UI 메서드 중개) → `UI 컴포넌트` (DOTween 애니메이션)

### 기타

- 아이템, 스테이지, 효과, 블록을 **ScriptableObject** 기반 데이터로 관리
