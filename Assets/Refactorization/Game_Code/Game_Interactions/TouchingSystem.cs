using Unity.VisualScripting;
using UnityEngine;
using System.Collections; // Added for IEnumerator

// it triggers selection multiple times. pickup -> assign too many times 
// colliding with the same tile which the mob stands on ignore same with tile and house when assigning destination
// it only recognizes tiles and mobs and buildings
// area of the finger is small or big enough for ease of use

public class TouchingSystem : MonoBehaviour{


    [SerializeField] private LayerMask interactableLayer;

    private GameObject currentlyTouching; 


    private DefaultMob selectedMob; 

    private bool isSelectionCooldownActive = false; // Added for delay management

    void Start(){

    }


    void Update(){

    }


     private void OnTriggerEnter(Collider other) {
        if (isSelectionCooldownActive) { // Check if cooldown is active
            return;
        }

        GameObject go = other.gameObject;


        if((interactableLayer.value & (1 << go.layer)) == 0){
            return;
        }

        Debug.Log("Touched: " + go.name);

        if (currentlyTouching == null || (go != currentlyTouching)) {
            currentlyTouching = go;
            Checker(go);
        }
        else {
            Debug.Log("Same object re-triggered");
        }
    }


    private void OnTriggerExit(Collider other) {
    if (other.gameObject == currentlyTouching) {
        currentlyTouching = null;
    }
    }



    public void Checker(GameObject gameObject){
        DefaultMob mob = gameObject.GetComponent<DefaultMob>();
        DefaultTile tile = gameObject.GetComponent<DefaultTile>();
        DefaultBuild build = gameObject.GetComponent<DefaultBuild>();


    


        if(mob != null && selectedMob == null){
            ActionGivenDefaultMob(mob);
        } 
        else if(selectedMob != null && tile != null){
            ActionMoveMobToTile(tile);
        }
        else if(selectedMob != null && build != null){
            ActionMoveMobToBuilding(build);
        }

    }


    private void ActionGivenDefaultMob(DefaultMob defaultMobTouched){
        selectedMob = defaultMobTouched;
        AudioManager.Instance.PlaySoundEffect(); // Audio plays here
        StartCoroutine(SelectionCooldownCoroutine()); // Start cooldown
    }


    public void ActionMoveMobToTile(DefaultTile defaultTileTouched){
        Debug.Log("OnTriggerEnter in ActionMoveMobToTile");
        Vector3 tilePosition = defaultTileTouched.gameObject.transform.position;  
        Vector3 targetPosition = new Vector3(tilePosition.x, selectedMob.transform.position.y, tilePosition.z);  // Use selectedMob's current Y

        selectedMob.RemoveFromBuilding();

        bool canTheMobBeAdded = defaultTileTouched.CanMobBeArrangedChecker();
        if(canTheMobBeAdded){
            selectedMob.SetBehaviorBasedOnBuilding(null);
            selectedMob.InitMove(targetPosition, defaultTileTouched.gameObject);
        }
        selectedMob = null;
    }


    public void ActionMoveMobToBuilding(DefaultBuild defaultBuildTouched){
        Debug.Log("OnTriggerEnter in ActionMoveMobToBuilding");
        Vector3 buildingPosition = defaultBuildTouched.gameObject.transform.position;  
        Vector3 targetPosition = new Vector3(buildingPosition.x, vectorYHeightGivenTile(BetterGridOverlay.Instance.GetTiles()[0].GetComponent<DefaultTile>(), selectedMob), buildingPosition.z);
        selectedMob.RemoveFromBuilding();
        selectedMob.AssignToBuilding(defaultBuildTouched);
        selectedMob.SetBehaviorBasedOnBuilding(defaultBuildTouched);
        selectedMob.InitMove(targetPosition, defaultBuildTouched.gameObject);
        selectedMob = null;
    }



    float vectorYHeightGivenTile(DefaultTile tile, DefaultMob mob){
        Vector3 tilePosition = tile.gameObject.transform.position;

        float tileHeight = tile.GetTileHeight();
        float mobHeight = mob.GetMobHeight();

        float height = tilePosition.y + ((tileHeight + (mobHeight / 2f))  / 1f);
        return height;
     }


    // Added coroutine for selection delay
    private IEnumerator SelectionCooldownCoroutine() {
        isSelectionCooldownActive = true;
        yield return new WaitForSeconds(0.5f);
        isSelectionCooldownActive = false;
    }


}