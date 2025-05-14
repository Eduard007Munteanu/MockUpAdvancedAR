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
    private bool alwayscard = false; // For testing purposes, always true.

    public static CardsDeck Instance {get; private set;}


    public List<CardsType> cardTypes;
    private Queue<(GameObject, string)> cardQueue;

    public int numberOfCardsToDraw = 3;



    private CardsInHand cardsInHand;

    private int drawbuffer = 0;
    private bool cardPresent = false;

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
        if (alwayscard)
        {
            DrawNextCard();
        }
    }

    void ShuffleDeck()
    {
        List<(GameObject, string)> deck = new List<(GameObject, string)>();

        foreach (CardsType cardType in cardTypes)
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

        //Testing code
        foreach (var (prefab, typeName) in cardQueue)
        {
            Debug.Log($"Card Type: {typeName}, Prefab Name: {prefab.name}");
        }
        //Testing code
    }

    public void AddDraw () {
        drawbuffer += 1;
        DrawIfAvailable();
    }

    public void DrawIfAvailable () {
        if (drawbuffer <= 0) {
            Debug.Log("No cards available to draw.");
            return;
        }
        if (cardPresent) {
            Debug.Log("Card already present, cannot draw another.");
            return;
        }

        DrawNextCard(); // Draw the next card from the queue
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

            cardPresent = true;
        }
    }


    // This should be called when a player grabs the card
    public void OnCardGrabDistanceReached(DefaultCard cardInHand)
    {

        NotTheMostOptimalInitOfDeckCard(cardInHand);
        
        int extraCards = Mathf.Min(cardQueue.Count, numberOfCardsToDraw - 1);

        Debug.Log("CardQueue.count is " + cardQueue.Count);
        Debug.Log("ExtraCards is " +  extraCards);
        for (int i = 1; i <= extraCards; i++)
        {
            Debug.Log("Calling for loop!");
            if (cardQueue.Count > 0)
            {
                Debug.Log("Cardqueue.Count bigger than 0");
                (GameObject prefab, string typeName) nextCard = cardQueue.Dequeue();
                GameObject spawnedCard = Instantiate(nextCard.Item1, 
                                    transform.position + Vector3.up * 0.01f, 
                                    Quaternion.identity);


                Debug.Log("card added given card in deck was taken");
                DefaultCard card = spawnedCard.GetComponent<DefaultCard>();

                if(card == null){
                    Debug.LogError("Card from the OnCardGrabedDistanceReached is empty");
                    return;
                }

                card.Init(false, this);
                cardsInHand.AddCardToHand(spawnedCard.GetComponent<DefaultCard>());

            }
        }

        Debug.Log("Cards in hand: " + cardsInHand.GetCardsInHandCount());
        Debug.Log("Cards left in deck after drawing: " + cardQueue.Count);
        
        cardsInHand.LayoutCardsOnPalm();        

        if (alwayscard) DrawNextCard();

        drawbuffer -= 1;
        cardPresent = false;
        DrawIfAvailable();
        
    }


    public float getGrabDistance(){
        return grabDistance;
    }



    private GameObject FindPrefabByTypeName(string typeName)
    {
        foreach (CardsType ct in cardTypes)
        {
            if (ct.typeName == typeName)
                return ct.prefab;
        }
        return null;
    }

    private void NotTheMostOptimalInitOfDeckCard(DefaultCard cardInHand){
        Debug.Log("OnCardGrabDistanceReached is called");

        string originalType = cardInHand.GetCardClass(); 
        
        GameObject prefabToUse = FindPrefabByTypeName(originalType);

        if (prefabToUse == null)
        {
            Debug.LogError("No prefab found matching the grabbed card's type.");
            return;
        }

       
        Destroy(cardInHand.gameObject);

        
        GameObject newCardGO = Instantiate(prefabToUse);
        DefaultCard newCard = newCardGO.GetComponent<DefaultCard>();
        if (newCard == null)
        {
            Debug.LogError("Newly spawned card has no DefaultCard component.");
            return;
        }

        newCard.Init(false, this); 
        cardsInHand.AddCardToHand(newCard);
    }
}
