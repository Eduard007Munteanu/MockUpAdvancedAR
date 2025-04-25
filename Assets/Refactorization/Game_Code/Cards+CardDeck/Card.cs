using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Cards 
{
    Dictionary<Item, int> GetCost();

    int GetCardID();

    string GetCardClass();

    void SendToCardInHands();


}
