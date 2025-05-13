using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryCard : BuildCard
{

  protected override string CardClass => "military";

  public override void Init(bool cardGrabbable, CardsDeck uniqueCardDeck) {
        base.Init(cardGrabbable, uniqueCardDeck);
        resourceEffects = new List<ResourceEffect>
        {
            new ResourceEffect(ResourceType.Gold, -50f),
        };
    }
    
}
