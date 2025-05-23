using UnityEngine;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting.Antlr3.Runtime;


public abstract class DefaultItem : MonoBehaviour, Item
{

    private int Id;

    protected virtual string ItemClass => "DefaultItem"; 

    // protected abstract ResourceType type = null;
    public abstract ResourceType Type { get; }

    public void Init(int Id){   
        this.Id = Id;
    }


    public virtual string GetItemClass(){
        return ItemClass;
    }
}