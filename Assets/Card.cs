using UnityEngine;

public class Card : MonoBehaviour
{
    private Transform deckTransform;
    private CardDeck cardDeck;

    private string cardName;
    private bool hasBeenTaken = false;

    public float grabDistance = 0.25f; 

    public Transform leftHandTransform; 

    private bool isFollowingHand = false;

    private int relativeToPalmIndexPosition;

    //private bool activatePinchSelection;

    private int myInHandId = -1; // -1 means not selected, 0 means selected

    public void Initialize(Transform deckTransform, CardDeck deck, string cardName, int relativeToPalmIndexPosition )
    {
        this.deckTransform = deckTransform;
        this.cardDeck = deck;
        this.cardName = cardName;
        this.relativeToPalmIndexPosition = relativeToPalmIndexPosition;
    }

    public void DestroySelf()
    {
        isFollowingHand = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void Start()
    {
        Material myMat = GenerateMaterialFromSprite();
        if(myMat != null)
        {
            GetComponent<SpriteRenderer>().material = myMat;
        }
    }


    void Update()
    {

    

        if (!hasBeenTaken && deckTransform != null)
        {
            float dist = Vector3.Distance(transform.position, deckTransform.position);
            if (dist > grabDistance)
            {
                hasBeenTaken = true;
                isFollowingHand = true;
                Debug.Log("Card taken from the deck!");

                cardDeck?.OnCardGrabbed(this);

            }
        }

        if (isFollowingHand && leftHandTransform!= null)
        {
            // transform.position = leftHandTransform.position;
            // transform.rotation = leftHandTransform.rotation;


            Vector3 basePosition = leftHandTransform.position;
            float spacing = 0.1f;
            Vector3 offset = leftHandTransform.right * spacing * relativeToPalmIndexPosition;

            transform.position = basePosition + offset;
            transform.rotation = leftHandTransform.rotation;
        }


        

    }

    public void CardNotFromDeck(){
        hasBeenTaken = true;
        isFollowingHand = true;
        Debug.Log("Card Not from Deck!");
    }


    public Material GenerateMaterialFromSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Material newMat = new Material(Shader.Find("Sprites/Default"));
            newMat.mainTexture = sr.sprite.texture;
            return newMat;
        }
        return null;
    }

    public void ActivatePinchSelection(int cardIndex){
        myInHandId = cardIndex;
    }


    public int GetMyInHandId(){
        return myInHandId;
    }

    public string getCardName(){
        return cardName;
    }



}
