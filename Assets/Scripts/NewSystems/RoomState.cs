using System.Collections.Generic;
using System.Linq;

public class RoomState
{
    public List<RoomCardInstance> cards = new List<RoomCardInstance>();
    public bool isCompleted = false;

    public RoomState(IEnumerable<CardSO> fourCards)
    {
        if (fourCards != null)
        {cards = fourCards.Select(card => new RoomCardInstance(card)).ToList();

        }
    }

    public bool HasUnresolvedEnemies()
    {
        foreach (var cards in cards)
        { 
            if (cards.cardData != null && (cards.cardData.type == CardType.Spade || cards.cardData.type == CardType.Clover) && cards.state != CardRevealState.Resolved)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateCompletion()
    {
        this.isCompleted = !HasUnresolvedEnemies();
    }
}