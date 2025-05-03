using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardsInHand : MonoBehaviour
{

    public static CardsInHand Instance {get; private set;}


    //TODO>  Consider if Destroy or set unactive the cards, regarding performace. Destroying is performance heavy. 



    private List<DefaultCard> cardsInHand = new List<DefaultCard>();

    [SerializeField] private GameObject leftHand; 

    private Transform palmTransform;

    [SerializeField] private float cardSpacing = 0.9f;

    




    public Transform GetPalmTransform(){
        return palmTransform;
    }


    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        OVRSkeleton leftHandSkeleton = leftHand.GetComponent<OVRSkeleton>();


        foreach (var bone in leftHandSkeleton.Bones)
        {
            if (bone.Id.ToString().Contains("Palm")) 
            {
                palmTransform = bone.Transform;
                Debug.Log("Found the left hand palm bone!");
                transform.SetParent(palmTransform, false);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //LayoutCardsOnPalm();
    }

    public bool IsCardInHand(DefaultCard card)
    {   
        return cardsInHand.Contains(card);
    }

    public void AddCardToHand(DefaultCard card)
    {
        cardsInHand.Add(card);

        Debug.Log("This was called, adding card with prefab name " + card.gameObject.name);

    
        Vector3 originalScale = card.transform.localScale;
        card.transform.SetParent(this.transform, true);
        card.transform.localScale = originalScale;
        

        
        
    }

    public void RemoveCardFromHand(DefaultCard card)
    {
        cardsInHand.Remove(card);
        Destroy(card.gameObject);  //Probably Monobehavior guaranteed
        LayoutCardsOnPalm();

        
        
    }

    public void RemoveAllCardsExpect(DefaultCard card){
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            if (cardsInHand[i] != card)
            {
                RemoveCardFromHand(cardsInHand[i]);
            }
        }
    }

    public void RemoveAllCards()
    {
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            RemoveCardFromHand(cardsInHand[i]);
        }
    }

    public List<DefaultCard> GetCardsInHand()
    {
        return cardsInHand;
    }

    public int GetCardsInHandCount()
    {
        return cardsInHand.Count;
    }


    public void LayoutCardsOnPalm()
    {
        int n = cardsInHand.Count;
        if (n == 0) return;

        
        float startX = -((n - 1) * cardSpacing) / 2f;

        for (int i = 0; i < n; i++)
        {
            var card = cardsInHand[i];
            
            
            float x = startX + i * cardSpacing;
            card.transform.localPosition = new Vector3(x, 0f, 0f); //Probably Monobehavior guaranteed

            
            card.transform.localRotation = Quaternion.identity; //Probably Monobehavior guaranteed

            
        }
    }



    


    
}
