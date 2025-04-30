using System.Collections.Generic;
using UnityEngine;

public class DefaultCard : MonoBehaviour, Cards{
    private string cardClass = "DefaultCard";

    private CardsDeck cardDeck;

    private bool isCardGrabbable = false;

    [SerializeField] private Dictionary<DefaultItem, int> defineCost;  


    void Start(){

    }

    void Update(){
        SendToCardInHands();
    }


    public void Init(bool cardGrabbable, CardsDeck uniqueCardDeck){
        isCardGrabbable = cardGrabbable;
        cardDeck = uniqueCardDeck;
    }




    public string GetCardClass()
    {
        return cardClass;
    }




    // public int GetCardID()   Probably not needed. 
    // {
        
    // }

    public Dictionary<DefaultItem, int> GetCost()
    {
        return defineCost;
    }

    public void SendToCardInHands()
    {
        float grabDistance = cardDeck.getGrabDistance();
        float dist = Vector3.Distance(transform.position, cardDeck.gameObject.transform.position);
            if (dist > grabDistance)
            {
                
                Debug.Log("Card taken from the deck!");

                cardDeck?.OnCardGrabDistanceReached(this);

            }
    }
}