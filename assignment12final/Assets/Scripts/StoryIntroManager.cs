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

    private int currentIndex = 0;
    private bool introFinished = false;

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

        if (titleText != null)
        {
            titleText.text = slide.title;
        }

        if (bodyText != null)
        {
            bodyText.text = slide.body;
        }

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
    }

    void OnClickNext()
    {
        if (introFinished) return;

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
