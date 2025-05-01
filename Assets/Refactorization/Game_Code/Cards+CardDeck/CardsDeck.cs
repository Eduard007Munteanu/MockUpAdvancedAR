using System.Collections.Generic;
using Oculus.Interaction.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


[System.Serializable]
public class CardsType
{
    public GameObject prefab;
    public int count;

    public string typeName;
}


public class CardsDeck : MonoBehaviour
{

    public static CardsDeck Instance {get; private set;}


    public List<CardType> cardTypes;
    private Queue<(GameObject, string)> cardQueue;

    public int numberOfCardsToDraw = 5;



    private CardsInHand cardsInHand; 

    [SerializeField] float grabDistance;



    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }


    void Start()
    {
        cardsInHand = CardsInHand.Instance;
        ShuffleDeck();
        DrawNextCard();
    }

    void ShuffleDeck()
    {
        List<(GameObject, string)> deck = new List<(GameObject, string)>();

        foreach (CardType cardType in cardTypes)
        {
            for (int i = 0; i < cardType.count; i++)
            {
                deck.Add((cardType.prefab, cardType.typeName));
            }
        }

        // Shuffle
        for (int i = 0; i < deck.Count; i++)
        {
            (GameObject, string) temp = deck[i];
            int randIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randIndex];
            deck[randIndex] = temp;
        }

        cardQueue = new Queue<(GameObject, string)>(deck);
    }


    public void DrawNextCard()
    {

        if (cardQueue.Count > 0)
        {
            
            (GameObject, string) nextCard = cardQueue.Dequeue();
            GameObject spawnedCard = Instantiate(nextCard.Item1, 
                                    transform.position + Vector3.up * 0.01f, 
                                    Quaternion.identity, 
                                    transform);
            Debug.Log("Card drawn from the deck!");
            DefaultCard card = spawnedCard.GetComponent<DefaultCard>();
            if (card == null)
            {
                Debug.LogError("Card component not found on the spawned card prefab.");
                return;
            } 
            card.Init(true, this);
        }
    }


    // This should be called when a player grabs the card
    public void OnCardGrabDistanceReached(DefaultCard cardInHand)
    {


        cardsInHand.RemoveAllCards();

        Debug.Log("OnCardGrabDistanceReached is called");


        cardsInHand.AddCardToHand(cardInHand);

        int extraCards = numberOfCardsToDraw - 1;
        for (int i = 1; i <= extraCards; i++)
        {
            if (cardQueue.Count > 0)
            {
                (GameObject prefab, string typeName) nextCard = cardQueue.Dequeue();
                GameObject spawnedCard = Instantiate(nextCard.Item1, 
                                    transform.position + Vector3.up * 0.01f, 
                                    Quaternion.identity, 
                                    transform);
                cardsInHand.AddCardToHand(spawnedCard.GetComponent<DefaultCard>());



            }
        }

        Debug.Log("Cards in hand: " + cardsInHand.GetCardsInHandCount());
        Debug.Log("Cards left in deck after drawing: " + cardQueue.Count);
        
        

        DrawNextCard();
        
    }


    public float getGrabDistance(){
        return grabDistance;
    }
}
