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
            new ResourceEffect(ResourceType.Wood, -100f),
            new ResourceEffect(ResourceType.Population, 0f, 0f, 0f, 0f, 0f, 0f, 5f, false),
        };
    }

    
}
