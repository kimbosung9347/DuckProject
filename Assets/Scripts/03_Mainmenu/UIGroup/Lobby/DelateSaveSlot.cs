using UnityEngine;
using UnityEngine.UI;
 
public class DelateSaveSlot : MonoBehaviour
{
    [SerializeField] float limitTime = 3f;

    private LobbyCanvas cachedLobbyCanvas;
    private Material cachedMaterial;
    private float progress = 0f;
    private bool bActive = false;

    void Start()
    {
        var image = GetComponent<Image>();
        cachedMaterial = image.material; 
    }
     
    void Update()
    {
        if (!bActive)
            return; 

        CalculateProgress();
        RenewProgress(progress);
        CheckProgressAndDelate();
    }

    public void Active(bool _active)
    {
        if (bActive == _active)
            return;

        bool wasActive = bActive;
        bActive = _active;

        if (!wasActive && bActive)
        {
            // 켜질 때 초기화
            progress = 0f;
            RenewProgress(0f);
        }
        else if (wasActive && !bActive)
        {
            // 꺼질 때 정리
            Clear();
        }
    }
    public void CacheLobbyCanvas(LobbyCanvas _canvas)
    {
        cachedLobbyCanvas = _canvas; 
    }

    private void RenewProgress(float _progress)
    {
        cachedMaterial.SetFloat("_Progress", _progress);
    }
    private void CalculateProgress()
    {
        progress += Time.deltaTime / limitTime;
        progress = Mathf.Clamp01(progress);
    }
    private void CheckProgressAndDelate()
    {
        if (progress >= 1f)
        {
            Active(false);
            DelateSave();
        }
    }  

    private void DelateSave()
    {
        var lobbyData = GameInstance.Instance.SAVE_GetLobbyData();
        if (lobbyData.selectedSaveSlotIndex == -1)
            return;
         
        GameInstance.Instance.SAVE_DelateSaveSlot(lobbyData.selectedSaveSlotIndex);
        cachedLobbyCanvas.DelateSaveSlot();
    } 

    private void Clear()
    {
        progress = 0f;
        RenewProgress(0f);
    }


}
