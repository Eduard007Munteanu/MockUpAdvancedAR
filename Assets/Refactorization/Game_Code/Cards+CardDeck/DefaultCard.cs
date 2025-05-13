using System.Collections.Generic;
using UnityEngine;

public abstract class DefaultCard : MonoBehaviour, Cards{
    protected virtual string CardClass => "DefaultCard";

    private CardsDeck cardDeck;

    private bool isCardGrabbable = false;
    private bool isAlreadyGrabbed = false; 

    [SerializeField] private Dictionary<DefaultItem, int> defineCost;

    protected List<ResourceEffect> resourceEffects;

    void Start(){

    }

    void Update(){
        SendToCardInHands();
    }

    public virtual void Init(bool cardGrabbable, CardsDeck uniqueCardDeck){
        isCardGrabbable = cardGrabbable;
        cardDeck = uniqueCardDeck;
    }

    public string GetCardClass()
    {
        return CardClass;
    }

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

    public bool TryBuyCard()
    {
        if (resourceEffects == null || resourceEffects.Count == 0)
        {
            Debug.Log("No resource effects to apply.");
            return true;
        }
        
        // check if resource has enough amount
        foreach (var effect in resourceEffects)
        {
            if (!effect.IsEnough())
            {
                Debug.Log("Not enough resources to apply effect.");
                return false;
            }
        }
        
        foreach (var effect in resourceEffects) effect.Apply();

        return true;
    }
}