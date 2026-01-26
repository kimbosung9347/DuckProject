using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingLocalizeDropDown : MonoBehaviour
{  
    [Header("Localization Table")] 
    [SerializeField] private string tableName;     
    [SerializeField] private List<string> localizeKeys;         
     
    private TMP_Dropdown cachedDropdown;
    private void Awake()
    {
        cachedDropdown = GetComponentInChildren<TMP_Dropdown>();
        MakeOption();
    } 
    private void Start()
    {
        RenewLocalizedTexts();
    }
      
    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }
    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
    private void OnLocaleChanged(Locale locale)
    {
        RenewLocalizedTexts();
    }
     
    private void MakeOption()
    {
        if (string.IsNullOrEmpty(tableName))
        {
            Debug.LogWarning($"[LocalizeDropdown] tableName이 비어있습니다. {gameObject.name}");
            return;
        }
        if (localizeKeys == null || localizeKeys.Count == 0)
        {
            Debug.LogWarning($"[LocalizeDropdown] localizeKeys가 비어있습니다. {gameObject.name}");
            return;
        }
         
        cachedDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        foreach (string key in localizeKeys)
        {
            string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
            options.Add(new TMP_Dropdown.OptionData(localizedText));
        }
        cachedDropdown.AddOptions(options);
    }
    private void RenewLocalizedTexts()
    {
        if (cachedDropdown == null)
            return;
         
        for (int i = 0; i < localizeKeys.Count; i++)
        {
            string key = localizeKeys[i];
            string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
            cachedDropdown.options[i].text = localizedText;
        }
        cachedDropdown.RefreshShownValue();
    }
}
