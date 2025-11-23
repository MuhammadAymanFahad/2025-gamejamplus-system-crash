using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new();

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        yield return UpdateCardPositions(0.15f);
    }
    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;
        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;

        Spline spline = splineContainer.Spline;
        
        for (int i = 0; i < cards.Count; i++)
        {
            float eachposition = firstCardPosition + i * cardSpacing;

            Vector3 splinerPosition = spline.EvaluatePosition(eachposition);
            Vector3 forward = spline.EvaluateTangent(eachposition);
            Vector3 up = spline.EvaluateUpVector(eachposition);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized) ;

            cards[i].transform.DOMove(splinerPosition + transform.position + 0.01f * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return new WaitForSeconds(duration);
    }
}
