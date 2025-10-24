using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameClock : MonoBehaviour
{
    [Header("UI ")]
    public TextMeshProUGUI clockText;

    [Header("超级就是快进")]
    [Tooltip("要控制是否可点击的按钮")]
    public Button burstButton;

    [Header("切换")]
    [Tooltip("切换贴图")]
    public Image targetUIImage;

    [Tooltip("普通时段贴图")]
    public Sprite idleSprite;

    [Tooltip("快进贴图")]
    public Sprite workSprite;

    [Header("平时的时间比")]
    public float realSecondsPerGameHour = 36000f;

    [Header("快进时间比")]
    public int fastStartHour = 3;
    public int fastEndHour = 11;
    public float burstMultiplier = 600f;

    [Header("显示细节")]
    public int spacesAroundColon = 1;

    private float elapsedGameMinutes = 0f;
    private bool isBurstActive = false;

    private bool hasSwitchedToWork = false;

    void Start()
    {
        if (clockText == null)
            clockText = GetComponent<TextMeshProUGUI>();

        elapsedGameMinutes = 11f * 60f;

        UpdateClockDisplay();


        if (targetUIImage != null && idleSprite != null)
            targetUIImage.sprite = idleSprite;
    }

    void Update()
    {
        int currentHour = Mathf.FloorToInt(elapsedGameMinutes / 60f) % 24;

    
        if (burstButton != null)
        {
            burstButton.interactable = IsInFastTimeRange(currentHour);
        }

      
        float gameMinutesPerRealSecond = 60f / realSecondsPerGameHour;

 
        if (isBurstActive)
        {
            gameMinutesPerRealSecond *= burstMultiplier;
            if (!IsInFastTimeRange(currentHour))
                isBurstActive = false;
        }

   
        elapsedGameMinutes += Time.deltaTime * gameMinutesPerRealSecond;
        if (elapsedGameMinutes >= 24 * 60f)
            elapsedGameMinutes -= 24 * 60f;

        UpdateClockDisplay();

        if (IsInFastTimeRange(currentHour) && !hasSwitchedToWork)
        {
            hasSwitchedToWork = true;
            SwitchToWorkImage();
        }
    }

    public void TriggerBurst()
    {
        int currentHour = Mathf.FloorToInt(elapsedGameMinutes / 60f) % 24;

        if (IsInFastTimeRange(currentHour))
        {
            isBurstActive = true;
        }
    
    }

    bool IsInFastTimeRange(int hour)
    {
        return hour >= fastStartHour && hour < fastEndHour;
    }

    void UpdateClockDisplay()
    {
        int hours = Mathf.FloorToInt(elapsedGameMinutes / 60f) % 24;
        int minutes = Mathf.FloorToInt(elapsedGameMinutes % 60f);

        string space = new string(' ', Mathf.Max(0, spacesAroundColon));
        clockText.text = $"{hours:00}{space}:{space}{minutes:00}";
    }

    void SwitchToWorkImage()
    {
        if (targetUIImage == null) return;
        if (workSprite == null) return;

        targetUIImage.sprite = workSprite;
    }
}
