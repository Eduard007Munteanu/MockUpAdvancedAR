using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialManager : MonoBehaviour


    // TODO:
    // - Create materials sphere gameobjects placed randomly (no collisions between each other) on the specfic tile.
{

    [SerializeField] private GameObject materialPrefab;

    private Dictionary<GameObject, List<GameObject>> materialsData = new Dictionary<GameObject, List<GameObject>>();   //materials spawned per tile. 

    [SerializeField] private GridOnCubus gridOnCubus;

   

    [SerializeField] int mineralPlacesToSpawn = 5;
    [SerializeField] int numberOfMaterialsPerTile = 5; 

    private bool tileRenderedOnRunTime = false; 




    // Start is called before the first frame update
    void Start()
    {
        

    }     

    // Update is called once per frame
    void Update()
    {
        if(tileRenderedOnRunTime){
            for(int i=0; i < mineralPlacesToSpawn; i++){
                CreateMaterialsForTile(numberOfMaterialsPerTile); 
                
            }   
            tileRenderedOnRunTime = false;     
        }
    }

    public void CreateMaterialsForTile(int numberOfMaterials)
    {
        GameObject randomTile = GetRandomTile();

        if (materialsData.ContainsKey(randomTile))
        {
            Debug.Log("Tile already has materials! Trying another tile...");

            
            for (int i = 0; i < 10; i++) 
            {
                randomTile = GetRandomTile();
                if (!materialsData.ContainsKey(randomTile))
                    break;
            }

            if (materialsData.ContainsKey(randomTile))
            {
                Debug.LogWarning("Could not find a unique tile after 10 tries. Skipping material spawn.");
                return;
            }
        }

        materialsData[randomTile] = new List<GameObject>();

        Vector3[] corners = gridOnCubus.GetTileCorners(randomTile);

        Vector3 topLeft = corners[0];
        Vector3 topRight = corners[1];
        Vector3 bottomRight = corners[2];
        Vector3 bottomLeft = corners[3];

        float tileHeight = randomTile.GetComponent<Renderer>().bounds.size.y;
        float materialHeight = materialPrefab.GetComponent<Renderer>().bounds.size.y;
        float fixedHeight = randomTile.transform.position.y + ((tileHeight + (materialHeight / 2f)) / 1f);



        Debug.Log("FixedHeight for random tile in MaterialManager is: " + fixedHeight); //Test




        HashSet<Vector3> usedPositions = new HashSet<Vector3>();

        Color materialColor = GetRandomColor(); 

        for (int i = 0; i < numberOfMaterials; i++)
        {
            Vector3 randomPosition;
            int tries = 0;
            do
            {
                randomPosition = new Vector3(
                    Random.Range(bottomLeft.x, bottomRight.x),
                    fixedHeight,
                    Random.Range(bottomRight.z, topRight.z)
                );
                tries++;
            }
            while (usedPositions.Contains(randomPosition) && tries < 10);

            if (tries >= 10)
            {
                Debug.LogWarning($"Failed to find unique position for material {i}. Skipping.");
                continue;
            }

            usedPositions.Add(randomPosition);

            GameObject material = Instantiate(materialPrefab, randomPosition, Quaternion.identity);
            material.GetComponent<MaterialElement>().Init(materialColor, i.ToString(), randomTile);
            material.transform.SetParent(randomTile.transform);
            material.name = $"Material_{i}";

            materialsData[randomTile].Add(material);
        }
    }


    public GameObject GetRandomTile(){
        Debug.Log("Grid on cubus tiles amount: " + gridOnCubus.GetTiles().Count); //Test
        int randomIndex = Random.Range(0, gridOnCubus.GetTiles().Count);
        GameObject randomTile = gridOnCubus.GetTiles()[randomIndex];
        return randomTile;
    }


    Color GetRandomColor(){
        Color[] colors = { Color.yellow, Color.green , Color.gray };
        int randomIndex = Random.Range(0, colors.Length);
        Color color = colors[randomIndex];
        return color;
    }

    public void SetTileRenderedOnRunTime(){
        tileRenderedOnRunTime = true; 
    }


 
}
