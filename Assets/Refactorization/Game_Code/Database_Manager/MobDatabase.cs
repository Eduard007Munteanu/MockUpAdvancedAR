using UnityEngine;
using System.Collections.Generic;
public class MobDatabase: MonoBehaviour{
    
    public static MobDatabase Instance;


    void Awake()
    {
        if (Instance == null)  
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject); 
        }
    }
}