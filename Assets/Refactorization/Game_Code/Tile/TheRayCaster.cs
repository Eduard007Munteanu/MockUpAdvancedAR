using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class TheRayCaster : MonoBehaviour
{
    // Start is called before the first frame update

    // private Transform palmTransform;

    // [SerializeField] private GameObject leftHand;

    [SerializeField] private OVRHand rightHand; //Maybe just GameObject, then specific OVRHand?

    // [SerializeField] private DefaultTile tilePrefab; //Maybe abstract class instead of interface?

    [SerializeField] private CardsInHand cardsInhand;

    

    //[SerializeField] private GameObject buildPrefab; //Well, what kind of building? Interface based? 

    

    [SerializeField] private PanelManager panelManager;

    private GameObject lastHitTile;

    private DefaultMob selectedMob;



    void Start()
    {
        // GetPalmPosition();
    }

    // Update is called once per frame
    void Update()
    {
        InitiateRaycast();

    }

    // void GetPalmPosition(){

    //     OVRSkeleton leftHandSkeleton = leftHand.GetComponent<OVRSkeleton>();

    //     foreach (var bone in leftHandSkeleton.Bones)
    //     {
    //         if (bone.Id.ToString().Contains("Palm"))
    //         {
    //             palmTransform = bone.Transform;
    //             break;
    //         }
    //     }  
    // }



    void InitiateRaycast(){

        float rightHandPinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);


        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        


        if(Physics.Raycast(ray, out hit, 100f)){
            GameObject hitObj = hit.collider.gameObject;


            DefaultCard card = hit.collider.GetComponent<DefaultCard>();
            DefaultBuild building = hit.collider.GetComponent<DefaultBuild>();
            DefaultMob mobs = hit.collider.GetComponent<DefaultMob>();
            DefaultTile tile = hit.collider.GetComponent<DefaultTile>();




            if(tile != null){
                GlowEffectTrigger(hitObj);
                if(cardsInhand.GetCardsInHand().Count == 1){
                    if(rightHandPinchStrength > 0.8f){
                        if(tile != null){
                            BuildManager.Instance.TrySpawnBuilding(tile, cardsInhand.GetCardsInHand()[0]);
                        }
                    }

                }
            }

            else if(card != null && cardsInhand.IsCardInHand(card)){
                if(rightHandPinchStrength > 0.8f){
                    cardsInhand.RemoveAllCardsExpect(card);
                }
            }

            else if(building != null){
                if(rightHandPinchStrength > 0.8f){
                    panelManager.SpawnPanelOnLeftHand(building);
                }
            }

            else if(mobs != null){
                if(rightHandPinchStrength > 0.8f){
                    selectedMob = mobs;
                    selectedMob.ReactOnClick();
                }
            }

            else if(selectedMob != null && mobs == null){
                if(rightHandPinchStrength > 0.8f){
                    if(tile != null){
                        Vector3 tilePosition = tile.gameObject.transform.position;  //Monobehavior probably guaranteed. Need to check
                        Vector3 targetPosition = new Vector3(tilePosition.x, vectorYHeightGivenTile(tile, selectedMob), tilePosition.z);
                        selectedMob.InitMove(targetPosition, hitObj);
                    }
                    else if(building != null){
                        Vector3 buildingPosition = building.gameObject.transform.position;  //Monobehavior probably guaranteed. Need to check
                        Vector3 targetPosition = new Vector3(buildingPosition.x, vectorYHeightGivenTile(tile, selectedMob), buildingPosition.z);
                        selectedMob.InitMove(targetPosition, hitObj);
                    }
                }
            }
        }
    }




    void GlowEffectTrigger(GameObject hitObj){
        if(hitObj != lastHitTile){
            GlowEffectReset();

            Renderer rend = hitObj.GetComponent<Renderer>();
            if (rend != null)
            {
                HoverEffect hoverEffect = hitObj.GetComponent<HoverEffect>();
                if (hoverEffect != null)
                {
                    hoverEffect.EnableEffect();
                }
            }

            lastHitTile = hitObj; 
        }
    }

    void GlowEffectReset(){
        if (lastHitTile != null)
        {
            Renderer rend = lastHitTile.GetComponent<Renderer>();
            if (rend != null)
            {
                HoverEffect hoverEffect = lastHitTile.GetComponent<HoverEffect>();
                if (hoverEffect != null)
                {
                    hoverEffect.DisableEffect();
                }

            }
            lastHitTile = null;
        }
    }

    

     float vectorYHeightGivenTile(DefaultTile tile, DefaultMob mob){
        Vector3 tilePosition = tile.gameObject.transform.position;

        float tileHeight = tile.GetTileHeight();
        float mobHeight = mob.GetMobHeight();

        float height = tilePosition.y + ((tileHeight + (mobHeight / 2f))  / 1f);
        return height;
     }

     

    







}
