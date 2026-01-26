## House Components

House 영역에서 플레이어·AI 진입에 따른 가시성 처리와 상호작용 트리거를 관리하는 환경 제어 컴포넌트 집합입니다.

- **DuckHouse**  
  House 내부 상태를 중앙에서 관리하며 지붕 숨김·프롭 가시성 등 하위 트리거 요청을 통합 처리하는 하우스 컨트롤러

- **HouseTriggerPart**  
  House 진입/이탈 시 트리거 타입에 따라 DuckHouse에 이벤트를 전달하는 영역 감지 컴포넌트

- **HouseHideRoof**  
  House 지붕 렌더러를 캐싱하고 플레이어 진입 여부에 따라 지붕 가시성을 제어하는 지붕 숨김 전용 컴포넌트
