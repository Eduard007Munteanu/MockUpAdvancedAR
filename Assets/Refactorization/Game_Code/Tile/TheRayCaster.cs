using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class TheRayCaster : MonoBehaviour
{
    
    [SerializeField] private OVRHand rightHand; //Maybe just GameObject, then specific OVRHand?


    [SerializeField] private PanelManager panelManager;

    private GameObject lastHitTile;

    private DefaultMob selectedMob;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InitiateRaycast();

    }

    



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
                if(CardsInHand.Instance.GetCardsInHand().Count == 1){
                    if(rightHandPinchStrength > 0.8f){
                        if(tile != null){
                            BuildManager.Instance.TrySpawnBuilding(tile, CardsInHand.Instance.GetCardsInHand()[0]);
                        }
                    }

                }
            }

            else if(card != null && CardsInHand.Instance.IsCardInHand(card)){
                if(rightHandPinchStrength > 0.8f){
                    CardsInHand.Instance.RemoveAllCardsExpect(card);
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
                        Vector3 tilePosition = tile.gameObject.transform.position;  
                        Vector3 targetPosition = new Vector3(tilePosition.x, vectorYHeightGivenTile(tile, selectedMob), tilePosition.z);
                        selectedMob.InitMove(targetPosition, hitObj);
                    }
                    else if(building != null){
                        Vector3 buildingPosition = building.gameObject.transform.position;  
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
