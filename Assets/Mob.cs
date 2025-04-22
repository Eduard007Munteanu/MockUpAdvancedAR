using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class Mob : MonoBehaviour
{
    // Start is called before the first frame update

    private Building buildingAssignedTo;

    private float speedFactor = 0.008f;

    private Vector3 target;

    private bool isMoving = false;

    //VERY VERY STUPID

    private MaterialElement targetMaterial;

    private bool goingToMaterial = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            moveMobTo();
        }
        else{
            if(targetMaterial != null){
                if(goingToMaterial){
                    target = targetMaterial.transform.position;
                    Vector3 p = target;
                    p.y = transform.position.y;
                    target= p;  
                    moveMobTo();
                }
                else if(!goingToMaterial){
                    target = buildingAssignedTo.transform.position;
                    Vector3 p = target;
                    p.y = transform.position.y;
                    target= p;  
                    moveMobTo();
                }
            }
        }
        


    }

    public void assignedToBuilding(Building building){
        buildingAssignedTo = building;
        Debug.Log("Assigned to building with class: " + building.GetBuidlingClass()); // <== Add this
        AssignBuildingTask(building.GetBuidlingClass());
        building.addAssignedMob(this.gameObject);
    }

    public void removeAssignedBuilding(){
        buildingAssignedTo.removeAssignedMob(this.gameObject);
        buildingAssignedTo = null;
    }

    public void moveMobTo(){
        Vector3 mobPosition = transform.position;
        if(Vector3.Distance(mobPosition, target) < 0.1f){
            isMoving = false;
            goingToMaterial = !goingToMaterial;
            return;
        }
        Vector3 direction = target - mobPosition;
        direction.Normalize();
        //transform.rotation = Quaternion.LookRotation(direction);  To look at the correct direction when moving, but not for now, since we only have cubes
        Vector3 newPosition = mobPosition + direction * speedFactor;
        transform.position = newPosition;
        
    }

    public void startMoving(Vector3 targetTo, GameObject colliderObject){
        isMoving = true;
        Vector3 same_y_target = new Vector3(targetTo.x, transform.position.y, targetTo.z);
        target = same_y_target; 


        //Debug only part:

        Debug.Log("Target position vector value: " + target); //Test
        Debug.Log("Mob position vector value: " + transform.position); //Test


        bool buildingCheck = colliderObject.GetComponent<Building>() == null;
        Debug.Log("BuildingCheck is " + buildingCheck); //Test
        if(buildingCheck){ 
            return;
        }
        removeAssignedBuilding();
        assignedToBuilding(colliderObject.GetComponent<Building>());

    }

    public void AssignBuildingTask(string buildingClass){

        Debug.Log($"Assigning task for {buildingClass}");

        if(buildingClass == "Farming_house"){  //Farming task 
            Debug.Log("Farming house task");
            targetMaterial = FindingClosestMaterial("Gold");
            if(targetMaterial != null){
                Vector3 p = targetMaterial.transform.position;
                p.y = transform.position.y;
                targetMaterial.transform.position = p;    
            }
            Debug.Log("Target material: " + targetMaterial); //Test
        }
        else if(buildingClass == "Military_house"){ //Military task
            Debug.Log("Military house task");
            targetMaterial = FindingClosestMaterial("Tree");
            if(targetMaterial != null){
                Vector3 p = targetMaterial.transform.position;
                p.y = transform.position.y;
                targetMaterial.transform.position = p;    
            }
            Debug.Log("Target material: " + targetMaterial); //Test
        } 
        else if(buildingClass == "Sleep_house"){  //Sleep task
            Debug.Log("Sleep house task");
            targetMaterial = FindingClosestMaterial("Stone");
            if(targetMaterial != null){
                Vector3 p = targetMaterial.transform.position;
                p.y = transform.position.y;
                targetMaterial.transform.position = p;    
            }
            Debug.Log("Target material: " + targetMaterial); //Test
        }
    }



    public MaterialElement FindingClosestMaterial(string materialClass){
        // MaterialElement[] allMaterials = FindObjectsOfType<MaterialElement>();
        // MaterialElement[] filteredMaterial = null; 
        // for(int i= 0; i < allMaterials.Length; i++){
        //     if(allMaterials[i].GetMaterialName() == materialClass){
        //         filteredMaterial.Append(allMaterials[i]);
        //     }
        // }

        // MaterialElement closest = null;
        // float minDistance = Mathf.Infinity;
        // Vector3 currentPos = transform.position;

        // foreach (MaterialElement mat in filteredMaterial)
        // {
        //     float dist = Vector3.Distance(currentPos, mat.transform.position);
        //     if (dist < minDistance)
        //     {
        //         minDistance = dist;
        //         closest = mat;
        //     }
        // }
        // return closest;
        var filtered = FindObjectsOfType<MaterialElement>()
                    .Where(m => m.GetMaterialName() == materialClass)
                    .ToArray();

        Debug.Log($"Found {filtered.Length} '{materialClass}' materials");

        return filtered
            .OrderBy(m => Vector3.Distance(transform.position, m.transform.position))
            .FirstOrDefault();
    }
}
