public enum CardRevealState { Hidden, Revealed, Resolved }

public class RoomCardInstance
{
    public CardSO cardData;
    public CardRevealState state = CardRevealState.Hidden;

    public RoomCardInstance(CardSO cardData)
    {
        this.cardData = cardData;
        this.state = CardRevealState.Hidden;
    }
}