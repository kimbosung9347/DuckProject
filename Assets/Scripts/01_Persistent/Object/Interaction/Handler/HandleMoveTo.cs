using UnityEngine;

// MoveType
enum EHandleMoveType
{ 
    ToOtherLevel,
    ToPos,
    End,
} 

public class HandleMoveTo : HandleInteractionBase
{
    [SerializeField] private EHandleMoveType moveType;
    [SerializeField] private ELoadingScreenType loadingScreenType; 
    [SerializeField] private ELevelType moveLevel = ELevelType.End;
    [SerializeField] private Transform moveTransform;

    protected override void Awake() 
    {
        base.Awake(); 
        interactionType = EInteractionType.MoveTo;
    } 
    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        // 해당 레벨로 전환 시켜줘야함
        switch (moveType)
        { 
            case EHandleMoveType.ToOtherLevel:
            {
                if (moveLevel == ELevelType.End)
                    return;
                 
                _playerInteraction.EnterMoveToOtherLevel(moveLevel, loadingScreenType);
            } 
            break; 

            case EHandleMoveType.ToPos:
            {
            }
            break;
        }
    }
    public override void EndInteractionToPlayer()
    {

    }
}
