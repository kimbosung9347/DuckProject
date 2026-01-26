## Interaction / Handle System

플레이어와 월드 오브젝트 간의 모든 상호작용을  
`DetectionTarget → HandleInteractionBase → PlayerInteraction` 흐름으로 통합 처리하는 상호작용 시스템입니다.

- **HandleInteractionBase 기반 구조**  
  모든 상호작용 대상은 공통 인터페이스를 상속받아 동일한 감지·실행·종료 흐름으로 처리됩니다.
