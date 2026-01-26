using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

enum ESettingDomainType
{
    General,
    Audio,
    End,
}

public class SettingCanvas : MonoBehaviour
{
    [Header("Domain Panels")]
    [SerializeField] private GameObject generalDomainObject;
    [SerializeField] private GameObject audioDomainObject;
    [SerializeField] private Color domainActiveColor;
    [SerializeField] private Color domainDisableColor;

    [Header("Buttons")]
    [SerializeField] private Button generalButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancleButton;

    [Header("General Domain")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("Audio Domain")]
    [SerializeField] private Slider volumeSlider;

    private Image[] domainImages;
    private GameObject[] domainObjects;

    private LobbyCanvas lobbyCanvas;
    private StopCanvas stopCanvas;

    private SettingData settingData;   // 실제 적용 데이터
    private SettingData workingCopy;   // UI 임시 데이터

    private void Awake()
    {
        settingData = GameInstance.Instance.SAVE_GetSettingData();
        workingCopy = Clone(settingData);

        domainObjects = new GameObject[(int)ESettingDomainType.End];
        domainObjects[(int)ESettingDomainType.General] = generalDomainObject;
        domainObjects[(int)ESettingDomainType.Audio] = audioDomainObject;

        domainImages = new Image[(int)ESettingDomainType.End];
        domainImages[(int)ESettingDomainType.General] = generalButton.GetComponent<Image>();
        domainImages[(int)ESettingDomainType.Audio] = audioButton.GetComponent<Image>();

        InitUI();
        BindAll();
        ApplyRuntime(settingData);
         
        gameObject.SetActive(false);
    }

    private void Start()
    { 
        // 게임 시작 시 저장된 설정 1회 적용

    }

    /* ==============================
        Init
    ============================== */
    private void InitUI()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(new List<string>
        {
            "1280 x 720",
            "1920 x 1080",
            "2560 x 1440",
        });

        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new List<string>
        {
            "Window",
            "Borderless",
            "Fullscreen"
        });

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>
        {
            "English",
            "Korean"
        });

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 100f;
    }

    /* ==============================
        Active / Disable
    ============================== */
    public void Active(LobbyCanvas _lobbyCanvas)
    {
        lobbyCanvas = _lobbyCanvas;
        Open();
    }

    public void Active(StopCanvas _stopCanvas)
    {
        stopCanvas = _stopCanvas;
        Open();
    }

    public void Disable()
    {
        lobbyCanvas = null;
        stopCanvas = null;
        gameObject.SetActive(false);
    }

    /* ==============================
        UI → WorkingCopy (미리보기용)
    ============================== */
    public void PressChangeResoultion(int index)
    {
        workingCopy.resolutionType = (EResolutionType)index;
    }

    public void PressChangeScreenMode(int index)
    {
        workingCopy.screenModeType = (EScreenModeType)index;
    }

    public void PressChangeLanguage(int index)
    {
        workingCopy.eLanguageType = (ELanguageType)index;
        ApplyLanguage(workingCopy.eLanguageType); // 언어 즉시 반영
    }

    public void PressChangeFullVolume(float volume)
    {
        workingCopy.fullVolume = volume;
        AudioListener.volume = volume / 100f; // 미리보기
    }

    /* ==============================
        Confirm / Cancel
    ============================== */
    private void PressConfirmButton()
    {
        Copy(workingCopy);           // 메모리 + 파일 저장
        ApplyRuntime(settingData);   // 여기서만 실제 적용

        if (lobbyCanvas) lobbyCanvas.Active();
        else if (stopCanvas) stopCanvas.Active();

        Disable();
    }

    private void PressCancleButton()
    {
        workingCopy = Clone(settingData);
        ApplyWorkingCopyToUI();
        AudioListener.volume = settingData.fullVolume / 100f;

        if (lobbyCanvas) lobbyCanvas.Active();
        else if (stopCanvas) stopCanvas.Active();

        Disable();
    }

    /* ==============================
        Runtime Apply (Confirm 전용)
    ============================== */
    private void ApplyRuntime(SettingData data)
    {
        ApplyResolutionAndScreenMode(
            data.resolutionType,
            data.screenModeType
        );

        ApplyLanguage(data.eLanguageType);
        AudioListener.volume = data.fullVolume / 100f;
    }

    /// <summary>
    /// 해상도 + 화면모드 통합 적용 (Unity 내부 덮어쓰기 방지)
    /// </summary>
    private void ApplyResolutionAndScreenMode(
        EResolutionType resType,
        EScreenModeType modeType)
    {
        int width = 1920;
        int height = 1080;

        switch (resType)
        {
            case EResolutionType.e1280_720:
                width = 1280; height = 720;
                break;
            case EResolutionType.e1920_1080:
                width = 1920; height = 1080;
                break;
            case EResolutionType.e2560_1440:
                width = 2560; height = 1440;
                break;
        }

        FullScreenMode mode = FullScreenMode.Windowed;

        switch (modeType)
        {
            case EScreenModeType.Window:
                mode = FullScreenMode.Windowed;
                break;
            case EScreenModeType.FullScreenWindow:
                mode = FullScreenMode.FullScreenWindow;
                break;
            case EScreenModeType.FullScreen:
                mode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        Screen.SetResolution(width, height, mode);
    }

    private void ApplyLanguage(ELanguageType type)
    {
        switch (type)
        {
            case ELanguageType.English:
                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale("en");
                break;
            case ELanguageType.Korean:
                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale("ko");
                break;
        }
    }

    /* ==============================
        UI Sync
    ============================== */
    private void ApplyWorkingCopyToUI()
    {
        resolutionDropdown.SetValueWithoutNotify((int)workingCopy.resolutionType);
        screenModeDropdown.SetValueWithoutNotify((int)workingCopy.screenModeType);
        languageDropdown.SetValueWithoutNotify((int)workingCopy.eLanguageType);
        volumeSlider.SetValueWithoutNotify(workingCopy.fullVolume);

        RefreshUIDisplayTexts();
    }

    private void RefreshUIDisplayTexts()
    {
        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();
        languageDropdown.RefreshShownValue();
    }

    private void ActiveDomain(ESettingDomainType type)
    {
        for (int i = 0; i < domainObjects.Length; ++i)
        {
            bool active = (i == (int)type);
            domainObjects[i].SetActive(active);
            domainImages[i].color = active ? domainActiveColor : domainDisableColor;
        }
    }

    /* ==============================
        Data
    ============================== */
    private SettingData Clone(SettingData src)
    {
        return new SettingData
        {
            resolutionType = src.resolutionType,
            screenModeType = src.screenModeType,
            eLanguageType = src.eLanguageType,
            fullVolume = src.fullVolume
        }; 
    }

    private void Copy(SettingData src)
    {
        SettingData data = GameInstance.Instance.SAVE_GetSettingData();
        data.Save(src);
    }

    private void Open()
    {
        workingCopy = Clone(settingData);
        ApplyWorkingCopyToUI();

        gameObject.SetActive(true);
        GetComponent<UIAnimation>()?.Action_Animation();
        ActiveDomain(ESettingDomainType.General);
    }

    private void BindAll()
    {
        generalButton.onClick.RemoveAllListeners();
        audioButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();
        cancleButton.onClick.RemoveAllListeners();

        generalButton.onClick.AddListener(() => ActiveDomain(ESettingDomainType.General));
        audioButton.onClick.AddListener(() => ActiveDomain(ESettingDomainType.Audio));
        confirmButton.onClick.AddListener(PressConfirmButton);
        cancleButton.onClick.AddListener(PressCancleButton);

        resolutionDropdown.onValueChanged.AddListener(PressChangeResoultion);
        screenModeDropdown.onValueChanged.AddListener(PressChangeScreenMode);
        languageDropdown.onValueChanged.AddListener(PressChangeLanguage);
        volumeSlider.onValueChanged.AddListener(PressChangeFullVolume);
    }
}
