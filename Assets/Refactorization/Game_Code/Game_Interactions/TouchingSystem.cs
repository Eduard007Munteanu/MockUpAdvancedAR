using Unity.VisualScripting;
using UnityEngine;

// it triggers selection multiple times. pickup -> assign too many times 
// colliding with the same tile which the mob stands on ignore same with tile and house when assigning destination
// it only recognizes tiles and mobs and buildings
// area of the finger is small or big enough for ease of use

public class TouchingSystem : MonoBehaviour{


    [SerializeField] private LayerMask interactableLayer;

    private GameObject currentlyTouching; 


    private DefaultMob selectedMob; 

    void Start(){

    }


    void Update(){

    }


     private void OnTriggerEnter(Collider other) {
        GameObject go = other.gameObject;


        if((interactableLayer.value & (1 << go.layer)) == 0){
            return;
        }

        // play sound effect
        AudioManager.Instance.PlaySoundEffect();

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
            // if (!tile.CheckIfMobOnTile(selectedMob)){
            //     ActionMoveMobToTile(tile);
            // }
            ActionMoveMobToTile(tile);
        }
        else if(selectedMob != null && build != null){
            ActionMoveMobToBuilding(build);
        }

    }


    private void ActionGivenDefaultMob(DefaultMob defaultMobTouched){
        selectedMob = defaultMobTouched;
    }


    public void ActionMoveMobToTile(DefaultTile defaultTileTouched){
        Debug.Log("OnTriggerEnter in ActionMoveMobToTile");
        Vector3 tilePosition = defaultTileTouched.gameObject.transform.position;  
        //Vector3 targetPosition = new Vector3(tilePosition.x, vectorYHeightGivenTile(defaultTileTouched, selectedMob), tilePosition.z);  //Pivot point in empty object parent of tile instead of vectorYHeightGivenTile
        // Vector3 targetPosition = new Vector3(tilePosition.x, 0, tilePosition.z);  //Pivot point in empty object parent of tile instead of vectorYHeightGivenTile
        Vector3 targetPosition = new Vector3(tilePosition.x, selectedMob.transform.position.y, tilePosition.z);  // Use selectedMob's current Y

        selectedMob.RemoveFromBuilding();
        //selectedMob.AssignToBuilding(); // Here we will have the military building assignment. 

        bool canTheMobBeAdded = defaultTileTouched.CanMobBeArrangedChecker();//defaultTileTouched.CanMobBeArrangedChecker(selectedMob);
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


    


}