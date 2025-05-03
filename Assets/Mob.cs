// using System.Linq;
// using UnityEngine;

// public class Mob : MonoBehaviour
// {
//     private Building       buildingAssignedTo;
//     private MaterialElement targetMaterial;
//     private GameObject      targetMaterialObject;

//     private Vector3 target;
//     private bool    isMoving        = false;
//     private bool    goingToMaterial = false;

//     // Delivery flag & cached name
//     private bool   incrementRequestByOne = false;
//     private string lastPickedMaterialName;

//     [SerializeField] private float speedFactor = 0.008f;

//     void Update()
//     {
//         if (isMoving)
//             MoveMobTo();
//     }

   
//     public void AssignedToBuilding(Building building)
//     {
//         // Remove from old building (if any) then add to new
//         if (buildingAssignedTo != null)
//             buildingAssignedTo.removeAssignedMob(gameObject);

//         buildingAssignedTo = building;
//         buildingAssignedTo.addAssignedMob(gameObject);

//         // Decide which resource to fetch next
//         AssignBuildingTask(building.GetBuildingClass());
//     }

//     public void RemoveAssignedBuilding()
//     {
//         if (buildingAssignedTo != null)
//         {
//             buildingAssignedTo.removeAssignedMob(gameObject);
//             buildingAssignedTo = null;
//         }
//     }


//     public void StartMoving(Vector3 destination, GameObject colliderObj)
//     {
//         isMoving = true;
//         target    = new Vector3(destination.x, transform.position.y, destination.z);

//         // If it’s a MaterialElement, head there
//         var mat = colliderObj.GetComponent<MaterialElement>();
//         if (mat != null)
//         {
//             targetMaterial       = mat;
//             targetMaterialObject = colliderObj;
//             goingToMaterial      = true;
//             return;
//         }

//         // Otherwise, if it’s a Building, assign yourself and prep the next resource
//         var bld = colliderObj.GetComponent<Building>();
//         if (bld != null)
//         {
//             AssignedToBuilding(bld);
//             goingToMaterial = false;
//         }


//         var tile = colliderObj.CompareTag("Tile");
//         if(tile){
//             targetMaterial = null;
//             targetMaterialObject = null;
//             goingToMaterial = false;
//             RemoveAssignedBuilding();
//             return;
//         }
//     }

//     private void MoveMobTo()
//     {



//         // if(!goingToMaterial && buildingAssignedTo == null && targetMaterial == null){
//         //     isMoving = false; 
//         // }

//         // Arrived?
//         if (Vector3.Distance(transform.position, target) < 0.1f)
//         {
//             isMoving = false;

            
//             // If we were bringing a material back, register delivery
//             if (goingToMaterial && targetMaterialObject != null)
//             {
//                 Debug.Log("We moved close to the material");
//                 lastPickedMaterialName  = targetMaterialObject
//                     .GetComponent<MaterialElement>()
//                     .GetMaterialName();
//                 //incrementRequestByOne   = true;
//                 targetMaterialObject    = null;
//                 targetMaterial          = null;
//                 goingToMaterial = false;

//                 StartMoving(
//                   buildingAssignedTo.transform.position,
//                   buildingAssignedTo.gameObject
//                 );
//             }
//             else if (!goingToMaterial && buildingAssignedTo != null)
//             {
//                 // === 2) Returned home: now increment once ===
//                 incrementRequestByOne = true;

//                 // prepare next outbound trip:
//                 goingToMaterial = true;
//                 AssignBuildingTask(buildingAssignedTo.GetBuildingClass());
//                 if (targetMaterial != null)
//                 {
//                     StartMoving(
//                       targetMaterial.transform.position,
//                       targetMaterial.gameObject
//                     );
//                 }
//             } 
//             return;
//         }

//         // Still en route
//         Vector3 dir = (target - transform.position).normalized;
//         transform.position += dir * speedFactor;
//     }

//     private void AssignBuildingTask(string buildingClass)
//     {
//         // Choose the right resource by building type
//         string matName = buildingClass switch
//         {
//             "Farming_house"  => "Gold",
//             "Military_house" => "Tree",
//             "Sleep_house"    => "Stone",
//             _                => null
//         };

//         if (matName != null)
//         {
//             targetMaterial = FindObjectsOfType<MaterialElement>()
//                 .Where(m => m.GetMaterialName() == matName)
//                 .OrderBy(m => Vector3.Distance(transform.position, m.transform.position))
//                 .FirstOrDefault();
//         }
//     }


//     public bool TryConsumeDelivery(out string materialType)
//     {
//         if (incrementRequestByOne)
//         {
//             Debug.Log($"TryConsumeDelivery() returning true for {name}, type={lastPickedMaterialName}");
//             materialType          = lastPickedMaterialName;
//             incrementRequestByOne = false;
//             return true;
//         }

//         materialType = null;
//         return false;
//     }
// }

