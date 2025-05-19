using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{


    public static ItemManager Instance {get; private set;}


    private bool tilesRendered = false;

    private BetterGridOverlay gridOverlay;


    [SerializeField] private DefaultItem[] itemsPrefabTypes; 
    

    private Dictionary<DefaultTile, List<DefaultItem>> itemsData = new Dictionary<DefaultTile, List<DefaultItem>>(); 

    [SerializeField ] private int mineralPlacesToSpawn =  2;
    [SerializeField] private int numberOfMaterialsPerTile = 5;


    


    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("More than one BuildManager detected. Destroying duplicate.");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gridOverlay = BetterGridOverlay.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(tilesRendered){
            Debug.Log("Tile did render from tileRendered");
            Debug.Log("Number of tiles from tileRendered : " + gridOverlay.GetTiles().Count);

            for (int i = 0; i < mineralPlacesToSpawn; i++)
            {
                CreateItemsForTile(numberOfMaterialsPerTile, itemsPrefabTypes[i % itemsPrefabTypes.Length]);
            }   
            tilesRendered = false;     
        }
    }

    public void CreateItemsForTile(int numberOfItems, DefaultItem defItem = null){


        DefaultItem specificItemPrefab = defItem ? itemsPrefabTypes[Random.Range(0, itemsPrefabTypes.Length)] : null;


        if(gridOverlay == null){
            Debug.LogError("GridOverlay is critical, bruh!");
        } else{
            Debug.Log("GridOverlay is NOT critical, bruh!");
        }

        DefaultTile randomTile = gridOverlay.GetRandomTile();
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
            DefaultItem itemObject = Instantiate(specificItemPrefab, position, specificItemPrefab.transform.rotation);//Quaternion.identity);

            itemObject.transform.position = position;
            itemObject.transform.SetParent(randomTile.transform.parent, true); // keep world position


            itemObject.Init(i);
            itemObject.name = $"Material_{i}";

            itemsData[randomTile].Add(itemObject);
            i++;
        }

    }


    private float ProjectHeights(DefaultTile randomTile, DefaultItem itemPrefab){
        // float tileHeight = randomTile.GetTileHeight();
        // float itemHeight = itemPrefab.GetComponent<Renderer>().bounds.size.y;
        // float fixedHeight = randomTile.transform.position.y + ((tileHeight + (itemHeight / 2f)) / 1f);

        Renderer randomTileRenderer = randomTile.GetComponent<Renderer>();
        Renderer itemRenderer = itemPrefab.GetComponent<Renderer>();

        float tileTopY = randomTileRenderer.bounds.max.y;

        float bottomOffset = itemRenderer.bounds.min.y - itemPrefab.transform.position.y;

        float fixedHeight = tileTopY - bottomOffset;

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
