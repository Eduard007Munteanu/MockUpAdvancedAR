using UnityEngine;

public class BuildingDatabase: MonoBehaviour{
    
    public static BuildingDatabase Instance;

    

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