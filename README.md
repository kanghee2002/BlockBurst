<img width="400" alt="Block_Block_썸네일" src="https://github.com/user-attachments/assets/864577f1-99dd-49d4-b0dc-163845cee59d" />
<p align="left">
  <img src="https://github.com/user-attachments/assets/14d9bfcc-3a5e-4946-a31c-3fb918072b19" width="30%" alt="GIF1" />
  <img src="https://github.com/user-attachments/assets/00ddefe2-af1f-4ff1-8f6d-5071ad8c15a3" width="30%" alt="GIF2" />
</p>

## 덱 빌딩 로그라이크 퍼즐
- Mobile, 팀 프로젝트 (4인)
- **장려상** (2025 **넥슨** 드림 멤버스), **우수상** (2025 **네오위즈** GameDev)

## 주요 기능
### 딕셔너리 자동 직렬화

- 이유
    - Unity가 Dictionary 직렬화 지원하지 않아 JsonUtility 데이터 저장에 어려움 발생
- 방법
    - 대안 1
        - Dictionary마다 구조체로 변환하여 저장
        - Dictionary마다 함수 호출이 1번씩 필요 (Dictionary 추가 시 함수도 추가 필요)
    - 대안 2
        - Dictionary를 직렬화 가능한 Wrapper 클래스로 리팩토링
        - 구조적으로 적절하지만, 기존 코드 영향 범위가 커 단기 적용 어려움
    - **대안 3**
        - **Reflection 기반 Dictionary 자동 탐색 및 직렬화 → 선택**
- 구현
    1. Reflection으로 클래스 FieldInfo 탐색
    2. Type.IsAssignableFrom으로 IDictionary 타입 필드만 선별, 직렬화 수행
    3. Type이 정적이지 않아 제네릭 직렬화 함수 직접 호출 불가 → 패턴 매칭으로 해결
- 장점
    - 신규 Dicionary 필드 추가 시 저장 로직 수정 최소화
    - **클래스 내 Dictionary 한 번의 함수 호출로 직렬화 가능**
- 단점
    - Reflection 기반 구조로 인해 **성능과 타입 안정성, 캡슐화** 측면에서 한계
- 깨달은 점
    - 직렬화 문제를 Reflection으로 우회하면 **단기적인 확장성 확보** 가능하지만, **타입 안정성과 캡슐화가 무너질 수 있음**을 경험
    - 장기적으로는 직렬화 가능한 Wrapper 클래스 사용 필요성 인식

### 데이터 저장 시스템 (JSON)

- 이유
    - 통계/해금 기능 구현 시 책임 분리 필요

- 방법
    - **데이터 클래스**: PlayerData
    - **해금 정보 클래스**: UnlockInfo
    - **데이터 저장 담당**: DataManager
    - **해금 처리 담당**: UnlockManager
- 효과
    - 역할 분담을 통한 **단일 책임 원칙**
    - 간편한 데이터, 해금 정보 추가

### 게임 데이터 수명 기반 관리

- 이유
    - 초기에는 모든 데이터를 하나의 데이터로 관리
    - 초기화 범위 불명확, 버그 원인 추적 어려움
    - 데이터의 수명과 역할을 기준으로 구분 필요
- 방법
    - **DeckData/LevelData** : 재사용되는 설정 데이터
    - **GameData** : 게임 전체에서 유지되는 공통 데이터
    - **RunData** : 한 번의 플레이 동안 유지되는 진행 데이터
    - **BlockGameData** : 스테이지별로 관리되는 세부 진행 데이터
- 효과
    - 스테이지 진입 시 BlockGameData만 초기화되어 RunData는 안전하게 관리
    - 데이터 버그 발생 시 문제 발생 시점 빠르게 특정

### MVP 패턴

- 이유
    - UI 상호작용이 핵심인 게임 플레이
    - UI 로직 분리 및 View 책임 최소화 필요
- 대안
    - **MVC** : Model과 View 사이 의존성 문제
    - **MVP : UI와 데이터 간 명확한 역할 분리**
    - **MVVM** : 프로젝트 규모에 비해 과설계
- 효과
    - 화면 표시와 UI 흐름, 데이터를 명확히 분리
    - Model과 View를 독립적으로 개발하고 Presenter를 통해 테스트 → 생산성

### 기타

- 아이템, 스테이지, 효과, 블록을 Scriptable Object로 최적화
- Queue 기반 점수 처리 애니메이션 구조 설계
- DoTween과 Coroutine을 이용한 애니메이션 처리

## 의미
### 리더십 & 협업 방식에 대한 인식 변화
팀장 역할을 맡아 프로젝트를 이끌며, 팀 전체의 방향성을 맞추기 위해 **로드맵 제시와 정확한 목표 설정**이 중요함을 체감했습니다. 이를 통해 작업 우선순위와 목표를 명확히 설정하고 팀 내 생산성을 크게 올릴 수 있었습니다.

### 게임 재미에 대한 설계 관점 확장
단순 기능 구현을 넘어, 시각적 쾌감·시련과 성취의 밸런스가 플레이 지속성에 핵심이라는 점을 인식했습니다. 통학 시간 동안 1시간씩 반복 플레이하며 목표 유저 관점에서 재미 요소를 점검·개선했고, 유저 경험을 기준으로 시스템을 설계하는 관점을 갖게 되었습니다.
