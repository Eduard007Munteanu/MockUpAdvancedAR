using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepCard : BuildCard
{

    protected override string CardClass => "sleep";

    public override void Init(bool cardGrabbable, CardsDeck uniqueCardDeck) {
        base.Init(cardGrabbable, uniqueCardDeck);
        resourceEffects = new List<ResourceEffect>
        {
            new ResourceEffect(ResourceType.Gold, -50f),
        };
    }

    
}
