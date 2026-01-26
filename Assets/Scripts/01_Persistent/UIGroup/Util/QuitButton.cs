using UnityEngine.UI;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(Quit);
        btn.onClick.AddListener(Quit);
    }

    private void Quit()
    { 
        GetComponent<UIAnimation>()?.Action_Animation();
        GameInstance.Instance.PLAYER_GetPlayerController().ChangePlay();
    }
} 
