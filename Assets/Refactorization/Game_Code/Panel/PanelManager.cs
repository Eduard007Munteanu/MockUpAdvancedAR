using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{

    private Transform palmTransform;

    [SerializeField] private GameObject leftHand;

    Dictionary<string, (GameObject, bool)> panels = new Dictionary<string, (GameObject, bool)>(); //List of panels that are spawned.


    // Start is called before the first frame update
    void Start()
    {
        GetPalmPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPanelOnLeftHand(Build panelFromBuilding)
    {


        string BuildingClassName = panelFromBuilding.GetBuildingClass();


        if(panels.ContainsKey(BuildingClassName)){
            if (panels[BuildingClassName].Item2 == true) //If the panel is already active, deactivate it. 
            {
                DeactivatePanel(panels[BuildingClassName].Item1);
                return;
            }
            else //If the panel is not active, activate it. 
            {
                ActivatePanel(panels[BuildingClassName].Item1);
                return;
            }
        }

        

        Vector3 spawnPosition = palmTransform.position + palmTransform.forward * 0.1f; 

        GameObject panel = Instantiate(panelFromBuilding.GetPanelPrefab(), spawnPosition, Quaternion.identity, leftHand.transform);
        panel.SetActive(false);    //Have panel to begin with, but innactive. 
 

        panel.transform.localPosition = Vector3.zero; 
        panel.transform.localRotation = Quaternion.identity;


        panels.Add(BuildingClassName, (panel, true)); 
    }

    public void DeactivatePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false); // Deactivate the panel
        }
    }

    public void ActivatePanel(GameObject panel){
        if (panel != null)
        {
            panel.SetActive(true); // Activate the panel
        }
    }

    void GetPalmPosition(){

        OVRSkeleton leftHandSkeleton = leftHand.GetComponent<OVRSkeleton>();

        foreach (var bone in leftHandSkeleton.Bones)
        {
            if (bone.Id.ToString().Contains("Palm"))
            {
                palmTransform = bone.Transform;
                break;
            }
        }  
    }
}
