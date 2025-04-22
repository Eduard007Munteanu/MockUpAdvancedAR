using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialElement : MonoBehaviour
{


    private Color color; 

    //Probably have different material sizes, but not for now.
    //private float materialsize; 

    private string materialClass;

    private string ID;

    private GameObject onTile; 




    public void Init(Color color,  string ID, GameObject onTile){
        this.color = color;
        this.GetComponent<Renderer>().material.color = color; 
        this.materialClass = GetMaterialName(); //TODO: Change to the correct name of the material.
        this.ID = ID;
        this.onTile = onTile;
        warnTile();
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getTile(){
        return onTile; 
    }

    public void warnTile(){
        onTile.GetComponent<TileObject>().NoSpawning(); 
    }


    string GetMaterialName(){
        

        if (color.Equals(Color.yellow))
        {
            return "Gold";
        }
        else if (color.Equals(Color.green))
        {
            return "Tree";
        }
        else if (color.Equals(Color.gray))
        {
            return "Stone";
        }
        else
        {
            throw new System.ArgumentException($"Invalid color value: {color}");
        }
        
    }


    

    
}
