## Item System

아이템을 **데이터(ItemData)와 런타임 객체(ItemBase)**로 분리하고,  
장비·소비·부착물·재료를 공통 구조로 확장한 데이터 중심 아이템 시스템입니다.

- **FItemShell / FItemShellIncludeIndex**  
  아이템 ID·개수·내구도·부착물·탄환 정보를 직렬화하여 인벤토리·창고·세이브에 사용되는 저장 단위

- **ItemBase**  
  모든 아이템의 런타임 베이스 클래스로, 드랍·획득·빌보드·상호작용·스토리지 연동을 담당

- **ItemData (ScriptableObject)**  
  아이템의 고정 데이터(ID·이름·가격·무게·등급·스탯)를 정의하는 데이터 베이스 클래스

- **EquipData : ItemData**  
  무기·방어구·가방 등 장비 아이템의 내구도·감소율을 확장 정의한 장비 데이터

- **ConsumData : ItemData**  
  힐·음식·탄환 등 소비 아이템의 타입·개수/용량·최대 보관 수를 정의한 소비 데이터

- **ItemVisualData (ScriptableObject)**  
  아이템 아이콘 등 시각 정보를 분리 관리하는 비주얼 데이터

- **EItemID**  
  무기·장비·부착물·소비·재료를 범위 기반으로 분류한 전체 아이템 식별자 열거형

- **EItemType / EItemGrade / EConsumableType 등**  
  아이템 분류·등급·소비 타입을 명확히 구분하기 위한 공통 열거형 집합

### 구조 특징
- ItemData ↔ ItemBase 분리로 **데이터 중심 설계**
- 상속을 통해 장비·소비·부착물 기능을 **점진적으로 확장**
- FItemShell 기반으로 **세이브/로드·인벤토리·상호작용을 단일 포맷으로 통합**
