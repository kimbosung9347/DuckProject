using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ESettingSubInfoType
{
    /* General */
    Resolution,
    ScreenMode,
    Language,
    
    /* Button */
    FullVolume, 
     
    End,
}

public class SettingRenewBySettingData : MonoBehaviour
{
    [SerializeField] private ESettingSubInfoType infoType;

    static private SettingData cachedSettingData;
    private GameInstance cachedGameInstance;
    private TMP_Dropdown cachedDropdown;
    private Slider cachedSlider; 
      
    private void Awake()
    {
        CacheGameInstanceAndSettingData();

        switch (infoType)
        {
            case ESettingSubInfoType.Resolution:
            case ESettingSubInfoType.ScreenMode: 
            case ESettingSubInfoType.Language:
            {
                cachedDropdown = GetComponentInChildren<TMP_Dropdown>();
            } 
            break;

            case ESettingSubInfoType.FullVolume:
            {
                cachedSlider = GetComponentInChildren<Slider>();
            }
            break;
        }
    }
    private void OnEnable()
    {
        RenewDropDown();
    } 
      
    public void RenewDropDown()
    { 
        // switch (infoType)
        // {
        //     case ESettingSubInfoType.Resolution:
        //     {
        //         RenewResolution();
        //     } 
        //     break;
        // 
        //     case ESettingSubInfoType.ScreenMode:
        //     {
        //         RenewScreenMode();
        //     }
        //     break;
        // 
        //     case ESettingSubInfoType.Language:
        //     {
        //         RenewLanguage();
        //     }
        //     break;
        //          
        //     case ESettingSubInfoType.FullVolume:
        //     {
        //         RenewFullVolume();
        //     } 
        //     break;
        // }
    }
    private void RenewResolution()
    {
        int index = (int)cachedSettingData.resolutionType;  
        cachedDropdown.value = Mathf.Clamp(index, 0, cachedDropdown.options.Count - 1);
    }
    private void RenewScreenMode()
    {
        int index = (int)cachedSettingData.screenModeType;
        cachedDropdown.value = Mathf.Clamp(index, 0, cachedDropdown.options.Count - 1);
    }
    private void RenewLanguage()
    {
        int index = (int)cachedSettingData.eLanguageType;
        cachedDropdown.value = Mathf.Clamp(index, 0, cachedDropdown.options.Count - 1);
    }
    private void RenewFullVolume()
    {
        float value = cachedSettingData.fullVolume;
        cachedSlider.value = value; 
    }
       
    private void CacheGameInstanceAndSettingData()
    {
        if (cachedSettingData == null)
        { 
            cachedGameInstance = GameInstance.Instance;
            if (cachedGameInstance)
            {
                var SettingData = cachedGameInstance.SETTING_GetSettingData();
                cachedSettingData = SettingData;
            }
        }
    }

}
