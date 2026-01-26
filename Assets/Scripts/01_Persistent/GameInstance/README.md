## GameInstance
GameInstance는 본 프로젝트의 **중앙 허브 역할**을 담당하는 클래스입니다.  
게임 실행 시 가장 먼저 초기화되며, 모든 주요 시스템(SubSystem)을 생성·관리합니다.

---

### 역할

- 게임 전반에서 사용되는 **전역 Singleton**
- 모든 핵심 시스템(Table, Save, Player, UI, Camera 등)을 **서브시스템 단위로 관리**
- 씬 로드/언로드 시점에 각 시스템에 이벤트 전달
- 다른 객체들이 시스템에 직접 접근하지 않도록 **단일 접근 창구(API)** 제공

---

### 초기화 흐름

1. `Awake()` 시점에서 Singleton 인스턴스 생성
2. 모든 `GameInstanceSubSystem` 인스턴스 생성
3. **순서 의존성이 있는 시스템을 고려해 명시적인 초기화 순서 보장**
		- Table > Save > ...  > Spawn >  map > player
4. 모든 서브시스템의 `Init()` 호출
5. `SceneManager.sceneLoaded / sceneUnloaded` 이벤트 등록

---

### 설계 의도

- Unity의 MonoBehaviour 기반 매니저 난립을 방지
- 시스템 간 직접 참조를 제거하고 **중앙 집중형 제어 구조** 유지

---

### 접근 방식

모든 외부 접근은 `GameInstance.Instance`를 통해 이루어지며,  
각 시스템별로 명확한 네이밍의 API를 제공합니다.

예:
- `TABLE_GetBattleTable()`
- `PLAYER_GetPlayerStat()`
- `SAVE_GetCurPlayData()`
- `SPAWN_MakeItem()`
- `CAMERA_SetCameraFocus()`

---

### 요약

GameInstance는 본 프로젝트의 전체 아키텍처를 관통하는 핵심 클래스이며,  
모든 시스템의 생성·수명·접근을 통제하는 **게임의 중심 관리자**입니다.
