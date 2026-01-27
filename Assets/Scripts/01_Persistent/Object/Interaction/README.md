## Detected / Interaction

플레이어가 다른 대상과 상호작용을 할 수 있게 하는 기능을 제공하며, 상호작용 대상 클래스서 DetectionTarget로 거리에 따른 인터페이스를
제공하며 플레이어가 상호작용 했을 때 HandleInteractionBase를 이용하여 특정 로직을 실행시켜줍니다.

- **DetectionTarget**  
  플레이어와의 거리 기반 감지를 통해 상호작용 가능 상태를 관리하고, UI 표시와 HandleInteractionBase 실행을 중계하는 상호작용 진입 포인트
