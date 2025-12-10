using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryIntroManager : MonoBehaviour
{
    [System.Serializable]
    public class IntroSlide
    {
        [TextArea(1, 3)]
        public string title;

        [TextArea(3, 8)]
        public string body;

        public Sprite backgroundSprite;
        public Sprite illustrationSprite;
    }

    [Header("Intro UI Root")]
    public GameObject introCanvasRoot;

    [Header("UI References")]
    public Image backgroundImage;
    public Image illustrationImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button nextButton;
    public Button skipButton;

    [Header("Slides")]
    public List<IntroSlide> slides = new List<IntroSlide>();

    [Header("Gameplay References")]
    public MonoBehaviour playerController;
    public GameObject mainUICanvas;

    [Header("Typewriter Settings")]
    public float titleCharsPerSecond = 25f;
    public float bodyCharsPerSecond = 35f;

    private int currentIndex = 0;
    private bool introFinished = false;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (mainUICanvas != null)
        {
            mainUICanvas.SetActive(false);
        }

        if (introCanvasRoot != null)
        {
            introCanvasRoot.SetActive(true);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnClickNext);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(FinishIntro);
        }

        if (slides.Count > 0)
        {
            ShowSlide(0);
        }
        else
        {
            FinishIntro();
        }
    }

    void Update()
    {
        if (introFinished) return;
        if (introCanvasRoot == null || !introCanvasRoot.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnClickNext();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinishIntro();
        }
    }

    void ShowSlide(int index)
    {
        if (index < 0 || index >= slides.Count) return;

        currentIndex = index;
        IntroSlide slide = slides[index];

        if (backgroundImage != null)
        {
            if (slide.backgroundSprite != null)
            {
                backgroundImage.sprite = slide.backgroundSprite;
                backgroundImage.color = Color.white;
                backgroundImage.enabled = true;
            }
            else
            {
                backgroundImage.enabled = true;
            }
        }

        if (illustrationImage != null)
        {
            if (slide.illustrationSprite != null)
            {
                illustrationImage.sprite = slide.illustrationSprite;
                illustrationImage.enabled = true;
            }
            else
            {
                illustrationImage.enabled = false;
            }
        }

        StartTypingSlide(slide);
    }

    void StartTypingSlide(IntroSlide slide)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        typingCoroutine = StartCoroutine(TypeSlide(slide));
    }

    System.Collections.IEnumerator TypeSlide(IntroSlide slide)
    {
        isTyping = true;

        string fullTitle = slide.title ?? "";
        string fullBody = slide.body ?? "";

        if (titleText != null) titleText.text = "";
        if (bodyText != null) bodyText.text = "";

        float tTitle = 0f;
        float tBody = 0f;
        int idxTitle = 0;
        int idxBody = 0;
        int lenTitle = fullTitle.Length;
        int lenBody = fullBody.Length;

        while (idxTitle < lenTitle || idxBody < lenBody)
        {
            tTitle += Time.deltaTime * titleCharsPerSecond;
            tBody += Time.deltaTime * bodyCharsPerSecond;

            if (idxTitle < lenTitle && titleText != null)
            {
                int newIdxTitle = Mathf.Clamp(Mathf.FloorToInt(tTitle), 0, lenTitle);
                if (newIdxTitle != idxTitle)
                {
                    idxTitle = newIdxTitle;
                    titleText.text = fullTitle.Substring(0, idxTitle);
                }
            }

            if (idxBody < lenBody && bodyText != null)
            {
                int newIdxBody = Mathf.Clamp(Mathf.FloorToInt(tBody), 0, lenBody);
                if (newIdxBody != idxBody)
                {
                    idxBody = newIdxBody;
                    bodyText.text = fullBody.Substring(0, idxBody);
                }
            }

            yield return null;
        }

        if (titleText != null) titleText.text = fullTitle;
        if (bodyText != null) bodyText.text = fullBody;

        isTyping = false;
        typingCoroutine = null;
    }

    void CompleteCurrentSlideInstant()
    {
        if (!isTyping) return;
        isTyping = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (currentIndex >= 0 && currentIndex < slides.Count)
        {
            IntroSlide slide = slides[currentIndex];
            if (titleText != null) titleText.text = slide.title;
            if (bodyText != null) bodyText.text = slide.body;
        }
    }

    void OnClickNext()
    {
        if (introFinished) return;

        if (isTyping)
        {
            CompleteCurrentSlideInstant();
            return;
        }

        currentIndex++;

        if (currentIndex >= slides.Count)
        {
            FinishIntro();
        }
        else
        {
            ShowSlide(currentIndex);
        }
    }

    void FinishIntro()
    {
        introFinished = true;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;

        if (introCanvasRoot != null)
        {
            introCanvasRoot.SetActive(false);
        }

        if (mainUICanvas != null)
        {
            mainUICanvas.SetActive(true);
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}
