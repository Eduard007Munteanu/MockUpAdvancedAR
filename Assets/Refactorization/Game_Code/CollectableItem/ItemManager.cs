using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{


    private bool tilesRendered = false;

    [SerializeField] private GridOverlay gridOverlay;

    // [SerializeField] private Gold goldItem;

    // [SerializeField] private Stone stoneItem;

    // [SerializeField] private Tree treeItem;


    [SerializeField] private DefaultItem[] itemsPrefabTypes; 

    //[SerializeField] private DefaultItem defaultItem;

    private Dictionary<Tile, List<DefaultItem>> itemsData = new Dictionary<Tile, List<DefaultItem>>(); 

    [SerializeField ] private int mineralPlacesToSpawn =  5;
    [SerializeField] private int numberOfMaterialsPerTile = 5;


    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(tilesRendered){
            for(int i=0; i < mineralPlacesToSpawn; i++){
                CreateItemsForTile(numberOfMaterialsPerTile); 
                
            }   
            tilesRendered = false;     
        }
    }

    public void CreateItemsForTile(int numberOfItems){


        DefaultItem specificItemPrefab = itemsPrefabTypes[Random.Range(0, itemsPrefabTypes.Length)];


        Tile randomTile = gridOverlay.GetRandomTile();
        while (itemsData.ContainsKey(randomTile))
        {
            randomTile = gridOverlay.GetRandomTile();
        }

        itemsData[randomTile] = new List<DefaultItem>();

        Vector3[] corners = gridOverlay.GetTileCorners(randomTile);

        

        float fixedHeight = ProjectHeights(randomTile, specificItemPrefab);

        HashSet<Vector3> usedPositions = SpawnOnRandomPositions(numberOfItems, corners, fixedHeight);

        int i = 0;
        foreach (Vector3 position in usedPositions)
        {
            DefaultItem itemObject = Instantiate(specificItemPrefab, position, Quaternion.identity);
            itemObject.Init(i);
            itemObject.name = $"Material_{i}";

            itemsData[randomTile].Add(itemObject);
            i++;
        }






    }


     private float ProjectHeights(Tile randomTile, DefaultItem itemPrefab){
        float tileHeight = randomTile.GetTileHeight();
        float itemHeight = itemPrefab.GetComponent<Renderer>().bounds.size.y;
        float fixedHeight = ((MonoBehaviour)randomTile).transform.position.y + ((tileHeight + (itemHeight / 2f)) / 1f);

        return fixedHeight;
     }


     private HashSet<Vector3> SpawnOnRandomPositions(int numberOfItems, Vector3[] corners, float fixedHeight){

        Vector3 topLeft = corners[0];
        Vector3 topRight = corners[1];
        Vector3 bottomRight = corners[2];
        Vector3 bottomLeft = corners[3];

        HashSet<Vector3> usedPositions = new HashSet<Vector3>();

        for (int i = 0; i < numberOfItems; i++)
        {
            Vector3 randomPosition;
            do
            {
                randomPosition = new Vector3(
                    Random.Range(bottomLeft.x, bottomRight.x),
                    fixedHeight,
                    Random.Range(bottomRight.z, topRight.z)
                );
            }
            while (usedPositions.Contains(randomPosition));

            usedPositions.Add(randomPosition);
        }

        return usedPositions;

     }


    public void TilesRendered(){
        tilesRendered = true;
    }





    
}
