using UnityEngine;

public class test : MonoBehaviour
{
    
    [SerializeField] private HandView handView;
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
            CardView cardView = CardViewCreators.Instance.CreateCardView(transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(cardView));
        }
    }
}
