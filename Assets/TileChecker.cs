using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;


public class TileChecker : MonoBehaviour
{
    private GameObject lastHitTile;

    public Material defaultMat;
    public Material hoverMat;

    [SerializeField] private OVRHand rightHand;



    [SerializeField] private GameObject buildingPrefab;

    [SerializeField] private BuildingManager buildingManager;

    private GameObject currentMenu;

    private GameObject mainMenu;

    private Transform palmTransform;

    [SerializeField] private GameObject buildCanvasPrefab;

    [SerializeField] private GameObject mainBuildCanvasPrefab;

    [SerializeField] private OVRSkeleton leftHandSkeleton;

    private bool wasPinching = false;

    [SerializeField] private GameObject mob;

    [SerializeField] private GameObject mobManagerPrefab;

    private bool spawnListenerAdded = false;

    private Button spawnButton;

    private Mob selectedMob = null;

    




    void Start()
    {
        foreach (var bone in leftHandSkeleton.Bones)
        {
            if (bone.Id.ToString().Contains("Palm"))
            {
                palmTransform = bone.Transform;
                break;
            }
        }     

        
    }

    void Update(){
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        float specifictileHeight = GameObject.FindWithTag("Tile").GetComponent<Renderer>().bounds.size.y;

        

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hit.collider.CompareTag("Tile"))
            {
                if (hitObj != lastHitTile)
                {
                    ResetLastTile();

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

                GameObject tile = hitObj;

                
                Renderer currentRend = hitObj.GetComponent<Renderer>();
                if (CardDeck.cardMaterialInLeftHand != null && currentRend != null)
                {
                    float pinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                    if (pinchStrength > 0.8f && hitObj.GetComponent<TileObject>().CheckIfCanSpawn())
                    {
                        
                        Debug.Log("Card applied to tile!");


                        Vector3 tilePosition = hitObj.transform.position;
                        float tileHeight = hitObj.GetComponent<Renderer>().bounds.size.y;
                        float buildingHeight = buildingPrefab.GetComponent<Renderer>().bounds.size.y;

                        Vector3 spawnPosition = tilePosition + Vector3.up * ((tileHeight + (buildingHeight / 2f))  / 1f);

                        

                        
                        GameObject newBuilding = Instantiate(buildingPrefab, spawnPosition, Quaternion.identity);
                        Building building_local = newBuilding.GetComponent<Building>();
                        if (building_local != null)
                        {
                            string buildingClass = CardDeck.selectedCardClass;
                            buildingManager.AddBuildingCountToSpecificClass(buildingClass, building_local);
                            building_local.SetMaterial(CardDeck.cardMaterialInLeftHand);
                            building_local.Initialization(buildingManager.SetBuildingID(buildingClass), buildingClass, tile);
                            
                        }


                        if (CardDeck.cardInLeftHand != null)
                        {
                            CardDeck.cardInLeftHand.DestroySelf();
                            CardDeck.cardInLeftHand = null;
                            CardDeck.cardMaterialInLeftHand = null;
                            CardDeck.cardSpriteInLeftHand = null;
                            CardDeck.selectedCardClass = null;
                        }

                        //!!!! Potential incorrect
                        CardDeck.ClearHand();
                        //!!!! Potential incorrect
                        
                        
                        

                    }
                }
            }

            Building building = hit.collider.GetComponent<Building>();
            if (building != null && selectedMob == null)
            {
                float pinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                bool isPinchingNow = pinchStrength > 0.8f;
                if (isPinchingNow && !wasPinching)
                {
                    if(building.GetBuildingClass() != null && building.GetBuildingClass() != "MainBuilding"){    // Here regarding having the mob following the building. 
                        if(currentMenu == null){
                            currentMenu = Instantiate(buildCanvasPrefab, palmTransform.position, palmTransform.rotation);
                            currentMenu.transform.SetParent(palmTransform, worldPositionStays: true);

                            Canvas canvas = currentMenu.GetComponent<Canvas>();
                            if (canvas != null)
                            {
                                canvas.worldCamera = Camera.main; 
                            }


                            var statsTransform = currentMenu.transform.Find("Layout_Stats");
                            Debug.Log(statsTransform ? $"Found Layout_Stats: {statsTransform.name}" : "Layout_Stats not found!");


                            var classTransform = statsTransform.Find("Text (Building Class Placeholder)");
                            var idTransform    = statsTransform.Find("Text (Building ID Placeholder)");

                            if (classTransform == null)
                            {
                                Debug.LogError("Couldn’t find the Building Class Placeholder under Layout_Stats!");
                            }
                            if (idTransform == null)
                            {
                                Debug.LogError("Couldn’t find the Building ID Placeholder under Layout_Stats!");
                            }

                            // Now safely fetch the Text components
                            TextMeshProUGUI buildingClassText = classTransform?.GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI buildingIdText    = idTransform?.GetComponent<TextMeshProUGUI>();

                            if (buildingClassText != null)
                                buildingClassText.text = building.GetBuildingClass();
                            else
                                Debug.LogError("Missing TextMeshProUGUI on Building Class Placeholder!");

                            if (buildingIdText != null)
                                buildingIdText.text = building.GetID().ToString();
                            else
                                Debug.LogError("Missing TextMeshProUGUI on Building ID Placeholder!");

                            


                            Debug.Log("buildingClassText " + buildingClassText.text + " buildingIdText " + buildingIdText.text);


                            building.ReactToPinch();  
                        }
                        else{
                            Destroy(currentMenu);
                            currentMenu = null;
                        }    
                    } 
                    else if(building.GetBuildingClass() == "MainBuilding") {
                        if(mainMenu == null){
                            mainMenu = Instantiate(mainBuildCanvasPrefab, palmTransform.position, palmTransform.rotation);
                            mainMenu.transform.SetParent(palmTransform, worldPositionStays: true);

                            MainBuildingCanvas contentManager = mainMenu.GetComponent<MainBuildingCanvas>();


                            // Debug.Log("Content manager: " + contentManager);
                            // if (contentManager != null)
                            // {
                                
                            //     foreach (var mobs in FindObjectsOfType<Mob>())
                            //     {
                            //         Debug.Log("Mob: " + mobs);
                            //         bool check = mobs.getIncrementRequestByOne();
                            //         Debug.Log("Check: " + check);
                            //         if(check){
                            //             Debug.Log("Increment request by one!");
                            //             contentManager.UpdateScore(1, mobs.TargetMaterialObjectFinder().GetComponent<MaterialElement>().GetMaterialName());
                            //         }
                            //     }
                            // }
                            // else
                            // {
                            //     Debug.LogError("CanvasContentManager not found on the canvas prefab!");
                            // }
                            

                            Canvas canvas = mainMenu.GetComponent<Canvas>();
                            if (canvas != null)
                            {
                                canvas.worldCamera = Camera.main; 
                            }


                            var statsTransform = mainMenu.transform.Find("Layout_Stats");
                            Debug.Log(statsTransform ? $"Found Layout_Stats: {statsTransform.name}" : "Layout_Stats not found!");


                            var classTransform = statsTransform.Find("Text (Building Class Placeholder)");
                            var idTransform    = statsTransform.Find("Text (Building ID Placeholder)");

                            if (classTransform == null)
                            {
                                Debug.LogError("Couldn’t find the Building Class Placeholder under Layout_Stats!");
                            }
                            if (idTransform == null)
                            {
                                Debug.LogError("Couldn’t find the Building ID Placeholder under Layout_Stats!");
                            }

                            // Now safely fetch the Text components
                            TextMeshProUGUI buildingClassText = classTransform?.GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI buildingIdText    = idTransform?.GetComponent<TextMeshProUGUI>();

                            if (buildingClassText != null)
                                buildingClassText.text = building.GetBuildingClass();
                            else
                                Debug.LogError("Missing TextMeshProUGUI on Building Class Placeholder!");

                            if (buildingIdText != null)
                                buildingIdText.text = building.GetID().ToString();
                            else
                                Debug.LogError("Missing TextMeshProUGUI on Building ID Placeholder!");

                            


                            Debug.Log("buildingClassText " + buildingClassText.text + " buildingIdText " + buildingIdText.text);


                            building.ReactToPinch();  



                            //var buttonTransform = currentMenu.transform.Find("Button (Spawn Mobs)");
                            var buttonTransform = mainMenu.transform.Find("Modifications/Button (Spawn Mobs)"); // Adjusted path to find the button

                            if (buttonTransform == null)
                            {
                                Debug.LogError("Couldn't find Button (Spawn Mobs)!");
                            }
                            else
                            {
                                Debug.Log("Found Button (Spawn Mobs)!, called : " + buttonTransform.name);
                                /* Button */ spawnButton = buttonTransform.GetComponent<Button>();
                                if (spawnButton == null)
                                {
                                    Debug.LogError("Missing Button component on Button (Spawn Mobs)!");
                                }
                                else
                                {
                                    if (!spawnListenerAdded)
                                    {
                                        spawnButton.onClick.RemoveAllListeners();
                                        spawnButton.onClick.AddListener(() =>
                                        {
                                            Debug.Log("Spawn Mob button clicked by pinch!");
                                            mobManagerPrefab.GetComponent<Mob_Manager>().createMob(building, specifictileHeight); 
                                        });

                                        spawnListenerAdded = true;
                                    }

                                    Debug.Log("We are close to the raycasting of the button zone!");

                                    

                                }
                            }
                        }
                        else{
                            //mainMenu.SetActive(!mainMenu.activeSelf);
                            
                            //Destroy(currentMenu);
                            //currentMenu = null;

                            var cv = mainMenu.GetComponent<Canvas>();
                            cv.enabled = !cv.enabled;
                            // spawnListenerAdded = false;
                            // spawnButton = null;
                        }
                    }



                    Debug.Log("Pinching while gazing at a building!");

                    
                }

                wasPinching = isPinchingNow;
            }

           


            if (spawnButton != null){
                Ray uiRay = new Ray(Camera.main.transform.position,
                                Camera.main.transform.forward);
                Debug.DrawRay(uiRay.origin, uiRay.direction * 2f, Color.red);

                RaycastHit uiHit;
                if (Physics.Raycast(uiRay, out uiHit, 100f))
                {
                    

                    if (uiHit.collider.transform.IsChildOf(spawnButton.transform))
                    {
                        Debug.Log("Gazing at Spawn Mobs button!");
                        if (rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.8f)
                            spawnButton.onClick.Invoke();
                    }
                }
            }
            



            Card card = hit.collider.GetComponent<Card>();
            if(card != null && card.GetMyInHandId() != -1){
                float pinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                bool isPinchingNow = pinchStrength > 0.8f;
                if (isPinchingNow && !wasPinching){
                    Debug.Log("Pinching while gazing at a card!");
                    CardDeck.SelectCard(card);

                }

                wasPinching = isPinchingNow;
            }



            Mob mob = hit.collider.GetComponent<Mob>();
            if(mob != null){
                float pinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                bool isPinchingNow = pinchStrength > 0.8f;
                if (isPinchingNow /*&& !wasPinching*/){
                    Debug.Log("Pinching while gazing at a mob!");
                    selectedMob = mob;
                }   
            }

            if(selectedMob != null && mob == null && hit.collider != null && hit.collider.CompareTag("Tile")){    //If you gaze away from the mob, you can move it around + movable only on tile 
                float pinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                bool isPinchingNow = pinchStrength > 0.8f;
                if (isPinchingNow /*&& !wasPinching*/){
                    Debug.Log("Pinching while gazing away from a mob!");
                    if(!hit.collider.GameObject()){
                        Debug.LogError("Hit.collider for mob target movement is null!");
                    }
                    selectedMob.StartMoving(hit.point, hit.collider.GameObject());
                    selectedMob = null;
                }   
                
            }


            //This part is incomplete. 
            if(selectedMob != null && mob == null && building != null){
                float pinchStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                bool isPinchingNow = pinchStrength > 0.8f;
                if (isPinchingNow /*&& !wasPinching*/){
                    Debug.Log("Pinch at the building");
                    Debug.Log("Building transform position: " + building.transform.position);
                    Debug.Log("Building gameobject" + building.gameObject);
                    Debug.Log("Building class: " + building.GetBuildingClass());
                    selectedMob.StartMoving(building.transform.position, building.gameObject);  //at the center of the building, not quite correct. 
                    selectedMob = null;
                }   
            }


        }
    }



    void ResetLastTile()
    {
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



    

}
