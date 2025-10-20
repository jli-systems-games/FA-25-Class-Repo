using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TarotShuffle2D : MonoBehaviour
{
    [Header("Card Setup")]
    public List<Button> cardSlots;        
    public Sprite cardBack;               
    public Sprite[] cardFronts;           

    [Header("Shuffle Control")]
    public Button shuffleButton;          
    public float shuffleDuration = 1.5f;  
    public float shuffleRadius = 80f;     
    public float rotationJitter = 5f;     

    private Dictionary<Button, Sprite> cardAssignments = new Dictionary<Button, Sprite>();
    private List<Vector3> originalPositions = new List<Vector3>();
    private Vector3 groupCenter;

    void Start()
    {
        foreach (var card in cardSlots)
            originalPositions.Add(card.transform.localPosition);

       
        groupCenter = Vector3.zero;
        foreach (var pos in originalPositions) groupCenter += pos;
        groupCenter /= originalPositions.Count;

        foreach (var card in cardSlots)
        {
            RectTransform rt = card.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        SetupGame();

        shuffleButton.onClick.AddListener(() => StartCoroutine(ShuffleAnim()));
    }

    void SetupGame()
    {
        List<Sprite> deck = new List<Sprite>(cardFronts);
        Shuffle(deck);

        cardAssignments.Clear();
        for (int i = 0; i < cardSlots.Count; i++)
        {
            Button btn = cardSlots[i];
            btn.image.sprite = cardBack;
            cardAssignments[btn] = deck[i];
            btn.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-rotationJitter, rotationJitter));
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => StartCoroutine(FlipCard(btn)));
        }
    }

    IEnumerator FlipCard(Button card)
    {
        card.interactable = false;
        float t = 0;
        Vector3 originalScale = card.transform.localScale;

        
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            float x = Mathf.Lerp(originalScale.x, 0, t);
            card.transform.localScale = new Vector3(x, originalScale.y, originalScale.z);
            yield return null;
        }

        
        if (card.image.sprite == cardBack)
            card.image.sprite = cardAssignments[card];
        else
            card.image.sprite = cardBack;

      
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            float x = Mathf.Lerp(0, originalScale.x, t);
            card.transform.localScale = new Vector3(x, originalScale.y, originalScale.z);
            yield return null;
        }
        card.interactable = true;
    }

    IEnumerator ShuffleAnim()
    {
        
        float elapsed = 0f;
        float angleStart = Random.Range(0, Mathf.PI * 2);
        float angleEnd = angleStart + Mathf.PI * 2; 
        List<Vector3> offsets = new List<Vector3>();

        
        foreach (var pos in originalPositions)
            offsets.Add(pos - groupCenter);

        // Shuffle animation
        while (elapsed < shuffleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shuffleDuration;
            float angle = Mathf.Lerp(angleStart, angleEnd, t);

            for (int i = 0; i < cardSlots.Count; i++)
            {
               
                float fan = Mathf.Sin(t * Mathf.PI) * 40f * (i - (cardSlots.Count - 1) / 2f);
                Vector3 to = groupCenter + offsets[i].normalized * fan;
                
                Vector3 centerPos = groupCenter + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * shuffleRadius;
                cardSlots[i].transform.localPosition = Vector3.Lerp(cardSlots[i].transform.localPosition, centerPos + to - groupCenter, 0.4f);
                cardSlots[i].transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-rotationJitter, rotationJitter));
            }
            yield return null;
        }

        
        float returnDuration = 0.4f;
        float t2 = 0f;
        List<Vector3> currentPositions = new List<Vector3>();
        foreach (var card in cardSlots)
            currentPositions.Add(card.transform.localPosition);

        while (t2 < returnDuration)
        {
            t2 += Time.deltaTime;
            float lerpT = Mathf.SmoothStep(0, 1, t2 / returnDuration);
            for (int i = 0; i < cardSlots.Count; i++)
            {
                cardSlots[i].transform.localPosition = Vector3.Lerp(currentPositions[i], originalPositions[i], lerpT);
                cardSlots[i].transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-rotationJitter, rotationJitter));
            }
            yield return null;
        }

        SetupGame(); 
    }

    void Shuffle(List<Sprite> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Sprite temp = deck[i];
            int rand = Random.Range(i, deck.Count);
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }
}