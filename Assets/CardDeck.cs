using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


[System.Serializable]
public class CardType
{
    public GameObject prefab;
    public int count;

    public string typeName;
}


public class CardDeck : MonoBehaviour
{
    public List<CardType> cardTypes;
    private Queue<(GameObject, string)> cardQueue;

    public int numberOfCardsToDraw = 5;

    public OVRSkeleton leftHandSkeleton;

    public Transform leftHandTransform;

    private Transform palmTransform = null;


    public static Material cardMaterialInLeftHand;  
    public static Sprite cardSpriteInLeftHand; 

    public static Card cardInLeftHand;

    private static List<Card> cardsInHand = new List<Card>();

    public static string selectedCardClass = null;



    
    void Start()
    {

        foreach (var bone in leftHandSkeleton.Bones)
        {
            if (bone.Id.ToString().Contains("Palm")) 
            {
                palmTransform = bone.Transform;
                Debug.Log("Found the left hand palm bone!");
                break;
            }
        }

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
            Card card = spawnedCard.GetComponent<Card>();
            if (card != null)
            {
                card.leftHandTransform = palmTransform;
                card.Initialize(transform, this, nextCard.Item2, 0 );

            }
            
            
        }
    }


    // This should be called when a player grabs the card
    public void OnCardGrabbed(Card cardInHand)
    {


        //!!!! Potential incorrect
        ClearHand();
        //!!!! Potential incorrect

        Debug.Log("OnCardGrabbed is called");
        cardsInHand.Add(cardInHand);

        int extraCards = numberOfCardsToDraw - 1;
        for (int i = 1; i <= extraCards; i++)
        {
            if (cardQueue.Count > 0)
            {
                (GameObject prefab, string typeName) nextCard = cardQueue.Dequeue();
                GameObject spawnedExtra = Instantiate(nextCard.prefab,
                    palmTransform.position, // initial position will be adjusted by Card.Update()
                    Quaternion.identity,
                    transform);

                Card extraCard = spawnedExtra.GetComponent<Card>();
                if (extraCard != null)
                {
                    extraCard.leftHandTransform = palmTransform;
                    extraCard.CardNotFromDeck();
                    extraCard.Initialize(palmTransform, this, nextCard.typeName, i);
                }

                cardsInHand.Add(extraCard);



            }
        }

        Debug.Log("Cards in hand: " + cardsInHand.Count);
        Debug.Log("Cards left in deck after drawing: " + cardQueue.Count);
        
        for (int cardindex = 0; cardindex < cardsInHand.Count; cardindex++)
        {
            Card card = cardsInHand[cardindex];
            card.ActivatePinchSelection(cardindex);
        }
        

        
        DrawNextCard();
        
    }






    //!!!! Potential incorrect
    public static void ClearHand()
    {
        foreach (Card card in cardsInHand)
        {
            card.DestroySelf();
        }
        cardsInHand.Clear();
    }
    //!!!! Potential incorrect
    

    // Static for now //
    public static void AssignSpriteAndMaterial(Card cardInHand){
        cardInLeftHand = cardInHand; 
        SpriteRenderer sr = cardInHand.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            cardSpriteInLeftHand = sr.sprite;

            
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = sr.sprite.texture;

            cardMaterialInLeftHand = mat;
        }

        selectedCardClass = cardInHand.getCardName();
    }

    public static void SelectCard(Card selectedCard)
    {
        List<Card> cardsToRemove = new List<Card>();

        foreach (Card card in cardsInHand)
        {
            if (card != selectedCard)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (Card card in cardsToRemove)
        {
            cardsInHand.Remove(card);
            card.DestroySelf();
        }

        // Assign the selected card explicitly
        AssignSpriteAndMaterial(selectedCard);
        
        Debug.Log($"Card '{selectedCard}' selected. Other cards removed.");
    }
}
