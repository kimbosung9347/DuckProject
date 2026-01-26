using Unity.Cinemachine;
using UnityEngine;

public class PlayerAttack : DuckAttack
{
    private CinemachineImpulseSource cachedCineImpulse;
    private PlayerAiming cachedPlayAiming; // todo 분리
    private PlayerUIController cachedUIController;
     
    protected override void Awake()
    {
        base.Awake();
         
        cachedPlayAiming = GetComponent<PlayerAiming>();
        cachedAiming = GetComponent<PlayerAiming>();
        cachedUIController = GetComponent<PlayerUIController>(); 
        cachedCineImpulse = GetComponent<CinemachineImpulseSource>();
    }

    public void ActionMouseHit(ECursorHitState _state)
    {  
        cachedUIController.CURSOR_ActiveHit(_state);
    }  

    protected override FFireInfo GetShootInfo()
    {
        FFireInfo info = base.GetShootInfo();
        return info; 
    }  
    protected override void ActiveShotEffect(Vector3 dir, float _size)
    {
        base.ActiveShotEffect(dir, _size);
         
        // 조준 반동   
        {
            float moveSize = cachedBattleTable.Calculate_CursorMoveSize(_size);
            cachedPlayAiming.MoveCursorToDir(moveSize);
        }  

        // 카메라 반동
        {
            cachedCineImpulse.GenerateImpulse(dir * cachedBattleTable.Calculate_CameraRecoilStrengh(_size));
        }

        // 총기의 공격했다는 걸 전달해줘야함 
        // Map에 전달해야함
        GameInstance.Instance.MAP_ShotInPos(transform.position);
    }  
}
