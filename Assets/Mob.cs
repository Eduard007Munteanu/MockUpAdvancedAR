using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Mob : MonoBehaviour
{
    // Start is called before the first frame update

    private Building buildingAssignedTo;

    private float speedFactor = 0.008f;

    private Vector3 target;

    private bool isMoving = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            moveMobTo();
        }
    }

    public void assignedToBuilding(Building building){
        buildingAssignedTo = building;
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
        if(buildingCheck){ 
            return;
        }
        removeAssignedBuilding();

    }
}
