using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : DefaultItem
{


    protected override string ItemClass => "GoldItem"; 

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
