using UnityEngine;
using System.Collections;

public class TilemapManager : MonoBehaviour {
    public bool worldGenerated = false;

    private ElevationNoise terrainGenerator;
    private GameObject[] map;
    private GameObject tilemapGameObject;
    private int mapWidth;
    private int mapHeight;

    void Start () {
        terrainGenerator = GameObject.Find("NoiseGenerator").GetComponent<ElevationNoise>();
	}
	
	void Update () {
        if (!worldGenerated) {
            Destroy(tilemapGameObject);
            tilemapGameObject = new GameObject("Tilemap container");
            map = terrainGenerator.GetMap();
            mapWidth = terrainGenerator.pixWidth;
            
            int x = 0;
            int y = 0;

            for (int i = 1; i <= map.LongLength; i++) {
                // If the tile is on the next line, we increase y
                if ((i - 1) % mapWidth == 0) {
                    Debug.Log("New line");
                    y += terrainGenerator.tileSize;
                    x = 0;
                }
                Debug.Log(i);
                Debug.Log("x : " + x + " -- y : " + y);
                Instantiate(map[i - 1], new Vector3(x, y, 0), Quaternion.identity, tilemapGameObject.transform);
                x += terrainGenerator.tileSize;
            }
            worldGenerated = true;
        }
    }
}
