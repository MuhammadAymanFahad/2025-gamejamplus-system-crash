using System.Collections.Generic;

public class RoomState
{
    public List<RoomCardInstance> cards = new List<RoomCardInstance>();
    public bool isCompleted = false;

    public RoomState(IEnumerable<CardSO> fourCards)
    {
        foreach(var c in fourCards)
        {
            cards.Add(new RoomCardInstance(c));
        }
    }

    public bool HasUnresolvedEnemies()
    {
        foreach (var c in cards)
        { 
            if((c.cardData.type == CardType.Spade || c.cardData.type == CardType.Clover) && c.state != CardRevealState.Resolved)
            {
                return true;
            }
        }
        return false;
    }
}