using UnityEngine;
using UnityEngine.UI;

public class UIHudSmall : MonoBehaviour
{
    public PetController pet;
    public int fontSize = 14;
    public Vector2 margin = new Vector2(16, 16);

    Canvas canvas;
    Text hpText, moodText, energyText;

    void Start()
    {
        if (!pet) pet = FindObjectOfType<PetController>();
        canvas = new GameObject("HUD_Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1f;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        var group = new GameObject("HUD_Group").AddComponent<RectTransform>();
        group.SetParent(canvas.transform, false);
        group.anchorMin = new Vector2(1, 0);
        group.anchorMax = new Vector2(1, 0);
        group.pivot = new Vector2(1, 0);
        group.anchoredPosition = new Vector2(-margin.x, margin.y);
        var layout = group.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 2;
        layout.childAlignment = TextAnchor.LowerRight;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        hpText = CreateText("HP: 100", group, fontSize);
        moodText = CreateText("Mood: 100", group, fontSize);
        energyText = CreateText("Energy: 100", group, fontSize);
    }

    Text CreateText(string init, RectTransform parent, int size)
    {
        var go = new GameObject("Text");
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = init;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = size;
        t.alignment = TextAnchor.MiddleRight;
        t.color = new Color(1f, 1f, 1f, 0.9f);
        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.7f);
        shadow.effectDistance = new Vector2(1, -1);
        return t;
    }

    void Update()
    {
        if (!pet) return;
        hpText.text = "HP: " + pet.vitals.hp.ToString();
        moodText.text = "Mood: " + pet.vitals.mood.ToString();
        energyText.text = "Energy: " + pet.vitals.energy.ToString();
    }
}
