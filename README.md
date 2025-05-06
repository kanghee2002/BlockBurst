<img width="400" alt="Block_Block_썸네일" src="https://github.com/user-attachments/assets/864577f1-99dd-49d4-b0dc-163845cee59d" />
<p align="left">
  <img src="https://github.com/user-attachments/assets/869dddd1-cbb9-462a-b5bd-987ef76b9f7a" width="30%" alt="GIF1" />
  <img src="https://github.com/user-attachments/assets/baaa4e5c-7d96-40d7-b781-3b7325b72e5c" width="30%" alt="GIF2" />
</p>

## 작업 규칙
- FileName, ClassName, PublicMethod, fieldAndVariable
- private 필드, 매서드 자유롭게 선언
- spec의 구조를 변경해야 할 경우 디스코드
- 본인이 작업한 것은 본인이 merge
- 이슈 있으면 디코 -> 바로 처리 or 모아서 회의
---


## 덱 빌딩 로그라이크 퍼즐
- Unity, Mobile
- 팀 프로젝트 (6인)
- 플레이 스토어 출시를 목표로 현재 개발 진행 중
- **장려상** (2025 NDM), **IP Power 부문 우수상 수상** (2025 고연전 GameDev)

## 주요 기능
### 게임 데이터 계층화
- 이유
    - 효율적인 데이터 관리를 위해 게임의 데이터를 3단계로 계층화
- 방법
    - 전체 게임 데이터, **GameData**
    - 한 번의 진행 정보 데이터, **RunData**
    - 한 번의 스테이지 데이터, **BlockGameData**
- 효과
    - **높은 확장성과 가독성** 확보
    - 베타 테스트 이후 고안된 컨텐츠인 덱 선택, 해금 시스템도 수월하게 구조화 가능
### MVC 패턴과 단일 책임 원칙
- 이유
    - UI 중심적으로 플레이가 진행되므로 각자의 역할을 철저히 분리할 필요
- 방법
    - **Model**. GameManager
    - **View**. UI
    - **Controller**. GameUIManager
    - 이외 ShopManager, StageManager, EffectManager, TutorialManager 등 분리
- 효과
    - 많은 양의 UI 입력과 출력을 **일률적으로 처리 가능**
    - 한 클래스가 하나의 책임만 맡게 하여 **유지보수성 향상**
### 효과 적용 구조 단일화
- 이유
    - 아이템의 효과, 스테이지 제한의 효과, 특수 블록의 효과 등 모든 효과들이 사실상 **하나의 방법으로 적용 가능**
- 방법
    - Scriptable Object인 EffectData 생성
    - 아이템은 ItemData에, 스테이지 제한은 StageData에, 특수 블록은 BlockData에 EffectData를 
    저장
    - 각 효과를 얻을 때 미리 EffectManager에 등록
    - 이후 효과가 발동될 때, 모두 EffectManager의 TriggerEffects()를 통해 발동
- 효과
    - EffectData를 확장하여 ‘n번 마다 발동’, ‘효과 계수 증가’ 등의 **추가적인 기능도 효율적으로 개발** 가능했음
    - 효과의 적용이 모두 단일하게 적용되기 때문에 **동적인 효과 적용에는 어려움**이 있었음. 보완 필요
### 기타
- 튜토리얼 전용 클래스를 활용한 하드코딩 완화
- 아이템, 스테이지, 효과, 블록을 Scriptable Object로 최적화
- DoTween과 Coroutine을 이용한 자연스러운 애니메이션 처리
- Dall-E를 이용한 이미지 리소스
