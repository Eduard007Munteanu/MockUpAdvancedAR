using UnityEngine;
using System.Collections.Generic;
using System.Data.Common;


public class DefaultItem : MonoBehaviour, Item
{

    private int Id;



    public void Init(int Id){   
        this.Id = Id;
    }


    public virtual string GetItemClass(){
        return "Undefined";
    }





}