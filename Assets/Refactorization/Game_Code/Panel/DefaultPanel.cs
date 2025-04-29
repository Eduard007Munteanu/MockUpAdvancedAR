using UnityEngine;

public class DefaultPanel: MonoBehaviour{

    [SerializeField] private GameObject leftHand;


    private Transform palmTransform;

    protected virtual  void Start()
    {
        foreach (var bone in leftHand.GetComponent<OVRSkeleton>().Bones)
        {
            if (bone.Id.ToString().Contains("Palm"))
            {
                palmTransform = bone.Transform;
                break;
            }
        }
        SetPanelPositionToLeftHand();     
    }

    private void SetPanelPositionToLeftHand(){
        transform.position = palmTransform.transform.position;
    }


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