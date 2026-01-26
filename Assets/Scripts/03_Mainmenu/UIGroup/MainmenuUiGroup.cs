using UnityEngine;
using UnityEngine.InputSystem;

public class MainmenuUiGroup : MonoBehaviour
{
    [SerializeField] private LogoCanvas logoCanvas;
    [SerializeField] private LobbyCanvas lobbyCanvas;
    [SerializeField] private SaveCanvas saveCanvas;


    public LogoCanvas GetLogoCanvas() { return logoCanvas; }
    public LobbyCanvas GetLobbyCanvas() { return lobbyCanvas; }
    public SaveCanvas GetSaveCanvas() { return saveCanvas; }
     
     
}
