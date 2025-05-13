using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;

public class TheRayCaster : MonoBehaviour
{

    [SerializeField] private OVRHand rightHand; //Maybe just GameObject, then specific OVRHand?




    private GameObject lastHitTile;

    private DefaultMob selectedMob;


    private bool wasPinching = false;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        InitiateRaycast();

    }





    void InitiateRaycast()
    {

        float rightHandPinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);


        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;




        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObj = hit.collider.gameObject;


            DefaultCard card = hit.collider.GetComponent<DefaultCard>();
            DefaultBuild building = hit.collider.GetComponent<DefaultBuild>();
            DefaultMob mobs = hit.collider.GetComponent<DefaultMob>();
            DefaultTile tile = hit.collider.GetComponent<DefaultTile>();

            MobSpawnButton mobButton = hit.collider.GetComponent<MobSpawnButton>();


            Debug.Log("SelectedMob is " + (selectedMob?.name ?? "null") + " And mob is " + (mobs?.name ?? "null"));






            if (selectedMob != null && mobs == null && !wasPinching)
            {
                Debug.Log("Allright!");
                if (rightHandPinchStrength > 0.8f)
                {
                    Debug.Log("Even better than allright!");
                    if (tile != null)
                    {
                        Debug.Log("We selected the selectedMob to the tile");
                        Vector3 tilePosition = tile.gameObject.transform.position;
                        Vector3 targetPosition = new Vector3(tilePosition.x, vectorYHeightGivenTile(tile, selectedMob), tilePosition.z);  //Pivot point in empty object parent of tile instead of vectorYHeightGivenTile
                        selectedMob.RemoveFromBuilding();
                        //selectedMob.AssignToBuilding(); // Here we will have the military building assignment. 

                        bool canTheMobBeAdded = tile.CanMobBeArrangedChecker(selectedMob);
                        if (canTheMobBeAdded)
                        {
                            selectedMob.SetBehaviorBasedOnBuilding(null);
                            selectedMob.InitMove(targetPosition, hitObj);
                        }
                        selectedMob = null;
                    }
                    else if (building != null)
                    {
                        Debug.Log("We selected the selectedMob to the building of type " + building.name);
                        Vector3 buildingPosition = building.gameObject.transform.position;
                        Vector3 targetPosition = new Vector3(buildingPosition.x, vectorYHeightGivenTile(BetterGridOverlay.Instance.GetTiles()[0].GetComponent<DefaultTile>(), selectedMob), buildingPosition.z);
                        selectedMob.RemoveFromBuilding();
                        selectedMob.AssignToBuilding(building);
                        selectedMob.SetBehaviorBasedOnBuilding(building);
                        selectedMob.InitMove(targetPosition, hitObj);
                        selectedMob = null;
                    }
                }
            }

            if (tile != null && !wasPinching)
            {
                GlowEffectTrigger(hitObj);
                if (CardsInHand.Instance.GetCardsInHand().Count == 1)
                {
                    if (rightHandPinchStrength > 0.8f)
                    {
                        if (tile != null)
                        {
                            BuildManager.Instance.TrySpawnBuilding(tile, CardsInHand.Instance.GetCardsInHand()[0]);
                            CardsInHand.Instance.RemoveAllCards();
                        }
                    }

                }
            }

            if (card != null && CardsInHand.Instance.IsCardInHand(card) && !wasPinching)
            {
                if (rightHandPinchStrength > 0.8f
                   && card.TryBuyCard())
                {
                    CardsInHand.Instance.RemoveAllCardsExpect(card);
                }
            }

            if (building != null && !wasPinching)
            {    //Only spawn once per pinch modification. 

                if (rightHandPinchStrength > 0.8f && !wasPinching)
                {
                    PanelManager.Instance.SpawnPanelOnLeftHand(building);
                }

            }

            if (mobButton != null && rightHandPinchStrength > 0.8f && !wasPinching)
            {
                mobButton.TriggerMobSpawn();
            }


            if (mobs != null && !wasPinching)
            {
                if (rightHandPinchStrength > 0.8f)
                {
                    selectedMob = mobs;
                    Debug.Log("We look and pinched at a mob, nice :) . selectedMob = mobs  , more concrete selectedMob = " + selectedMob.name);
                    selectedMob.ReactOnClick();
                }
            }


        }
        wasPinching = rightHandPinchStrength > 0.8f;
    }




    void GlowEffectTrigger(GameObject hitObj)
    {
        if (hitObj != lastHitTile)
        {
            GlowEffectReset();

            Renderer rend = hitObj.GetComponent<Renderer>();
            if (rend != null)
            {
                HoverEffects hoverEffect = hitObj.GetComponent<HoverEffects>();
                if (hoverEffect != null)
                {
                    hoverEffect.EnableEffect();
                }
            }

            lastHitTile = hitObj;
        }
    }

    void GlowEffectReset()
    {
        if (lastHitTile != null)
        {
            Renderer rend = lastHitTile.GetComponent<Renderer>();
            if (rend != null)
            {
                HoverEffects hoverEffect = lastHitTile.GetComponent<HoverEffects>();
                if (hoverEffect != null)
                {
                    hoverEffect.DisableEffect();
                }

            }
            lastHitTile = null;
        }
    }



    float vectorYHeightGivenTile(DefaultTile tile, DefaultMob mob)
    {
        Vector3 tilePosition = tile.gameObject.transform.position;

        float tileHeight = tile.GetTileHeight();
        float mobHeight = mob.GetMobHeight();

        float height = tilePosition.y + ((tileHeight + (mobHeight / 2f)) / 1f);
        return height;
    }











}
