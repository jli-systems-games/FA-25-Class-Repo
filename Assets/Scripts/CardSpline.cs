using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using DG.Tweening;


public class CardSpline : MonoBehaviour
{
    [SerializeField] private int maxHandSize = 3;
    [SerializeField] private SplineContainer splineContainer; // 发牌路径
    [SerializeField] private Transform spawnPoint;       // 牌生成位置

    private readonly List<GameObject> handCards = new();

    public void AddCard(GameObject card)
    {
        if (handCards.Count >= maxHandSize) return;

        card.transform.position = spawnPoint.position;
        card.transform.rotation = spawnPoint.rotation;
        card.transform.SetParent(transform);

        handCards.Add(card);
        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float t = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(t);
            Vector3 forward = spline.EvaluateTangent(t);
            Vector3 up = spline.EvaluateUpVector(t);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            handCards[i].transform.DOMove(splinePosition, 0.4f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.4f);
        }
    }

    public void ClearCards()
    {
        foreach (var c in handCards)
        {
            if (c != null) Destroy(c);
        }
        handCards.Clear();
    }
}
