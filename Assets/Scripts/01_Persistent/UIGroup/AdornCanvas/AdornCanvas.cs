using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
enum EAdornButtonType
{ 
    Head,
    EyeBows,
    Eye,
    Beek,
    Arm,
    Foot,
    Body,
       
    End
}
enum EAdornColorType
{
    Black,
    White,
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Sodomy,
    Purple,
     
    End
}

public class AdornCanvas : MonoBehaviour
{ 
    /* Domain */
    [SerializeField] private Button headButton;
    [SerializeField] private Button eyeBowsButton;
    [SerializeField] private Button eyeButton;
    [SerializeField] private Button beekButton;
    [SerializeField] private Button armButton;
    [SerializeField] private Button footButton;
    [SerializeField] private Button bodyButton;
     
    [SerializeField] private Color disableColor = Color.black;
    [SerializeField] private Color activeColor = Color.black;
     
    /* Type */
    [SerializeField] private TextMeshProUGUI textMenu;
    [SerializeField] private TextMeshProUGUI textType;
    [SerializeField] private TextMeshProUGUI textTypeObject;
    [SerializeField] private Button leftArrowButton; 
    [SerializeField] private Button rightArrowButton;

    /* Color */
    [SerializeField] private Image curColor;
    [SerializeField] private Button blackButton;
    [SerializeField] private Button whiteButton;
    [SerializeField] private Button redButton;
    [SerializeField] private Button orangeButton;
    [SerializeField] private Button yellowButton;
    [SerializeField] private Button greenButton; 
    [SerializeField] private Button blueButton;
    [SerializeField] private Button sodomyButton;
    [SerializeField] private Button purpleButton;
     
    /* Check And Cancle */ 
    [SerializeField] private Button checkButton;
    [SerializeField] private Button cancleButton;
     
    /* Drag */
    [SerializeField] private AdornDragRot adornDragRot; 

    private AdornDuck adornDuck;
    private EAdornButtonType curDomainType = EAdornButtonType.End;
    // private 
     
    private void Awake()
    {
        ButtonBind();

        curDomainType = EAdornButtonType.End;
        gameObject.SetActive(false);
    }

    public void Active()
    {
        gameObject.SetActive(true);

        if (adornDuck == null)
        {
            adornDuck = GameInstance.Instance.MAP_GetAdornDuck();
            adornDragRot.SetAdornTransform(adornDuck.transform);
        }  
        adornDuck.Active();
         
        PressDomainButton(EAdornButtonType.Head);
    } 
    public void Disable()
    {
        curDomainType = EAdornButtonType.End;
        gameObject.SetActive(false);
        ////
        adornDuck.Disable();
    } 
    public AdornDuck GetAdornDuck()
    {
        return adornDuck;
    }
    private void ButtonBind()
    {
        // 도메인
        {
            headButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.Head));
            eyeBowsButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.EyeBows));
            eyeButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.Eye));
            beekButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.Beek));
            armButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.Arm));
            footButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.Foot));
            bodyButton.onClick.AddListener(() => PressDomainButton(EAdornButtonType.Body));
        } 
         
        // 서브 
        {
            leftArrowButton.onClick.AddListener(() => PressArrowButton(false));
            rightArrowButton.onClick.AddListener(() => PressArrowButton(true));
        }

        // 색상 
        {
            blackButton    .onClick.AddListener(() => PressColorButton(blackButton));  
            whiteButton    .onClick.AddListener(() => PressColorButton(whiteButton));  
            redButton      .onClick.AddListener(() => PressColorButton(redButton));  
            orangeButton   .onClick.AddListener(() => PressColorButton(orangeButton));  
            yellowButton   .onClick.AddListener(() => PressColorButton(yellowButton));  
            greenButton    .onClick.AddListener(() => PressColorButton(greenButton));  
            blueButton     .onClick.AddListener(() => PressColorButton(blueButton));   
            sodomyButton   .onClick.AddListener(() => PressColorButton(sodomyButton));  
            purpleButton   .onClick.AddListener(() => PressColorButton(purpleButton));
        }
         
        // 체크 
        { 
            checkButton.onClick.AddListener(() => PressCheckButton());
            cancleButton.onClick.AddListener(() => PressCancleButton());
        }
    }

    private void PressDomainButton(EAdornButtonType _type)
    {
        if (curDomainType == _type)
            return;

        curDomainType = _type;
        RefreshButtonState();
        RefreshSubButtonState();
        RefreshCurrentColor();
    } 
    private void PressArrowButton(bool _isLeft)
    {
        if (!adornDuck) 
            return;

        switch (curDomainType)
        {
            case EAdornButtonType.Head:
                {
                    var cur = adornDuck.GetHairType();
                    var next = NextEnum(cur, _isLeft, allowEnd: true);
                    if (cur != next)
                        adornDuck.SpawnHair(next);
                }
                break;

            case EAdornButtonType.EyeBows:
                {
                    var cur = adornDuck.GetEyesBowType();
                    var next = NextEnum(cur, _isLeft, allowEnd: true);
                    if (cur != next)
                        adornDuck.SpawnEyesBow(next);
                }
                break;

            case EAdornButtonType.Eye:
                {
                    var cur = adornDuck.GetEyesType();
                    var next = NextEnum(cur, _isLeft, allowEnd: true);
                    if (cur != next)
                        adornDuck.SpawnEyes(next);
                }
                break;

            case EAdornButtonType.Beek:
                {
                    var cur = adornDuck.GetBeakType();
                    var next = NextEnum(cur, _isLeft, allowEnd: true);
                    if (cur != next)
                        adornDuck.SpawnBeak(next);
                }
                break;

            case EAdornButtonType.Arm:
                {
                    var cur = adornDuck.GetHandType();
                    var next = NextEnum(cur, _isLeft, allowEnd: false);
                    if (cur != next)
                        adornDuck.SpawnHand(next);
                }
                break;

            case EAdornButtonType.Foot:
                {
                    var cur = adornDuck.GetFootType();
                    var next = NextEnum(cur, _isLeft, allowEnd: false);
                    if (cur != next)
                        adornDuck.SpawnFoot(next);
                } 
                break;

            case EAdornButtonType.Body:
                {
                    var cur = adornDuck.GetBodyType();
                    var next = NextEnum(cur, _isLeft, allowEnd: false);
                    if (cur != next)
                        adornDuck.SpawnBody(next);
                }
                break;
        }

        RenewType(curDomainType);
    }
    private void PressColorButton(Button _button)
    {
        if (!adornDuck || !_button)
            return;

        var img = _button.image;
        if (!img)
            return;

        Color color = img.color;

        // 현재 선택 색 표시
        curColor.color = color;
         
        switch (curDomainType)
        {
            case EAdornButtonType.Head:
                adornDuck.SetHairColor(color);
                break;

            case EAdornButtonType.EyeBows:
                adornDuck.SetEyesBowColor(color);
                break;

            case EAdornButtonType.Eye:
                adornDuck.SetEyesColor(color);
                break;

            case EAdornButtonType.Beek:
                adornDuck.SetBeakColor(color);
                break;

            case EAdornButtonType.Arm:
                adornDuck.SetHandColor(color);
                break;

            case EAdornButtonType.Foot:
                adornDuck.SetFootColor(color);
                break;

            case EAdornButtonType.Body:
                adornDuck.SetBodyColor(color);
                break; 
        }
    }
    
    private void PressCheckButton()
    {
        ////////////////////
        // 여기서 실제로 바뀜

        var gameInstance = GameInstance.Instance;
        var uiGroup = gameInstance.UI_GetPersistentUIGroup();
        var canvas = uiGroup.GetRadiusCollaspeCanvas();
        FRadiuseCollaspeInfo info = new FRadiuseCollaspeInfo();
        info.radType = ERadiuseCollaspeType.Small_Clear_Bigger;
        info.startPosA = new Vector2(0.5f,  0.5f);
        info.startPosB = new Vector2(0.5f,  0.5f);
        info.clearAction = ApplyAdorn; 
        canvas.Active(info);
        // 바꿔줘야함  
    }  
    private void PressCancleButton()
    {
        GameInstance.Instance.PLAYER_GetPlayerController().ChangePlay();
    }

    private void RefreshButtonState()
    {
        SetButtonColor(headButton, curDomainType == EAdornButtonType.Head);
        SetButtonColor(eyeBowsButton, curDomainType == EAdornButtonType.EyeBows);
        SetButtonColor(eyeButton, curDomainType == EAdornButtonType.Eye);
        SetButtonColor(beekButton, curDomainType == EAdornButtonType.Beek);
        SetButtonColor(armButton, curDomainType == EAdornButtonType.Arm);
        SetButtonColor(footButton, curDomainType == EAdornButtonType.Foot);
        SetButtonColor(bodyButton, curDomainType == EAdornButtonType.Body);
    } 
    private void RefreshSubButtonState()
    {
        switch (curDomainType)
        {
            case EAdornButtonType.Head:
                textMenu.text = "헤어";
                textType.text = "헤어 타입";
                break; 

            case EAdornButtonType.EyeBows:
                textMenu.text = "눈썹";
                textType.text = "눈썹 타입";
                break;

            case EAdornButtonType.Eye:
                textMenu.text = "눈";
                textType.text = "눈 타입";
                break;

            case EAdornButtonType.Beek:
                textMenu.text = "부리";
                textType.text = "부리 타입";
                break;

            case EAdornButtonType.Arm:
                textMenu.text = "팔";
                textType.text = "팔 타입";
                break;

            case EAdornButtonType.Foot:
                textMenu.text = "다리"; 
                textType.text = "다리 타입";
                break;

            case EAdornButtonType.Body:
                textMenu.text = "몸통";
                textType.text = "몸통 타입";
                break; 

            default:
                textMenu.text = string.Empty;
                textType.text = string.Empty;
                break;
        }

        // 타입 text 갱신 
        RenewType(curDomainType);
    }
    private void RefreshCurrentColor()
    {
        if (!adornDuck)
            return;

        switch (curDomainType)
        {
            case EAdornButtonType.Head:
                curColor.color = adornDuck.GetHairColor();
                break;

            case EAdornButtonType.EyeBows:
                curColor.color = adornDuck.GetEyesBowColor();
                break;

            case EAdornButtonType.Eye:
                curColor.color = adornDuck.GetEyesColor();
                break;

            case EAdornButtonType.Beek:
                curColor.color = adornDuck.GetBeakColor();
                break;

            case EAdornButtonType.Arm:
                curColor.color = adornDuck.GetHandColor();
                break;

            case EAdornButtonType.Foot:
                curColor.color = adornDuck.GetFootColor();
                break;

            case EAdornButtonType.Body:
                curColor.color = adornDuck.GetBodyColor();
                break;

            default:
                curColor.color = Color.white;
                break;
        }
    }

    private void SetButtonColor(Button _button, bool _active)
    {
        if (!_button) return;
        _button.image.color = _active ? activeColor : disableColor;

        if (_active)
        {
            _button.gameObject.GetComponent<UIAnimation>()?.Action_Animation();
        }  
    }
    private void RenewType(EAdornButtonType _type)
    {
        if (!adornDuck)
            return; 

        var gi = GameInstance.Instance;
         
        switch (_type)
        {
            case EAdornButtonType.Head:
                {
                    var type = adornDuck.GetHairType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            case EAdornButtonType.EyeBows:
                {
                    var type = adornDuck.GetEyesBowType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            case EAdornButtonType.Eye:
                {
                    var type = adornDuck.GetEyesType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            case EAdornButtonType.Beek:
                {
                    var type = adornDuck.GetBeakType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            case EAdornButtonType.Arm:
                {
                    var type = adornDuck.GetHandType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            case EAdornButtonType.Foot:
                {
                    var type = adornDuck.GetFootType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            case EAdornButtonType.Body:
                {
                    var type = adornDuck.GetBodyType();
                    textTypeObject.text = gi.TABLE_GetAdornName(type);
                }
                break;

            default:
                textTypeObject.text = string.Empty;
                break;
        }
    }
    private T NextEnum<T>(T cur, bool isLeft, bool allowEnd) where T : Enum
    {
        var values = (T[])Enum.GetValues(typeof(T));

        int maxIndex = allowEnd
            ? values.Length - 1          // End 포함
            : values.Length - 2;         // End 제외

        int idx = Array.IndexOf(values, cur);
        if (idx < 0 || idx > maxIndex)
            idx = 0;

        idx += isLeft ? -1 : 1;

        if (idx < 0) idx = maxIndex;
        if (idx > maxIndex) idx = 0;

        return values[idx];
    }

    private void ApplyAdorn()
    {
        // playerSave도 여기서 실제로 저장됨  
        adornDuck.ApplyToPlayer();
        PressCancleButton();
    }
} 
 