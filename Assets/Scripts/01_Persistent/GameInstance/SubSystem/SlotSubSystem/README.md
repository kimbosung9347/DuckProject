# SlotController 시스템

## 개요
SlotController는 게임 내 **모든 아이템 슬롯 입력과 상호작용을 중앙에서 관리하는 컨트롤러**입니다.  
Equip, Backpack, ItemBox, Store, QuickSlot, Attach, WareHouse 등 서로 다른 슬롯 간의 **드래그·클릭·우클릭·즉시 행동**을 단일 규칙 체계로 처리하도록 설계되었습니다.

## 설계 목적
- 슬롯별 로직 분산으로 인한 규칙 중복 방지
- UI 입력과 실제 아이템 이동/교환 로직 분리
- 슬롯 타입이 늘어나도 상호작용 규칙을 한 곳에서 관리

## 구조 구성
- **SlotController**
  - 슬롯 상태 관리 (Hover / Drag / LeftSelect / RightSelect)
  - 입력 이벤트의 진입점
  - 상황에 맞는 처리 클래스로 위임

- **SlotHandle**
  - 슬롯 간 실제 상호작용 로직 담당
  - 슬롯 타입 조합(Equip↔Backpack, Backpack↔Store 등)에 따른 규칙 처리
  - 아이템 이동, 교환, 장착, 부착, 구매/판매, 스택 병합 처리

- **SlotCheck**
  - 상호작용 가능 여부 판단
  - 금액, 무게, 인벤토리 여유 슬롯, 상점 활성 상태 등 사전 검증

- **SlotCursor**
  - 커서 UI 및 툴팁, 우클릭 선택 메뉴 관리
  - 슬롯 타입 및 아이템 타입에 따른 사용 키 가이드 표시

## 핵심 특징
- **중앙 집중형 규칙 관리**
  - 모든 슬롯 조합을 `BetweenSlotType` 기준으로 명시적으로 정의
  - 예외 규칙(가방 해제 시 슬롯 제한, 퀵슬롯 추적 유지 등)을 한 곳에서 처리

- **입력과 로직 분리**
  - UI 슬롯은 자신의 타입/인덱스/아이템 ID만 전달
  - 실제 판단과 처리 흐름은 SlotController 계층에서만 수행

- **확장 고려**
  - 슬롯 타입 추가 시 UI 변경 없이 규칙만 확장 가능
  - QuickSlot, Attach, WareHouse 같은 특수 슬롯도 동일한 흐름으로 통합

## 사용 방식 요약
- Hover / Drag / Click 입력 → SlotController
- 가능 여부 판단 → SlotCheck
- 실제 아이템 처리 → SlotHandle
- UI 후처리 → SlotCursor / 각 Canvas
