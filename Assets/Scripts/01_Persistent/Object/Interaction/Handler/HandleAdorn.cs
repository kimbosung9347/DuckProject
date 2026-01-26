using Unity.Cinemachine;
using UnityEngine;

public class HandleAdorn : HandleInteractionBase
{
    [SerializeField] private CinemachineCamera adornCamera;
     
    protected override void Awake()
    {
        interactionType = EInteractionType.Adorn;
    }   
    public override void DoInteractionToPlayer(PlayerInteraction _playerInteraction)
    {
        _playerInteraction.EnterAdorn();
        adornCamera.Priority = 40;
    } 
    public override void EndInteractionToPlayer()
    {
        adornCamera.Priority = 0;
    }
}   
