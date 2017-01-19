using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilemapManager : MonoBehaviour {
    public bool worldGenerated = false;
    public GameObject[][] tilemap = null;

    private ElevationNoise terrainGenerator;
    private GameObject[] map;
    private GameObject tilemapGameObject;
    public int mapWidth;
    public int mapHeight;

    void Start () {
        terrainGenerator = GameObject.Find("NoiseGenerator").GetComponent<ElevationNoise>();
	}

    public List<Tile> getNeighbours(Tile tile) {
        List<Tile> neighbours = new List<Tile>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                // If we want to remove diagonals
                if (Mathf.Abs(x) == Mathf.Abs(y))
                    continue;

                int checkX = tile.position.x + x;
                int checkY = tile.position.y + y;

                if(checkX >= 0 && checkX < mapWidth && checkY >= 0 && checkY < mapHeight) {
                    neighbours.Add(tilemap[checkY][checkX].GetComponent<Tile>());
                }
            }
        }

        return neighbours;
    }

    public bool isWalkable(Position position) {
        if(tilemap[position.y][position.x].GetComponent<Tile>().isWalkable) {
            return true;
        }

        return false;
    }
	
	void Update () {
        if (!worldGenerated) {
            Destroy(tilemapGameObject);
            tilemapGameObject = new GameObject("Tilemap container");
            map = terrainGenerator.GetMap();
            mapWidth = terrainGenerator.pixWidth;
            mapHeight = terrainGenerator.pixHeight;

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

                int xIndex = x / terrainGenerator.tileSize;
                int yIndex = y / terrainGenerator.tileSize;
                
                GameObject tile = Instantiate(map[i - 1], new Vector3(x, y, 0), Quaternion.identity, tilemapGameObject.transform) as GameObject;
                tile.name = "tile_" + xIndex + "_" + yIndex;
                tile.GetComponent<Tile>().position = new Position(xIndex, yIndex);
                tilemap[yIndex][xIndex] = tile;

                x += terrainGenerator.tileSize;
            }
            worldGenerated = true;
        }
    }
}
