using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardsInHand : MonoBehaviour
{


    //TODO>  Consider if Destroy or set unactive the cards, regarding performace. Destroying is performance heavy. 



    private List<Cards> cardsInHand = new List<Cards>();

    [SerializeField] private GameObject leftHand; 

    private Transform palmTransform;

    [SerializeField] private float cardSpacing = 0.01f;

    

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
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        LayoutCardsOnPalm();
    }

    public bool IsCardInHand(Cards card)
    {   
        return cardsInHand.Contains(card);
    }

    public void AddCardToHand(Cards card)
    {
        cardsInHand.Add(card);
        
    }

    public void RemoveCardFromHand(Cards card)
    {
        cardsInHand.Remove(card);
        Destroy(((MonoBehaviour)card).gameObject);  //Probably Monobehavior guaranteed
        
    }

    public void RemoveAllCardsExpect(Cards card){
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

    public List<Cards> GetCardsInHand()
    {
        return cardsInHand;
    }

    public int GetCardsInHandCount()
    {
        return cardsInHand.Count;
    }


    private void LayoutCardsOnPalm()
    {
        int n = cardsInHand.Count;
        if (n == 0) return;

        
        float startX = -((n - 1) * cardSpacing) / 2f;

        for (int i = 0; i < n; i++)
        {
            var card = cardsInHand[i];
            
            ((MonoBehaviour)card).transform.SetParent(palmTransform, false); //Probably Monobehavior guaranteed

            
            float x = startX + i * cardSpacing;
            ((MonoBehaviour)card).transform.localPosition = new Vector3(x, 0f, 0f); //Probably Monobehavior guaranteed

            
            ((MonoBehaviour)card).transform.localRotation = Quaternion.identity; //Probably Monobehavior guaranteed
        }
    }

    
}
