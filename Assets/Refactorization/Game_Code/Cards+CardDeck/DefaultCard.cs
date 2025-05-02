using System.Collections.Generic;
using UnityEngine;

public abstract class DefaultCard : MonoBehaviour, Cards{
    protected virtual string CardClass => "DefaultCard";

    private CardsDeck cardDeck;

    private bool isCardGrabbable = false;
    private bool isAlreadyGrabbed = false; 

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
        return CardClass;
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
        if(!isCardGrabbable || cardDeck == null || isAlreadyGrabbed) return;
        float grabDistance = cardDeck.getGrabDistance();
        float dist = Vector3.Distance(transform.position, cardDeck.gameObject.transform.position);
            if (dist > grabDistance)
            {
                
                Debug.Log("Card taken from the deck!");
                isAlreadyGrabbed = true;
                cardDeck?.OnCardGrabDistanceReached(this);

            }
    }
}