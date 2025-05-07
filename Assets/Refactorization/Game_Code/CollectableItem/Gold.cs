using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : DefaultItem
{


    protected override string ItemClass => "GoldItem"; 
    public override ResourceType Type => ResourceType.Gold; // TODO: Update ResourceType

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   

    public override string GetItemClass()
    {
        return ItemClass;
    }

    
}
