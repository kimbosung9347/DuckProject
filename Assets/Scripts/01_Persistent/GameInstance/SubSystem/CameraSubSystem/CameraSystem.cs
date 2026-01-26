using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public enum EDuckCameraType
{
    BillboardCam,
} 
public enum EPlayCameraType
{
    TopView,
} 

public class CameraSystem : MonoBehaviour 
{ 
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera billboardCamera;
    [SerializeField] private float zoomDuration = 0.2f;
    [SerializeField] private CinemachineCamera topView;
     
    private Transform cameraFocusTP;
    private Coroutine zoomRoutine;
    private CinemachinePositionComposer topViewPosComposer;

    private void Awake() 
    {
        topViewPosComposer = topView.GetComponent<CinemachinePositionComposer>();
        topViewPosComposer.CameraDistance  = DuckDefine.DEFAULT_ZOOM_CAMERA_DISTANCE;
    }    
    public void DisableMainCamera()
    {
        mainCamera.gameObject.SetActive(false);
    }
    public void ActiveMainCamera()
    {
        mainCamera.gameObject.SetActive(true);
    } 

    public Camera GetMainCamera()
    {
        return mainCamera;
    } 


    // 
    public void SetCameraFocus(Transform _tp)
    {
        cameraFocusTP = _tp;
        topView.Target.TrackingTarget = cameraFocusTP;
    } 
    public void ActievMainCamera(EDuckCameraType type, bool isActive)
    {
        switch (type) 
        {
            case EDuckCameraType.BillboardCam:
            {
                if (billboardCamera != null)
                    billboardCamera.gameObject.SetActive(isActive);
            }
            break;
        } 
    }
    public void ActiveCinemachinCamera(EPlayCameraType _type)
    {
        // 전부 내리기
        topView.Priority = DuckDefine.DISABLE_CINEMACHIN_PRIORITY;
           
        switch (_type)
        {
            case EPlayCameraType.TopView:
            {
                topView.Priority = DuckDefine.NORMAL_CINEMACHIN_PRIORITY;
            } 
            break;
        }
    }   
    public void SetTopViewCameraDistance(float distance)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);
         
        zoomRoutine = StartCoroutine(TopView_LerpDistance(distance));
    } 
    public void MoveCameraFocuse(Vector3 _pos)
    {
        cameraFocusTP.position = _pos; 
    } 
    public void ClearColor()
    {
        var cam = Camera.main;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black; 
    } 

    private IEnumerator TopView_LerpDistance(float target)
    {
        float start = topViewPosComposer.CameraDistance;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            topViewPosComposer.CameraDistance = Mathf.Lerp(start, target, t);
            yield return null;
        }

        topViewPosComposer.CameraDistance = target;
        zoomRoutine = null;
    }
} 
  