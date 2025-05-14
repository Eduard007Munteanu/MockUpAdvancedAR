using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmCard : BuildCard
{

   protected override string CardClass => "farming";

   public override void Init(bool cardGrabbable, CardsDeck uniqueCardDeck) {
        base.Init(cardGrabbable, uniqueCardDeck);
        resourceEffects = new List<ResourceEffect>
        {
            new ResourceEffect(ResourceType.Wood, -100f),
        };
    }
}
