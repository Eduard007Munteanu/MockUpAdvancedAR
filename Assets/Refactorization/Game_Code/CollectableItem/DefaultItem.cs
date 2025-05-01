using UnityEngine;
using System.Collections.Generic;
using System.Data.Common;


public class DefaultItem : MonoBehaviour, Item
{

    private int Id;

    protected virtual string ItemClass => "DefaultItem"; 



    public void Init(int Id){   
        this.Id = Id;
    }


    public virtual string GetItemClass(){
        return ItemClass;
    }





}