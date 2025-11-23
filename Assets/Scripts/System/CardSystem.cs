using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> handPile = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardPerformer);
        ActionSystem.SubscribePerformer<EnemyTurnGameAction>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribePerformer<EnemyTurnGameAction>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.UnSubscribePerformer<EnemyTurnGameAction>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnSubscribePerformer<EnemyTurnGameAction>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    // Publics 
    public void Setup (List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }
    // performers

    private IEnumerator DrawCardsPerformer (DrawCardsGA drawCardsGA)
    {
        int actualAmount = (int)MathF.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardsGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }

        if (notDrawnAmount > 0)
        {
            RefillDeck();
            for (int i = 0; i < notDrawnAmount; i++)
            {
                yield return DrawCard();
            }
        }
    }
    private IEnumerator DiscardAllCardPerformer (DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in handPile)
        {
            discardPile.Add(card);
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);            
        }
        handPile.Clear();
    }

    // Reactions

    private void EnemyTurnPreReaction (EnemyTurnGameAction enemyTurnGameAction)
    {
        DiscardAllCardsGA discardAllCardsGA = new ();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction (EnemyTurnGameAction enemyTurnGameAction)
    {
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }

    // Helpers

    private IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        handPile.Add(card);

        CardView cardView = CardViewCreators.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        
        yield return handView.AddCard(cardView);
    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    private IEnumerator DiscardCard (CardView cardView)
    {
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView);
    }
}
