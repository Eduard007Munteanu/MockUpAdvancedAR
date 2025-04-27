using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCard : MonoBehaviour, Cards
{

    private string cardClass = "BuildCard";

    private CardsDeck cardDeck;

    private bool isCardGrabbable = false;


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




    public int GetCardID()
    {
        throw new System.NotImplementedException();
    }

    public Dictionary<Item, int> GetCost()
    {
        throw new System.NotImplementedException();
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

    void SendToSpecificTile(Tile tile){
        
    }

    

}
