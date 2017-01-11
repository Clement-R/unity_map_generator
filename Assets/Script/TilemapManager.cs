using UnityEngine;
using System.Collections;

public class TilemapManager : MonoBehaviour {
    public bool worldGenerated = false;
    public GameObject[][] tilemap = null;

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

            tilemap = new GameObject[terrainGenerator.pixHeight][];

            int x = 0;
            int y = 0;

            tilemap[y] = new GameObject[terrainGenerator.pixWidth];
            for (int i = 1; i <= map.LongLength; i++) {
                // If the tile is on the next line, we increase y
                if ((i - 1) % mapWidth == 0 && (i -1) != 0) {
                    y += terrainGenerator.tileSize;
                    tilemap[y / terrainGenerator.tileSize] = new GameObject[terrainGenerator.pixWidth];
                    x = 0;
                }
                
                GameObject tile = Instantiate(map[i - 1], new Vector3(x, y, 0), Quaternion.identity, tilemapGameObject.transform) as GameObject;
                tile.name = "tile" + i;
                tile.GetComponent<Tile>().position = new Vector2(x / terrainGenerator.tileSize, y / terrainGenerator.tileSize);
                tilemap[y / terrainGenerator.tileSize][x / terrainGenerator.tileSize] = tile;

                x += terrainGenerator.tileSize;
            }
            worldGenerated = true;
        }
    }
}
