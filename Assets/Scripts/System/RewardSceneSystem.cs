using UnityEngine;

public class RewardSceneSystem : MonoBehaviour
{
    
    [SerializeField] private HandView handView;
    [SerializeField] private HolderView holderView;
    [SerializeField] private CardData cardData;
    
    void Start()
    {
        int NumberOfCard = Random.Range(1, 4);

        for (int i = 1; i <= NumberOfCard; i++)
        {
            Card card = new(cardData);
            if (handView != null)
            {
                CardView cardView = CardViewCreators.Instance.CreateCardView(card, transform.position, Quaternion.identity);
                StartCoroutine(handView.AddCard(cardView));    
            } else
            {
                CardView cardView = CardViewCreators.Instance.CreateCardView(card, transform.position, Quaternion.identity);
                StartCoroutine(holderView.AddCard(cardView));
            }
        }
        
    }
}
