using TMPro;
using UnityEngine;

public class DeathReasonText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Header("Colors")]
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color monsterColor = Color.red;
    [SerializeField] private Color weaponColor = Color.yellow;

    [Header("Layout")]
    [SerializeField] private float titleOffset = 8f;
    [SerializeField] private float reasonOffset = -6f;
    [SerializeField] private float titleSize = 100f;
    [SerializeField] private float reasonSize = 80f;

    [Header("Editor Preview")]
    [SerializeField] private string previewMonsterName = "복서";
    [SerializeField] private string previewWeaponName = "산탄총";
     
    public void SetText(string monsterName, string weaponName)
    {
        if (!text) return;

        string baseHex = ColorUtility.ToHtmlStringRGBA(baseColor);
        string monsterHex = ColorUtility.ToHtmlStringRGBA(monsterColor);
        string weaponHex = ColorUtility.ToHtmlStringRGBA(weaponColor);

        text.richText = true; 
        text.text =
            $"<voffset={titleOffset}><size={titleSize}%><color=#{baseHex}>이것 때문에 죽었어</color></size></voffset>  " +
            $"<voffset={reasonOffset}><size={reasonSize}%>" + 
            $"<color=#{monsterHex}>{{{monsterName}</color>" + 
            $"<color=#{baseHex}> : </color>" +
            $"<color=#{weaponHex}>{weaponName}}}</color>" +
            $"</size></voffset>";
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetText(previewMonsterName, previewWeaponName);
    }
#endif
}
