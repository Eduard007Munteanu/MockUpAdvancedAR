using UnityEngine;

public class TouchingSystem : MonoBehaviour{



    void Start(){

    }


    void Update(){

    }


     private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touched: " + other.name);
    }


    


}