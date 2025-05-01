using UnityEngine;

public class DefaultPanel: MonoBehaviour, Panel{


    public  void Activate(){
        gameObject.SetActive(true);
    }

    public void Deactivate(){
        gameObject.SetActive(false);
    }

    public virtual void UpdatePanel(DataPacket dataPacketFromBuildingManager)
    {
       
    }

    

}