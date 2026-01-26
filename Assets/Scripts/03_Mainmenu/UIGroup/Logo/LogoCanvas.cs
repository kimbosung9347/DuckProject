using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogoCanvas : MonoBehaviour
{
    [SerializeField] private MainmenuUiGroup cachedMainmenu;
    [SerializeField] private Button changeMenuButton;
    [SerializeField] private TextMeshProUGUI textTouch;
    [SerializeField] private LogoQuad logoQuad;

    [SerializeField] private float delay = 0.2f;
    [SerializeField] private float fadeTime = 0.8f;

    private Coroutine fadeCo;

    private void Awake()
    {
        var gameInstance = GameInstance.Instance;
          
        var cursor = gameInstance.UI_GetPersistentUIGroup().GetCursorCanvas();
        cursor.GetUICursor().gameObject.SetActive(true);
        cursor.GetPlayCursor().gameObject.SetActive(false);
           
        BindStartTapButton();
        Active();
    } 

    public void Active()
    { 
        // 초기 알파 0
        SetTextAlpha(0f);

        logoQuad.Active(ELogoState.Appear);

        if (fadeCo != null)
            StopCoroutine(fadeCo);

        fadeCo = StartCoroutine(FadeInText());
    }

    private IEnumerator FadeInText()
    {
        yield return new WaitForSeconds(delay);

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetTextAlpha(Mathf.Clamp01(t / fadeTime));
            yield return null;
        }

        SetTextAlpha(1f);
    }

    private void SetTextAlpha(float a)
    {
        var c = textTouch.color;
        c.a = a;
        textTouch.color = c;
    }

    private void PressChangeLobbyButton()
    {
        logoQuad.gameObject.SetActive(false);
        gameObject.SetActive(false); 
        cachedMainmenu.GetLobbyCanvas().Active();
        GameInstance.Instance.SOUND_PlaySoundSfx(ESoundSfxType.KeyLock, Vector3.zero);
    }  
     
    private void BindStartTapButton()
    {
        changeMenuButton.onClick.RemoveAllListeners();
        changeMenuButton.onClick.AddListener(PressChangeLobbyButton);
    }
}
