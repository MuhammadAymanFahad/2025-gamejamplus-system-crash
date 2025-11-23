using UnityEngine;

public class test : MonoBehaviour
{
    
    [SerializeField] private HandView handView;
    [SerializeField] private HolderView holderView;
    [SerializeField] private CardData cardData;
    void Start()
    {
        // Main logic on how to add the card (VERY IMPORTANT WILL USE LATER)
        // CardView cardView = CardViewCreators.Instance.CreateCardView(transform.position, Quaternion.identity);
        // StartCoroutine(handView.AddCard(cardView));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
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
