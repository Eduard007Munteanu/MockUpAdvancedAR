using UnityEngine;
using System.Linq;


public class JustGoBehavior : IMobBehavior
{

    private DefaultMob mob;

    public void ActionLoop()
    {
        if(mob.isMoving){
            Move();
        }
    }

    public void Init(DefaultMob mob)
    {
        this.mob = mob;
    }

    public void InitMove(Vector3 destination, GameObject colliderObj)
    {
        mob.isMoving = true;
        mob.toDestination = destination;
        mob.toColliderObj = colliderObj;
    }

    public void Move(){

        

        Vector3 dir = (mob.toDestination - mob.transform.position).normalized;
        mob.transform.position += dir * mob.speedFactor;

        


        if(Vector3.Distance(mob.transform.position, mob.toDestination) < 0.1f){
            Debug.Log("Reached the closest point on tile to my DefaultMob object");
            mob.isMoving = false;
            // transform.position = toDestination;

            if (mob.toColliderObj.GetComponent<DefaultBuild>() != null)
            {
                Debug.Log("DefaultBuild from JustGoBehavior is here");
                mob.toColliderObj.GetComponent<DefaultBuild>().GetTile().ArrangeMobsGivenPressBuild(mob, mob.toColliderObj.GetComponent<DefaultBuild>());  
            }

            if (mob.toColliderObj.GetComponent<DefaultTile>() != null)
            {
                Debug.Log("DefaultTile from JustGoBehavior is here");
                mob.toColliderObj.GetComponent<DefaultTile>().ArrangeMobs(mob);    
            }

            
            
            // I want to set the mob in a  specific order regarding the tile, involving taking into account possible other mobs from the same tile. 
        }
    }

    public void OnClick()
    {
        mob.isMoving = false;
    }
}










