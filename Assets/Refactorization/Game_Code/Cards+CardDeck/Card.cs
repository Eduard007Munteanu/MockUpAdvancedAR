using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;

public interface Cards 
{

    Dictionary<Item, int> GetCost();

    int GetCardID();

    string GetCardClass();

    void SendToCardInHands();

    void Init(bool cardGrabbable, CardsDeck uniqueCardDeck);


}
