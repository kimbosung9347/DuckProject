using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    PlayerSight cachedPlayerSight;
    PlayerState cachedState;
    PlayerStat cachedStat; 

    private void Awake()
    {
        cachedPlayerSight = GetComponent<PlayerSight>();
        cachedState = GetComponent<PlayerState>();
        cachedStat = GetComponent<PlayerStat>(); 
    }  

    public void SpawnInPos(Transform _tp)   
    {
        if (!_tp)
            return;

        // Tp
        if (_tp) 
        {
            transform.position = _tp.position;
            transform.rotation = _tp.rotation;
        } 

        var instance = GameInstance.Instance;

        // 카메라 Focus
        instance.CAMERA_MoveCameraFocus(transform.position);
        GetComponent<PlayerController>().ChangePlay();
    }   
    public void SpawnInHouse(Transform _tp) 
    {
        cachedPlayerSight.ActiveDarkFov(false);
        SpawnInPos(_tp);
        cachedState.ChangeState(EDuckState.Default);
        cachedStat.InitHP(); 

        FRadiuseCollaspeInfo info = new FRadiuseCollaspeInfo();
        info.startPosA = new Vector2(0.5f, 0.5f);
        info.radType = ERadiuseCollaspeType.Bigger_Clear;
        GetComponent<PlayerUIController>().ActiveCollaspeCanvas(info);

    }
    public void SpawnInFarm(Transform _tp)
    {
        cachedPlayerSight.ActiveDarkFov(true);
        SpawnInPos(_tp);

        var ui = GetComponent<PlayerUIController>();
        ui.InstantActiveDisableCanvas();
        StartCoroutine(DelayDisable(ui));
    }

    private IEnumerator DelayDisable(PlayerUIController ui)
    { 
        yield return new WaitForSecondsRealtime(0.5f); 
        ui.InstantDisableDisableCanvas();
        FRadiuseCollaspeInfo info = new FRadiuseCollaspeInfo();
        info.startPosA = new Vector2(0.5f, 0.5f); 
        info.radType = ERadiuseCollaspeType.Bigger_Clear;
        GetComponent<PlayerUIController>().ActiveCollaspeCanvas(info);
    } 
}  
      