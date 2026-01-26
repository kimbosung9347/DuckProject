using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curTime;
    [SerializeField] private TextMeshProUGUI afterTime;
    [SerializeField] private GameObject nextDay;
    [SerializeField] private Slider slider;

    [SerializeField] private Button backButton;
    [SerializeField] private Button sleepButton;


    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);
        BindButton();

        Disable();
    }
    private void Update()
    {
        if (!gameObject.activeSelf)
            return;
        RefreshTime(slider.value);
    }
     
    public void Active()
    { 
        gameObject.SetActive(true);
        slider.value = 0f;
        RefreshTime(slider.value);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnSliderChanged(float value)
    {
        RefreshTime(value);
    }
    private void RefreshTime(float sliderValue) 
    {
        var playerTime = GameInstance.Instance.PLAYER_GetPlayerTime();
        float cur = playerTime.GetGameTime();
         
        float sleepHour = Mathf.Lerp(0f, 24f, sliderValue);
        float afterRaw = cur + sleepHour;

        bool isNextDay = afterRaw >= 24f;
        float after = isNextDay ? afterRaw - 24f : afterRaw;

        nextDay.SetActive(isNextDay);

        SetTimeText(curTime, cur);
        SetTimeText(afterTime, after);
    }
    private void SetTimeText(TextMeshProUGUI text, float gameTime)
    {
        int totalMinutes = Mathf.FloorToInt(gameTime * 60f);
        int hour = totalMinutes / 60;
        int minute = totalMinutes % 60;

        text.SetText($"{hour:00}:{minute:00}");
    }
    private void BindButton()
    {
        backButton.onClick.RemoveAllListeners();
        sleepButton.onClick.RemoveAllListeners();

        backButton.onClick.AddListener(PressBackbutton);
        sleepButton.onClick.AddListener(PressSleepButton);
    }
     
    private void PressBackbutton()
    { 
        GameInstance.Instance.PLAYER_GetPlayerController().ChangePlay();
    }
    private void PressSleepButton()
    {
        var playerTime = GameInstance.Instance.PLAYER_GetPlayerTime();
        float sleepHour = Mathf.Lerp(0f, 24f, slider.value);
        float cur = playerTime.GetGameTime();
        float afterRaw = cur + sleepHour;
        float after = afterRaw % 24f;
         
        var interaction = GameInstance.Instance.PLAYER_GetPlayerInteraction();
        interaction.SuccesSleep(after);
    } 
}
