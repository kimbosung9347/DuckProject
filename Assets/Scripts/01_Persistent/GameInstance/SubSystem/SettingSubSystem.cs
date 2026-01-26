using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using Unity.VisualScripting;

public class SettingSubSystem : GameInstanceSubSystem
{
    private SettingData cloneSettingData;

    public override void Init()
    {
        // cloneSettingData = GameInstance.Instance.SAVE_CloneSettingData();
        // ProcessAllSetting();
    } 
     
    public override void LevelStart(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Mainmenu:
            {

            }  
            break;

            case ELevelType.Loading:
            {
                     
            }
            break;
        }
    }
    public override void LevelEnd(ELevelType _type)
    { 
         
    } 
     
    public void ChangeResolution(EResolutionType _type)
    {
        if (cloneSettingData.resolutionType == _type)
            return;
         
        cloneSettingData.resolutionType = _type;
        ProcessResolution(); 
    }
    public void ChangeScreenMode(EScreenModeType _type)
    {
        if (cloneSettingData.screenModeType == _type)
            return;
         
        cloneSettingData.screenModeType = _type;
        ProcessScreenMode();
    }
    public void ChangeLanguage(ELanguageType _type)
    {
        if (cloneSettingData.eLanguageType == _type)
            return;

        cloneSettingData.eLanguageType = _type;
        ProcessLanguage();
    }
    public void ChangeVolume(float _volume)
    { 
        cloneSettingData.fullVolume = _volume;
        ProcessVolume();
    }
    public SettingData GetSettingData()
    {
        return cloneSettingData;
    }

    public void SaveCurSetting()
    {
        GameInstance.Instance.SAVE_SettingData(cloneSettingData);
    }
    public void CancleCurSetting()
    {
        var original = GameInstance.Instance.SAVE_GetSettingData();

        ChangeResolution(original.resolutionType);
        ChangeScreenMode(original.screenModeType);
        ChangeLanguage(original.eLanguageType);
        ChangeVolume(original.fullVolume);
    } 


    /* Process */
    private void ProcessAllSetting()
    {
        ProcessResolution();
        ProcessScreenMode();
        ProcessLanguage();
        ProcessVolume();
    }

    private void ProcessResolution()
    {
        switch (cloneSettingData.resolutionType)
        {
            case EResolutionType.e1920_1080:
            {
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
            } 
            break;

            case EResolutionType.e1280_720:
            {
                Screen.SetResolution(1280, 720, Screen.fullScreenMode);
            } 
            break;
        }
    }

    private void ProcessScreenMode()
    {
        switch (cloneSettingData.screenModeType)
        {
            case EScreenModeType.Window:
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                break;

            case EScreenModeType.FullScreenWindow:  
                {  
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
                break;

            case EScreenModeType.FullScreen:
                {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                break;

            default:
                {
                    Debug.LogWarning($"[SettingSubSystem] Unknown ScreenModeType: {cloneSettingData.screenModeType}");
                } 
                break;
        }

        // 현재 해상도 유지하면서 모드만 바꾸기
        Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode);
        Debug.Log($"[SettingSubSystem] Screen Mode Applied: {Screen.fullScreenMode}");
    }

    private void ProcessLanguage()
    {
        // 현재 LocaleSettings에서 사용 중인 Locale과 목표 Locale이 같으면 return
        var selectedType = cloneSettingData.eLanguageType;
         
        // LocalizationSettings가 아직 로드 안 됐을 수 있으므로 방어
        if (LocalizationSettings.Instance == null)
        {
            Debug.LogWarning("[SettingSubSystem] LocalizationSettings not initialized yet.");
            return;
        }

        Locale targetLocale = null;

        switch (selectedType)
        {
            case ELanguageType.English:
                targetLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
                break;

            case ELanguageType.Korean:
                targetLocale = LocalizationSettings.AvailableLocales.GetLocale("ko");
                break;
        }

        if (targetLocale == null)
        {
            Debug.LogWarning($"[SettingSubSystem] Locale not found for {selectedType}");
            return;
        }
         
        // 에디터에서 Locale 드롭다운 바꾼 것처럼 즉시 언어 교체
        LocalizationSettings.SelectedLocale = targetLocale;
        Debug.Log($"[SettingSubSystem] Language changed → {targetLocale.LocaleName}");
    }

    private void ProcessVolume()
    {
        // AudioListener.volume = cloneSettingData.fullVolume / 100f;
        Debug.Log($"[SettingSubSystem] Volume Applied: {cloneSettingData.fullVolume}%");
    }
}