using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class CinemachineTriggerZone : MonoBehaviour
{
    [SerializeField] private CinemachineCamera questCamera;
     
    private void Awake()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;

        if (questCamera)
            questCamera.Priority = DuckDefine.DISABLE_CINEMACHIN_PRIORITY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!questCamera)
            return; 

        questCamera.Priority = DuckDefine.INTERACTION_CINEMACHIN_PRIORITY;
    } 

    private void OnTriggerExit(Collider other)
    {
        if (!questCamera)
            return;

        questCamera.Priority = DuckDefine.DISABLE_CINEMACHIN_PRIORITY;
    }
}
